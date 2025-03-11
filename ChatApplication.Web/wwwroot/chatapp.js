$(document).ready(function () {

    const $popup = $('#popup');
    const $chatContainer = $('#chat');
    const $usernameInput = $('#username');
    const $startChatButton = $('#startChat');
    const $chatMessages = $('#chatMessages');
    const $messageInput = $('#messageInput');
    const $sendMessageButton = $('#sendMessage');

    $startChatButton.prop('disabled', true); // disabled until connected

    let username = localStorage.getItem('uname');

    if (!username) {
        $popup.css('display', 'flex');
    } else {
        $popup.hide();
        $chatContainer.css('display', 'flex');
    }

    let messages = {};

    $.ajax({
        async: false,
        url: "/messages/",
        type: "GET",
        contentType: "application/json",
        success: function (response) {
            messages = response;
        }
    });

    $.each(messages, function (_, e) {
        let { messageContent, messageDate, messageSender } = e;

        if (messageSender != username) {
            const $responseElement = $('<div>').addClass('message other').text(`${messageSender}: ${messageContent}`);
            $chatMessages.append($responseElement);
            $chatMessages.scrollTop($chatMessages[0].scrollHeight);
        } else {
            const $messageElement = $('<div>').addClass('message user').text(`${username}: ${messageContent}`);
            $chatMessages.append($messageElement);
            $messageInput.val('');
            $chatMessages.scrollTop($chatMessages[0].scrollHeight);
        }
    })

    $startChatButton.on('click', function () {
        username = $usernameInput.val().trim();
        localStorage.setItem('uname', username);
        if (username) {
            $popup.hide();
            $chatContainer.css('display', 'flex');
        } else {
            alert('Por favor, insira seu nome.');
        }
    });

    const connection = new signalR.HubConnectionBuilder().withUrl("/hubs/chat").build();

    connection.on("ReceiveMessage", function (user, message) {
        if (user != username) {
            const $responseElement = $('<div>').addClass('message other').text(`${user}: ${message}`);
            $chatMessages.append($responseElement);
            $chatMessages.scrollTop($chatMessages[0].scrollHeight);
        }        
    });

    connection.start().then(function () {
        $startChatButton.prop('disabled', false); // disabled until connected
    }).catch(function (err) {
        return console.error(err);
    });

    $sendMessageButton.on('click', function () {
        const messageText = $messageInput.val().trim();
        if (messageText) {
            const $messageElement = $('<div>').addClass('message user').text(`${username}: ${messageText}`);
            $chatMessages.append($messageElement);
            $messageInput.val('');
            $chatMessages.scrollTop($chatMessages[0].scrollHeight);

            connection.invoke("SendMessage", username, messageText);

            $.ajax({
                url: "/messages/",
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify({
                    "MessageContent": messageText,
                    "MessageSender": username,
                    "MessageDate": new Date().toISOString()
                }),
                success: function (response) {
                    console.log("Message sent successfully:", response);
                },
                error: function (_, _, error) {
                    console.error("Error sending message:", error);
                }
            });

        }
    });

    $messageInput.on('keypress', function (e) {
        if (e.key === 'Enter') {
            $sendMessageButton.click();
        }
    });
});