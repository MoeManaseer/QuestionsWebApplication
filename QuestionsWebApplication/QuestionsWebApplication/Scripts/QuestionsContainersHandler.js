let tQuestionExtraDataContainer;
let tModelTypeField;
let tQuestionTypeMenu;

const SelectExtraQuestionContainer = (typeIndex = 0, specificType) => {
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
    tQuestionTypeMenu = document.querySelector('.question__type-dropdown');

    if (tQuestionTypeMenu) {
        const isDisabled = tQuestionTypeMenu.classList.contains('question__type-dropdown--disabled') ? true : false;
        tQuestionTypeMenu.disabled = isDisabled;

        if (!isDisabled) {
            tQuestionExtraDataContainer = document.querySelector('.question__extra-data-container');
            tModelTypeField = document.querySelector('#ModelTypeName');

            SelectExtraQuestionContainer(tQuestionTypeMenu.selectedIndex);

            tQuestionTypeMenu.addEventListener('change', (event) => {
                SelectExtraQuestionContainer(tQuestionTypeMenu.selectedIndex);
            });
        }
    }
});
