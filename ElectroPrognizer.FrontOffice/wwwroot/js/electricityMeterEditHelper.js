/*jshint esversion: 6 */

$(document).ready(() => {
    electricityMeterEditHelper.init();
});

let electricityMeterEditHelper = {
    init: () => {
        electricityMeterEditHelper._saveForm = $('#prognizer-electricity-meter-edit-form');
        electricityMeterEditHelper._saveUrl = electricityMeterEditHelper._saveForm.attr('electricity-meter-save-url');

        electricityMeterEditHelper._saveForm.submit((event) => {
            electricityMeterEditHelper._save(event);
        });
    },

    _saveUrl: '',
    _saveForm: '',

    _save: (event) => {

        event.preventDefault();

        let formData = new FormData(event.target);

        $.ajax({
            url: electricityMeterEditHelper._saveUrl,
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
