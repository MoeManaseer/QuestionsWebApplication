let tSettingsUserPasswordContainer;

const toggleSettingsUserPasswordContainer = (valueIndex = 0) => {
    if (tSettingsUserPasswordContainer) {
        if (!valueIndex) {
            tSettingsUserPasswordContainer.classList.add('settings__user-password-container--disabled');
        } else {
            tSettingsUserPasswordContainer.classList.remove('settings__user-password-container--disabled');
        }

        [...tSettingsUserPasswordContainer.children].forEach((inputBoxContainer) => {
            const inputBox = inputBoxContainer.querySelector('input');

            if (!valueIndex) {
                inputBox.setAttribute('disabled', '');
            } else {
                inputBox.removeAttribute('disabled');
            }
        });
    }
};

document.addEventListener('DOMContentLoaded', () => {
    const tSettingsSecuirtyDropDown = document.querySelector('.settings__secuirty-dropdown');
    tSettingsUserPasswordContainer = document.querySelector('.settings__user-password-container');

    if (tSettingsSecuirtyDropDown) {
        toggleSettingsUserPasswordContainer(tSettingsSecuirtyDropDown.selectedIndex);

        tSettingsSecuirtyDropDown.addEventListener('change', () => {
            toggleSettingsUserPasswordContainer(tSettingsSecuirtyDropDown.selectedIndex);
        });
    }
});