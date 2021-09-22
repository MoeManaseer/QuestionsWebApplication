let tQuestionExtraDataContainer;
let tModelTypeField;

const SelectExtraQuestionContainer = (type = "Smiley") => {
    if (tQuestionExtraDataContainer) {
        [...tQuestionExtraDataContainer.childNodes].forEach((tDataContainer) => {
            if (tDataContainer && tDataContainer.nodeType != Node.TEXT_NODE) {
                if (tDataContainer.classList.contains(type + '__container')) {
                    tDataContainer.style.display = 'block';
                }
                else {
                    tDataContainer.style.display = 'none';
                }
            }
        });

        tModelTypeField.value = `${type}Question`;
    }
};

document.addEventListener("DOMContentLoaded", () => {
    const tQuestionTypeMenu = document.querySelector('.question__type-dropdown');
    tQuestionExtraDataContainer = document.querySelector('.question__extra-data-container');
    tModelTypeField = document.querySelector('#ModelTypeName');

    SelectExtraQuestionContainer();

    if (tQuestionTypeMenu) {
        tQuestionTypeMenu.disabled = tQuestionTypeMenu.classList.contains('question__type-dropdown--disabled') ? true : false;

        tQuestionTypeMenu.addEventListener('change', (event) => {
            const tCurrentType = tQuestionTypeMenu.options[tQuestionTypeMenu.selectedIndex].text;
            SelectExtraQuestionContainer(tCurrentType);
        });
    }
});
