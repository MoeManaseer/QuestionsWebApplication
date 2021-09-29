let testConnectionButton;
let testConnectionText;
let testConnectionMessageContainer;

const testConnection = () => {
    testConnectionText.classList.remove('connection-test__text--hidden');
    $.ajax({
        url: '/Settings/TestConnection',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        type: 'POST',
        dataType: 'json',
        data: $('form').serialize(),
        success: (result) => {
            createResponseMessage(result.requestResponse, result.message, testConnectionMessageContainer)
            testConnectionText.classList.add('connection-test__text--hidden');

        },
        error: (error) => {
            console.log(error);
            testConnectionText.classList.add('connection-test__text--hidden');
        },
    });
};

document.addEventListener('DOMContentLoaded', () => {
    testConnectionButton = document.querySelector('.connection-test__button');
    testConnectionMessageContainer = document.querySelector('.connection-test__container');
    testConnectionText = document.querySelector('.connection-test__text');

    if (testConnectionButton) {
        testConnectionButton.addEventListener('click', testConnection);
    }
});
