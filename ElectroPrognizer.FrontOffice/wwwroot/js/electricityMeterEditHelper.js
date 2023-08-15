/*jshint esversion: 6 */

$(document).ready(() => {
    applicationSettingEditHelper.init();
});

let applicationSettingEditHelper = {
    init: () => {
        applicationSettingEditHelper._saveForm = $('#prognizer-electricity-meter-edit-form');
        applicationSettingEditHelper._saveUrl = applicationSettingEditHelper._saveForm.attr('electricity-meter-save-url');

        applicationSettingEditHelper._saveForm.submit((event) => {
            applicationSettingEditHelper._save(event);
        });
    },

    _saveUrl: '',
    _saveForm: '',

    _save: (event) => {
        event.preventDefault();

        let formData = new FormData(event.target);

        $.ajax({
            url: applicationSettingEditHelper._saveUrl,
            type: 'POST',
            data: formData,
            success: (data) => {
                modalWindowHelper.showInfo(data.message);
            },
            processData: false,
            contentType: false
        });
    }
};
