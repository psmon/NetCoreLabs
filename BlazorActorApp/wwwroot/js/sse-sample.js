function display() {
    var source = new EventSource('/api/sse/message');

    //var source = new EventSource('/api/sse/subscribe/a');

    //subscribe
    var ul = document.getElementById("sse");
    source.onmessage = function (e) {
        var li = document.createElement("li");
        var retrievedData = JSON.parse(e.data)
        li.textContent = retrievedData.message;
        ul.appendChild(li);
    }
}

