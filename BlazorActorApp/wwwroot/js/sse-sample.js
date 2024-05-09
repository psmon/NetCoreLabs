function display(id) {
    var source = new EventSource('/api/sse/message/' + id);

    var ul = document.getElementById("sse");
    source.onmessage = function (e) {
        var li = document.createElement("li");
        if (!!e.data) {
            var retrievedData = JSON.parse(e.data)
            li.textContent = retrievedData.Message;
            ul.appendChild(li);

            console.log(retrievedData.Message);

        }
        
    }
}

