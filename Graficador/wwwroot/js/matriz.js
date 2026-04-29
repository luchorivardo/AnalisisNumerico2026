let currentMethod = "gaussjordan";

document.addEventListener("DOMContentLoaded", () => {

    const sizeInput = document.getElementById("matrix-size");

    // ====== MATRIZ INICIAL ======
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

            currentMethod = option.dataset.method;

            // mover slider
            const index = option.dataset.index;
            slider.style.transform = `translateX(${index * 100}%)`;

            // actualizar título
            document.querySelector(".result-header h3").innerText =
                option.innerText.trim();

            // mostrar campos de Seidel
            const seidelFields = document.querySelectorAll(".seidel-only");

            if (currentMethod === "gaussseidel") {
                seidelFields.forEach(f => f.style.display = "flex");
            } else {
                seidelFields.forEach(f => f.style.display = "none");
            }
        });
    });

    // =========================
    // SUBMIT
    // =========================
    document.getElementById("system-form").addEventListener("submit", async function (e) {
        e.preventDefault();

        const size = parseInt(document.getElementById("matrix-size").value);

        // ====== 1. LEER MATRIZ ======
        let matrix = [];

        for (let i = 0; i < size; i++) {
            let row = [];

            for (let j = 0; j < size + 1; j++) {
                const input = document.getElementById(`cell-${i}-${j}`);
                const val = parseFloat(input.value);
                row.push(isNaN(val) ? 0 : val);
            }

            matrix.push(row);
        }

        // ====== 2. PAYLOAD BASE ======
        let payload = {
            method: currentMethod,
            matrix: matrix
        };

        // ====== 3. SI ES SEIDEL → agregar params ======
        if (currentMethod === "gaussseidel") {
            payload.tolerance = parseFloat(document.getElementById("tolerance").value);
            payload.maxIterations = parseInt(document.getElementById("iterations").value);
        }

        console.log("PAYLOAD:", payload);

        try {
            const response = await fetch("/api/calculator/calculate", {
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

            // ====== 4. MOSTRAR RESULTADO ======
            document.getElementById("solution-container").innerHTML =
                data.root.split("|").map(x => `<div>${x.trim()}</div>`).join("");

            // ====== 5. MOSTRAR ITERACIONES ======
            if (data.iterations && data.iterations.length > 0) {

                const iterationsContainer = document.getElementById("iterations-container");

                const isSystem = data.iterations[0].values && data.iterations[0].values.length > 0;
                const numVars = isSystem ? data.iterations[0].values.length : 0;

                let html = `<table class="iterations-table">`;

                // ===== HEADER =====
                html += "<thead><tr>";
                html += "<th>Iter</th>";

                if (isSystem) {
                    for (let i = 0; i < numVars; i++) {
                        html += `<th>x${i + 1}</th>`;
                    }
                } else {
                    html += "<th>x</th><th>f(x)</th>";
                }

                html += "<th>Error</th>";
                html += "</tr></thead><tbody>";

                // ===== BODY =====
                data.iterations.forEach(it => {

                    html += `<tr>`;
                    html += `<td>${it.iteration}</td>`;

                    if (isSystem) {
                        it.values.forEach(v => {
                            html += `<td>${v.toFixed(6)}</td>`;
                        });
                    } else {
                        html += `<td>${it.x.toFixed(6)}</td>`;
                        html += `<td>${it.y.toFixed(6)}</td>`;
                    }

                    html += `<td>${it.error ? it.error.toExponential(2) : "-"}</td>`;
                    html += `</tr>`;
                });

                html += "</tbody></table>";

                iterationsContainer.innerHTML = html;
            }

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
            input.step = "any";
            input.classList.add("matrix-input");

            input.id = `cell-${i}-${j}`;

            row.appendChild(input);
        }

        const equal = document.createElement("span");
        equal.innerText = "=";
        row.appendChild(equal);

        const b = document.createElement("input");
        b.type = "number";
        b.step = "any";
        b.classList.add("matrix-input");

        b.id = `cell-${i}-${n}`;

        row.appendChild(b);

        container.appendChild(row);
    }
}