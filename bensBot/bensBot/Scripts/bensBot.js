function sendText() {
    var textBox = document.getElementById('chatText');
    var message = textBox.value;

    textBox.value = '';

    sendTextMessage(message);
}

function sendTextMessage(message) {
    var request = $.ajax({
        url: 'api/Bot/SendBotMessage',
        type: 'POST',
        async: false,
        dataType: 'text',
        headers: { 'Accept': 'application/json', 'Content-Type': 'application/json' },
        data: JSON.stringify(message),
        success: function (data) {
            var reply = JSON.parse(data);

            if (reply.FromMessage === "")
                return;

            //$('#chatBox').html($('#chatBox').html() + reply.FromMessage + "<br />");
            $('#chatBox').html($('#chatBox').html() + formatText(1, reply.FromMessage) + "<br /><br />");
            $('#chatBox').scrollTop($('#chatBox')[0].scrollHeight);
        }
    });

    request.fail(function (xhr) {
        alert(xhr.responseText);
    });
}

var ws;
var userId;

function startConversation() {
    $('#chatBox').html($('#chatBox').html() + "Please wait, connecting..." + "<br /><br />");
    $('#chatBox').scrollTop($('#chatBox')[0].scrollHeight);

    var request = $.ajax({
        url: 'api/Bot/StartConversation',
        type: 'GET',
        async: false,
        dataType: 'text',
        headers: { 'Accept': 'application/json', 'Content-Type': 'application/json' },
        success: function (data) {
            var client = JSON.parse(data);

            userId = client.UserId;
            connectWebSocket(client.StreamUrl);
        }
    });

    request.fail(function (xhr) {
        alert(xhr.responseText);
    });
}

function connectWebSocket(url) {
    ws = new WebSocket(url);
    ws.binaryType = 'blob';

    ws.onopen = function () {
        $("#spanStatus").text("connected");
        sendTextMessage('');
    };
    ws.onmessage = function (evt) {
        gotData(evt.data);
    };
    ws.onerror = function (evt) {
        $("#spanStatus").text(evt.message);
    };
    ws.onclose = function () {
        $("#spanStatus").text("disconnected");
    };
}

function gotData(data) {
    if (data === '')
        return;

    var message = JSON.parse(data);

    if (message.activities[0].from.id !== userId) {
        //$('#chatBox').html($('#chatBox').html() + message.activities[0].text + "<br />");
        $('#chatBox').html($('#chatBox').html() + formatText(0, message.activities[0].text) + "<br /><br />");
        $('#chatBox').scrollTop($('#chatBox')[0].scrollHeight);
    }
}

function formatText(type, message) {
    var className;

    if (type === 0)
        className = "BotText";
    else
        className = "UserText";

    var divText = "<div class='" + className + "' >" + message + "</div";

    return divText;
}