/*jshint esversion: 6 */

$(document).ready(() => {
    substationEditHelper.init();
});

let substationEditHelper = {
    init: () => {
        substationEditHelper._saveForm = $('#prognizer-substation-edit-form');
        substationEditHelper._saveUrl = substationEditHelper._saveForm.attr('substation-save-url');

        substationEditHelper._saveForm.submit((event) => {
            substationEditHelper._save(event);
        });
    },

    _saveUrl: '',
    _saveForm: '',

    _save: (event) => {

        event.preventDefault();

        let formData = new FormData(event.target);

        $.ajax({
            url: substationEditHelper._saveUrl,
            type: 'POST',
            data: formData,
            success: (data) => {
                modalHelper.showMessage(data.message);
            },
            processData: false,
            contentType: false
        });
    }
};
