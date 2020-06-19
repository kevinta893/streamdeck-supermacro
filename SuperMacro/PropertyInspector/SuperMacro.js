console.log("Index");
document.addEventListener('websocketCreate', function () {
    console.log("Websocket created!");
    showHideSettings(actionInfo.payload.settings);

    websocket.addEventListener('message', function (event) {
        console.log("Got message event!");
        var jsonObj = JSON.parse(event.data);

        if (jsonObj.event === 'sendToPropertyInspector') {
            var payload = jsonObj.payload;
            showHideSettings(payload);
        }
        else if (jsonObj.event === 'didReceiveSettings') {
            var payload = jsonObj.payload;
            showHideSettings(payload.settings);
        }
    });
});

function showHideSettings(payload) {
    console.log("Show Hide Settings Called");
    setTextAreas("");
    setFileNames("none");
    if (payload['loadFromFiles']) {
        setTextAreas("none");
        setFileNames("");
    }
}

function setTextAreas(displayValue) {
    var dvTextAreas = document.getElementById('dvTextAreas');
    dvTextAreas.style.display = displayValue;
}

function setFileNames(displayValue) {
    var dvFileNames = document.getElementById('dvFileNames');
    dvFileNames.style.display = displayValue;
}