document.addEventListener("DOMContentLoaded", () => {

    const resultTitle = document.querySelector(".result-header h3");

    // =========================
    // LIMPIAR RESULTADOS
    // =========================
    function clearResults(methodName = "") {
        // Iteraciones
        document.getElementById("res-iterations").innerText = "-";
        // Error
        document.getElementById("res-error").innerText = "-";
        // Raíz
        document.getElementById("res-root").innerText = "-";
        const intervalEl = document.getElementById("res-interval");
        // Intervalo
        if (methodName.toLowerCase() === "newton-raphson") {
            intervalEl.innerText = "Punto inicial: -";
        } else {
            intervalEl.innerText = "Intervalo: -";
        }
        // Tolerancia
        document.getElementById("res-tolerance").innerText = "Tolerancia: -";
        // Función
        document.getElementById("res-function").innerText = "";
    }

    // =========================
    // SWITCH PRINCIPAL
    // =========================
    function loadMethods(option) {

        const container = option.closest(".switch-container");
        const slider = container.querySelector(".switch-slider");
        const options = container.querySelectorAll(".switch-option");

        options.forEach(o => o.classList.remove("active"));
        option.classList.add("active");

        const index = option.dataset.index;
        slider.style.transform = `translateX(${index * 100}%)`;

        const url = option.dataset.url;

        fetch(url)
            .then(res => res.text())
            .then(html => {

                document.getElementById("methods-container").innerHTML = html;

                clearResults();

                // activar automáticamente el primer submétodo
                const firstSub = document.querySelector(".switch-sub.active")
                    || document.querySelector(".switch-sub");

                if (firstSub) loadSubMethod(firstSub);
            });
    }

    // =========================
    // SWITCH SECUNDARIO
    // =========================
    function loadSubMethod(option) {

        const container = option.closest(".switch-container");
        const slider = container.querySelector(".switch-slider");
        const options = container.querySelectorAll(".switch-sub");

        options.forEach(o => o.classList.remove("active"));
        option.classList.add("active");

        const index = option.dataset.index;
        slider.style.transform = `translateX(${index * 100}%)`;

        const url = option.dataset.url;

        fetch(url)
            .then(res => res.text())
            .then(html => {

                document.getElementById("form-container").innerHTML = html;

                const methodName = option.innerText.trim();
                clearResults(methodName);

                // actualizar título del panel
                if (resultTitle)
                    resultTitle.innerText = option.innerText.trim();
            });
    }

    // =========================
    // EVENTOS
    // =========================
    document.addEventListener("click", function (e) {

        if (e.target.classList.contains("switch-option")) {
            loadMethods(e.target);
        }

        if (e.target.classList.contains("switch-sub")) {
            loadSubMethod(e.target);
        }

    });

    // =========================
    // INICIALIZAR
    // =========================
    const active = document.querySelector(".switch-option.active");
    if (active) loadMethods(active);

});