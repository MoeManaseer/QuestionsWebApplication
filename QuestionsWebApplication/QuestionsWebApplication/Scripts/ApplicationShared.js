const createResponseMessage = (requestResponse, message, container) => {
    if (container) {
        const messageNode = document.createElement('div');
        messageNode.classList.add('alert', `alert-${requestResponse}`);
        messageNode.innerText = message;

        if (container.firstChild) {
            container.insertBefore(messageNode, container.firstChild);
        }
        else {
            container.insert(message);
        }

        setTimeout(() => {
            messageNode.classList.add('fade');
            setTimeout(() => {
                messageNode.remove();
            }, 150)
        }, 4850);
    }
};