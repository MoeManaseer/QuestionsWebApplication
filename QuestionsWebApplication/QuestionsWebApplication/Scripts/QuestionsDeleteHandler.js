let deleteQuestionResponseContainer;

const createResponseMessage = (requestResponse, message) => {
    if (deleteQuestionResponseContainer) {
        const messageNode = document.createElement('div');
        messageNode.classList.add('alert', `alert-${requestResponse}`);
        messageNode.innerText = message;

        if (deleteQuestionResponseContainer.firstChild) {
            deleteQuestionResponseContainer.insertBefore(messageNode, deleteQuestionResponseContainer.firstChild);
        }
        else {
            deleteQuestionResponseContainer.insert(message);
        }

        setTimeout(() => {
            messageNode.classList.add('fade');
            setTimeout(() => {
                messageNode.remove();
            }, 150)
        }, 4850);
    }
}

const RemoveQuestion = (resultData = {}, questionId = 0) => {
    const {
        didDelete = false,
        requestResponse = 'error',
        message = 'Error in deleting the question.. please try again'
    } = resultData;

    if (didDelete) {
        const currentQuestionContainer = document.querySelector(`.question__container-${questionId}`);
        currentQuestionContainer.remove();
    }

    createResponseMessage(requestResponse, message);
};

const RemoveQuestionAjax = (questionId) => {
    $.ajax({
        url: '/Questions/OnDeleteQuestion',
        contentType: 'application/json; charset=utf-8',
        type: 'POST',
        dataType: 'json',
        data: JSON.stringify({ 'pQuestionId' : questionId }),
        success: (result) => {
            RemoveQuestion(result, questionId);
        },
        error: (error) => {
            console.log(error);
            return {};
        },
    });
};

const AddRemoveEventListeners = () => {
    const deleteQuestionButtons = document.querySelectorAll('.question__delete-btn');

    if (deleteQuestionButtons) {
        deleteQuestionButtons.forEach((deleteButton) => {
            deleteButton.addEventListener('click', (event) => {
                RemoveQuestionAjax(deleteButton.id.split('-')[1]);
            });
        });
    }
};

document.addEventListener('DOMContentLoaded', () => {
    deleteQuestionResponseContainer = document.querySelector('.questions__delete-response-container');
    AddRemoveEventListeners();
});
