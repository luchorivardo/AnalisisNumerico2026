let currentMethod = "gaussjordan";

document.addEventListener("DOMContentLoaded", () => {

    const sizeInput = document.getElementById("matrix-size");

    // generar matriz inicial
    generateMatrix(parseInt(sizeInput.value));

    sizeInput.addEventListener("change", () => {
        generateMatrix(parseInt(sizeInput.value));
    });

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

            // método seleccionado
            currentMethod = option.dataset.method;

            // mover slider
            const index = option.dataset.index;
            slider.style.transform = `translateX(${index * 100}%)`;

            // actualizar título
            document.querySelector(".result-header h3").innerText =
                option.innerText.trim();

            // mostrar parámetros de seidel
            const seidelFields = document.querySelectorAll(".seidel-only");

            if (currentMethod === "gaussseidel") {
                seidelFields.forEach(f => f.style.display = "flex");
            } else {
                seidelFields.forEach(f => f.style.display = "none");
            }
        });

    });

    document.getElementById("system-form").addEventListener("submit", async function (e) {
        e.preventDefault();

        const size = parseInt(document.getElementById("matrix-size").value);
        const method = document.querySelector(".switch-option.active").dataset.method;

        // ====== 1. Leer matriz del HTML ======
        let matrix = [];

        for (let i = 0; i < size; i++) {
            let row = [];

            for (let j = 0; j < size + 1; j++) {
                const input = document.getElementById(`cell-${i}-${j}`);
                row.push(parseFloat(input.value) || 0);
            }

            matrix.push(row);
        }

        // ====== 2. Armar request ======
        const payload = {
            method: method,
            matrix: matrix
        };

        try {
            // ====== 3. Llamar API ======
            const response = await fetch("/api/calculator/calculate", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(payload)
            });

            let data;

            if (response.ok) {
                data = await response.json();

                document.getElementById("solution-container").innerHTML =
                    data.root.split("|").map(x => `<div>${x}</div>`).join("");

            } else {
                const errorText = await response.text();
                throw new Error(errorText);
            }

            // ====== 4. Mostrar resultado ======
            document.getElementById("solution-container").innerHTML = data.root.split("|").map(x => `<div>${x}</div>`).join("");

        } catch (error) {
            alert("Error: " + error.message);
        }
    });
});


// =========================
// GENERAR MATRIZ
// =========================
function generateMatrix(n) {

    const container = document.getElementById("matrix-container");
    container.innerHTML = "";

    for (let i = 0; i < n; i++) {

        const row = document.createElement("div");
        row.classList.add("matrix-row");

        for (let j = 0; j < n; j++) {

            const input = document.createElement("input");
            input.type = "number";
            input.classList.add("matrix-input");

            // CLAVE
            input.id = `cell-${i}-${j}`;

            row.appendChild(input);
        }

        const equal = document.createElement("span");
        equal.innerText = "=";
        row.appendChild(equal);

        const b = document.createElement("input");
        b.type = "number";
        b.classList.add("matrix-input");

        // CLAVE (última columna)
        b.id = `cell-${i}-${n}`;

        row.appendChild(b);

        container.appendChild(row);
    }
}