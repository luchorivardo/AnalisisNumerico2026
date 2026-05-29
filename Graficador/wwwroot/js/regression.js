let currentMethod = "linear";
let pointsArray = []; // Array para almacenar las coordenadas [x, y]

document.addEventListener("DOMContentLoaded", () => {

    // =========================
    // INICIALIZAR GEOGEBRA
    // =========================
    const parameters = {
        "id": "ggbApp",
        "width": 650,
        "height": 450,
        "showToolBar": false,
        "showMenuBar": false,
        "showAlgebraInput": false,
        "language": "es"
    };
    const applet = new GGBApplet(parameters, true);
    applet.inject('ggb-element');

    // =========================
    // SWITCH MÉTODOS
    // =========================
    const container = document.querySelector(".switch-container");
    const slider = container.querySelector(".switch-slider");
    const options = container.querySelectorAll(".switch-option");

    options.forEach(option => {
        option.addEventListener("click", () => {
            options.forEach(o => o.classList.remove("active"));
            option.classList.add("active");

            currentMethod = option.dataset.method;

            // Mover slider
            const index = option.dataset.index;
            slider.style.transform = `translateX(${index * 100}%)`;

            // Actualizar título
            let methodTitle = option.innerText.trim();
            document.getElementById("result-title").innerText = `Regresión ${methodTitle}`;

            // Mostrar/Ocultar campos de Polinomial
            const polyFields = document.querySelectorAll(".poly-only");
            if (currentMethod === "polynomial") {
                polyFields.forEach(f => f.style.display = "block");
            } else {
                polyFields.forEach(f => f.style.display = "none");
            }
        });
    });

    // =========================
    // GESTIÓN DE PUNTOS
    // =========================
    const xInput = document.getElementById("x-input");
    const yInput = document.getElementById("y-input");
    const btnAdd = document.getElementById("btn-add-point");
    const pointsList = document.getElementById("points-list");

    function updatePointsUI() {
        pointsList.innerHTML = "";
        pointsArray.forEach((p, index) => {
            const option = document.createElement("option");
            option.text = `Punto ${index} = (${p[0]}, ${p[1]})`;
            pointsList.add(option);
        });
    }

    // Agregar punto
    btnAdd.addEventListener("click", () => {
        const x = parseFloat(xInput.value);
        const y = parseFloat(yInput.value);

        if (!isNaN(x) && !isNaN(y)) {
            pointsArray.push([x, y]);
            updatePointsUI();

            // Limpiar inputs y devolver el foco a X
            xInput.value = "";
            yInput.value = "";
            xInput.focus();
        } else {
            alert("Por favor, ingrese valores numéricos válidos para X e Y.");
        }
    });

    // Borrar último
    document.getElementById("btn-delete-last").addEventListener("click", () => {
        if (pointsArray.length > 0) {
            pointsArray.pop();
            updatePointsUI();
        }
    });

    // Borrar todos
    document.getElementById("btn-delete-all").addEventListener("click", () => {
        pointsArray = [];
        updatePointsUI();
        if (typeof ggbApp !== "undefined") ggbApp.reset(); // Limpia la gráfica también
    });


    // =========================
    // SUBMIT (ENVIAR AL BACKEND)
    // =========================
    document.getElementById("regression-form").addEventListener("submit", async function (e) {
        e.preventDefault();

        if (pointsArray.length < 2) {
            alert("Se requieren al menos 2 puntos para realizar una regresión.");
            return;
        }

        // 1. Armar el payload basado en el modelo C#
        let payload = {
            puntosCargados: pointsArray,
            tolerancia: parseFloat(document.getElementById("tolerance").value),
            method: currentMethod
        };

        // Si es polinomial, mandamos el grado (deberás agregar 'Degree' a tu Request en C# luego)
        if (currentMethod === "polynomial") {
            payload.degree = parseInt(document.getElementById("poly-degree").value);
        }

        console.log("PAYLOAD REGRESIÓN:", payload);

        try {
            // Asegúrate de que la ruta coincida con el controlador que creamos
            const response = await fetch("/api/regression/calculate", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(payload)
            });

            const text = await response.text();

            if (!response.ok) {
                throw new Error(text);
            }

            const data = JSON.parse(text);

            // 2. Mostrar Resultados en la UI
            document.getElementById("res-function").innerText = data.funcion;
            document.getElementById("res-correlation").innerText = data.efectividadPorcentaje;
            document.getElementById("res-effectiveness").innerText = data.efectividadMensaje;

            // 3. Dibujar en GeoGebra
            drawInGeoGebra(data.funcion);

        } catch (error) {
            alert("Error: " + error.message);
        }
    });

    // =========================
    // FUNCIONES AUXILIARES
    // =========================
    function drawInGeoGebra(functionString) {
        if (typeof ggbApp === "undefined") {
            console.error("GeoGebra no está inicializado.");
            return;
        }

        // Reiniciar la vista gráfica
        ggbApp.reset();

        // Dibujar los puntos
        pointsArray.forEach((p, index) => {
            // Crea puntos con nombres como P_0, P_1, etc.
            ggbApp.evalCommand(`P_{${index}} = (${p[0]}, ${p[1]})`);
        });

        // Dibujar la función obtenida. 
        // GeoGebra prefiere "f(x) =" en lugar de "y =", así que lo reemplazamos si es necesario.
        let ggbFunction = functionString.replace("y =", "f(x) =");
        ggbApp.evalCommand(ggbFunction);
    }
});