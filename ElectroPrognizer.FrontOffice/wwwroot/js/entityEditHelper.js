/*jshint esversion: 6 */

$(document).ready(() => {
    entityEditHelper.init();
});

let entityEditHelper = {
    init: () => {
        entityEditHelper._saveForm = $('#prognizer-edit-form');
        entityEditHelper._saveUrl = entityEditHelper._saveForm.attr('prognizer-save-url');

        entityEditHelper._saveForm.submit((event) => {
            entityEditHelper._save(event);
        });
    },

    _saveUrl: '',
    _saveForm: '',

    _save: (event) => {
        event.preventDefault();

        let form = $(event.target);

        if (!form.valid()) {
            return;
        }

        let formData = new FormData(form[0]);

        $.ajax({
            url: entityEditHelper._saveUrl,
            type: 'POST',
            data: formData,
            success: (data) => {
                if (data.isSuccess) {
                    modalWindowHelper.showInfo(data.message);
                } else {
                    modalWindowHelper.showError(data.message);
                }
            },
            processData: false,
            contentType: false
        });
    }
};
