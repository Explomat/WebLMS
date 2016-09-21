$(document).ready(function () {
    resetForm();
    prepareCheckbox();

    var INCORRECT_CLASS = 'form-modal__input-value--incorrect';
    var INPUT_ClASSES = {
        fullname: 'form-modal__fullname-value',
        email: 'form-modal__email-value',
        phone: 'form-modal__phone-value',
        description: 'form-modal__description-value'
    }

    function resetForm() {
        $('#header__formModal').click(function () {
            $('.form-modal__input-value').removeClass('form-modal__input-value--incorrect');
            $('.form-modal__input-value').val('');
        });
    }

    function prepareCheckbox() {
        $('.form-modal__is-quickly-value').change(function () {
            var isChecked = $(this).prop('checked');
            $('.form-modal__is-quickly-value-hidden').val(isChecked);
        })
    }

    function isFullnameCorrect() {
        return $('.' + INPUT_ClASSES.fullname).val() !== '';
    }

    function isEmailCorrect() {
        var email = $('.' + INPUT_ClASSES.email).val();
        return /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/.test(email)
    }

    function isPhoneCorrect() {
        var phone = $('.' + INPUT_ClASSES.phone).val();
        return /^[0-9]{1,}$/.test(phone);
    }

    function isDescriptionCorrect() {
        return $('.' + INPUT_ClASSES.description).val() !== '';
    }

    var submitButton = $('#form-modal__submit');
    submitButton.click(function (e) {
        var isIncorrect = false;

        if (!isFullnameCorrect()) {
            $('.' + INPUT_ClASSES.fullname).addClass(INCORRECT_CLASS);
            isIncorrect = true;
        }
        else {
            $('.' + INPUT_ClASSES.fullname).removeClass(INCORRECT_CLASS);
        }

        if (!isEmailCorrect()) {
            $('.' + INPUT_ClASSES.email).addClass(INCORRECT_CLASS);
            isIncorrect = true;
        }
        else {
            $('.' + INPUT_ClASSES.email).removeClass(INCORRECT_CLASS);
        }

        if (!isPhoneCorrect()) {
            $('.' + INPUT_ClASSES.phone).addClass(INCORRECT_CLASS);
            isIncorrect = true;
        }
        else {
            $('.' + INPUT_ClASSES.phone).removeClass(INCORRECT_CLASS);
        }

        if (!isDescriptionCorrect()) {
            $('.' + INPUT_ClASSES.description).addClass(INCORRECT_CLASS);
            isIncorrect = true;
        }
        else {
            $('.' + INPUT_ClASSES.description).removeClass(INCORRECT_CLASS);
        }

        if (isIncorrect) {
            e.preventDefault();
        }
    });
})