function StartSSE(id) {
    var source = new EventSource('/api/sse/message/' + id);
    var source2 = new EventSource('/api/sse/message/all');

    var ul = document.getElementById('sse');
    var ul2 = document.getElementById('sse2');

    source.onmessage = function (e) {      
        var li = document.createElement('li');
        if (!!e.data) {
            var retrievedData = JSON.parse(e.data)
            li.textContent = retrievedData.Message;
            ul.appendChild(li);
            console.log(retrievedData.Message);
        }        
    }

    source2.onmessage = function (e) {
        var li = document.createElement('li');
        if (!!e.data) {
            var retrievedData = JSON.parse(e.data)
            li.textContent = retrievedData.Message;
            ul2.appendChild(li);
            console.log(retrievedData.Message);
        }
    }
}

