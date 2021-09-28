let tQuestionExtraDataContainer;
let tModelTypeField;

const SelectExtraQuestionContainer = (typeIndex = 0) => {
    if (tQuestionExtraDataContainer) {
        [...tQuestionExtraDataContainer.children].forEach((tDataContainer) => {
            tDataContainer.style.display = 'none';
        });

        const selectedContainer = tQuestionExtraDataContainer.children[typeIndex];
        selectedContainer.style.display = 'block';

        tModelTypeField.value = `${selectedContainer.getAttribute('type')}Question`;
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
            SelectExtraQuestionContainer(tQuestionTypeMenu.selectedIndex);
        });
    }
});
