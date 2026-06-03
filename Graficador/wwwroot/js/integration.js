document.addEventListener("DOMContentLoaded", () => {
    // Inicializar GeoGebra
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

    // Deshabilitar Input 'N' si el método es "Simple" o "3/8" (que asumen N fijo)
    const methodSelect = document.getElementById("method-select");
    const nInput = document.getElementById("n-input");

    methodSelect.addEventListener("change", (e) => {
        const val = e.target.value;
        if (val.includes("simple") || val === "simpson_38") {
            nInput.value = "";
            nInput.disabled = true;
        } else {
            nInput.disabled = false;
        }
    });

    // Formulario Submit
    document.getElementById("integration-form").addEventListener("submit", async function (e) {
        e.preventDefault();

        const method = methodSelect.value;
        const nValue = parseInt(nInput.value);

        // Validación frontend Simpson 1/3 múltiple requiere par si no usas el combinado
        if (method === "simpson_13_multiple" && nValue % 2 !== 0) {
            alert("El método Simpson 1/3 Múltiple puro requiere una cantidad PAR de subintervalos. Utiliza el método 'Combinado' si n es impar.");
            return;
        }

        let rawFunction = document.getElementById("fx-input").value;
        let cleanedFunction = rawFunction.replace(/\s+/g, '').replace(',', '.');

        let payload = {
            function: cleanedFunction,
            xi: parseFloat(document.getElementById("xi-input").value),
            xd: parseFloat(document.getElementById("xd-input").value),
            n: isNaN(nValue) ? 0 : nValue,
            method: method
        };

        try {
            const response = await fetch("/api/integrations/calculate", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(payload)
            });

            const text = await response.text();
            if (!response.ok) throw new Error(text);

            const data = JSON.parse(text);

            // Mostrar Resultado
            document.getElementById("res-area").innerText = data.areaFormateada;

            const selectedText = methodSelect.options[methodSelect.selectedIndex].text;
            document.getElementById("res-method").innerText = selectedText;

            // Dibujar en GeoGebra
            drawInGeoGebra(payload.function, payload.xi, payload.xd);

        } catch (error) {
            alert("Error: " + error.message);
        }
    });

    function drawInGeoGebra(fx, xi, xd) {
        if (typeof ggbApp === "undefined") return;

        ggbApp.reset();

        // Reemplazar notación C# a GeoGebra si hace falta
        let ggbFunction = fx.replace("y =", "f(x) =");
        if (!ggbFunction.startsWith("f(x)")) ggbFunction = "f(x) = " + ggbFunction;

        // 1. Dibujar la función
        ggbApp.evalCommand(ggbFunction);

        // 2. Usar el comando nativo de GeoGebra para sombrear el área de la integral
        ggbApp.evalCommand(`Integral(f, ${xi}, ${xd})`);
    }
});