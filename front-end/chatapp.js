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
    
    const connection = new signalR.HubConnectionBuilder().withUrl("http://192.168.2.100:8080/chat").build();

    connection.on("/chat", function (user, message) {
        const $responseElement = $('<div>').addClass('message other').text(`${user}: ${message}`);
        $chatMessages.append($responseElement);
        $chatMessages.scrollTop($chatMessages[0].scrollHeight);
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

            setTimeout(() => {
                const $responseElement = $('<div>').addClass('message other').text(`Bot: Recebi sua mensagem: "${messageText}"`);
                $chatMessages.append($responseElement);
                $chatMessages.scrollTop($chatMessages[0].scrollHeight);
            }, 1000);
        }
    });

    $messageInput.on('keypress', function (e) {
        if (e.key === 'Enter') {
            $sendMessageButton.click();
        }
    });
});