console.log("JS cargado");
document.addEventListener("submit", function (e) {
    console.log("Submit detectado");
    const forms = ["bisection-form", "falseRule-form"];

    if (e.target.classList.contains("form")) {
        e.preventDefault();

        const formData = new FormData(e.target);
        const xi = formData.get("xi");
        const xd = formData.get("xd");

        const data = {
            method: formData.get("method"),
            function: formData.get("function"),
            xStart: xi ? parseFloat(xi) : null,
            xEnd: xd ? parseFloat(xd) : null,
            tolerance: parseFloat(formData.get("tolerance")),
            maxIterations: parseInt(formData.get("iterations"))
        };

        fetch("/api/calculator/calculate", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(data)
        })
            .then(res => res.json())
            .then(result => {

                console.log(result);

                const last = result.iterations[result.iterations.length - 1];

                // Iteraciones
                document.getElementById("res-iterations").innerText =
                    `${result.iterations.length}/${data.maxIterations}`;

                // Error
                document.getElementById("res-error").innerText =
                    last.error.toFixed(6);

                // Raíz
                document.getElementById("res-root").innerText =
                    parseFloat(result.root).toFixed(6);

                // Intervalo
                const intervalEl = document.getElementById("res-interval");

                if (data.method === "newton") {
                    intervalEl.innerText = `Punto inicial: ${data.xStart}`;
                } else {
                    intervalEl.innerText = `Intervalo: [${data.xStart}, ${data.xEnd}]`;
                }

                // Tolerancia
                document.getElementById("res-tolerance").innerText =
                    `Tolerancia: ${data.tolerance}`;

                // Función
                document.getElementById("res-function").innerText =
                    data.function;

                // ======================
                // GEO GEBRA
                // ======================
                const ggb = window.ggb;

                if (result.ggbCommand && ggb) {

                    const commands = result.ggbCommand.split(";");

                    // Limpiar gráfico anterior
                    ggb.reset();

                    commands.forEach(cmd => {
                        if (cmd.trim() !== "") {
                            ggb.evalCommand(cmd);
                        }
                    });
                }
            })
            .catch(err => console.error(err));
    }
});