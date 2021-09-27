let tSettingsUserPasswordContainer;

const toggleSettingsUserPasswordContainer = (value = 'Other') => {
    if (tSettingsUserPasswordContainer) {
        if (value == 'SSPI') {
            tSettingsUserPasswordContainer.classList.add('settings__user-password-container--disabled');
        } else {
            tSettingsUserPasswordContainer.classList.remove('settings__user-password-container--disabled');
        }

        [...tSettingsUserPasswordContainer.children].forEach((inputBoxContainer) => {
            const inputBox = inputBoxContainer.querySelector('input');

            if (value == 'SSPI') {
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
        toggleSettingsUserPasswordContainer(tSettingsSecuirtyDropDown.options[tSettingsSecuirtyDropDown.value].text);

        tSettingsSecuirtyDropDown.addEventListener('change', () => {
            toggleSettingsUserPasswordContainer(tSettingsSecuirtyDropDown.options[tSettingsSecuirtyDropDown.value].text);
        });
    }
});