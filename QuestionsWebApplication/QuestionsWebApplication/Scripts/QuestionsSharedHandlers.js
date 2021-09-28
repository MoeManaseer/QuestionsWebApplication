const getCellValue = (tr, idx) => tr.children[idx].innerText || tr.children[idx].textContent;

const comparer = (idx, asc) => (a, b) => ((v1, v2) =>
    v1 !== '' && v2 !== '' && !isNaN(v1) && !isNaN(v2) ? v1 - v2 : v1.toString().localeCompare(v2)
)(getCellValue(asc ? a : b, idx), getCellValue(asc ? b : a, idx));

const InitSortingFunctionality = () => {
    document.querySelectorAll('.sortable').forEach(th => th.addEventListener('click', (() => {
        const table = th.closest('table');
        const tbody = table.querySelectorAll('tbody')[1];
        Array.from(tbody.querySelectorAll('tr'))
            .sort(comparer(Array.from(th.parentNode.children).indexOf(th), this.asc = !this.asc))
            .forEach(tr => tbody.appendChild(tr));
    })));
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
};

document.addEventListener('DOMContentLoaded', () => {
    deleteQuestionResponseContainer = document.querySelector('.questions__delete-response-container');
});

let isSelfUpdated = false;