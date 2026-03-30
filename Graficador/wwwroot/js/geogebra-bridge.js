let ggb;

window.addEventListener("load", function () {

    const params = {
        appName: "graphing",
        width: 1100,
        height: 400,
        showToolBar: false,
        showAlgebraInput: false,
        showMenuBar: false,
        language: "en",

        appletOnLoad: function (api) {
            ggb = api;
            window.ggb = api; 
            console.log("GeoGebra listo");
        }
    };

    const app = new GGBApplet(params, true);
    app.inject('ggb-element');

});