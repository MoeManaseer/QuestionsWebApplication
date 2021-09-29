let deleteQuestionResponseContainer;

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

    createResponseMessage(requestResponse, message, deleteQuestionResponseContainer);
};

const RemoveQuestionAjax = (questionId) => {
    isSelfUpdated = true;

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

document.addEventListener('DOMContentLoaded', () => {
    deleteQuestionResponseContainer = document.querySelector('.questions__delete-response-container');
    AddRemoveEventListeners();
});
