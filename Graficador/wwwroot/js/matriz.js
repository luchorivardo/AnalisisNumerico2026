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
            input.name = `a_${i}_${j}`;

            row.appendChild(input);
        }

        const equal = document.createElement("span");
        equal.innerText = "=";
        row.appendChild(equal);

        const b = document.createElement("input");
        b.type = "number";
        b.classList.add("matrix-input");
        b.name = `b_${i}`;

        row.appendChild(b);

        container.appendChild(row);
    }
}

document.getElementById("system-form")
    .addEventListener("submit", function (e) {

        e.preventDefault();

        console.log("Calcular sistema");

    });