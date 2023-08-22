/*jshint esversion: 6 */

$(document).ready(() => {
    downloadLogHelper.init();
});

let downloadLogHelper = {
    init: () => {
        downloadLogHelper._saveForm = $('#prognizer-edit-form');
        downloadLogHelper._saveUrl = downloadLogHelper._saveForm.attr('prognizer-save-url');

        downloadLogHelper._saveForm.submit((event) => {
            downloadLogHelper._save(event);
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
            url: downloadLogHelper._saveUrl,
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
