const getUpdatedData = () => {
    $.ajax({
        url: '/Questions/GetUpdatedData',
        contentType: 'application/html; charset=utf-8',
        type: 'POST',
        dataType: 'html',
        success: (result) => {
            const questionsContainer = document.querySelector('#questions__container');

            if (questionsContainer) {
                questionsContainer.outerHTML = result;
                AddRemoveEventListeners();
                InitSortingFunctionality();
            }
        },
        error: (error) => {
            console.log(error);
        },
    });
};

document.addEventListener('DOMContentLoaded', () => {
    // Declare a proxy to reference the hub. 
    const tQuestionsTicker = $.connection.questionTicker;

    // Create a function that the hub can call to notify changes in the data.
    tQuestionsTicker.client.broadcastUpdateData = function () {
        getUpdatedData();
    };

    // Start the connection
    $.connection.hub.start().done();
});