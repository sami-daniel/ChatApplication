$(document).ready(function () {
    const $popup = $('#popup');
    const $chatContainer = $('#chat');
    const $usernameInput = $('#username');
    const $startChatButton = $('#startChat');
    const $chatMessages = $('#chatMessages');
    const $messageInput = $('#messageInput');
    const $sendMessageButton = $('#sendMessage');

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