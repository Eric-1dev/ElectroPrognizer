/*jshint esversion: 6 */

$(document).ready(() => {
    uploadHelper.init();
});

let uploadHelper = {
    init: () => {

        uploadHelper._fileUploadUrl = $('#file-upload-form').attr('upload-file-action');
        uploadHelper._uploadFromMailUrl = $('#file-upload-form').attr('upload-from-mail-action');

        $('#file-upload-button').click(() => {
            let input = $('#file-upload-input')[0];

            let formData = new FormData();

            let files = input.files;

            if (files.length === 0) {
                return;
            }

            $(files).each((_, file) => {
                formData.append('file', file, file.name);
            });

            let needOverride = $('#override-existing').is(':checked');
            formData.append('overrideExisting', needOverride);

            $.ajax({
                url: uploadHelper._fileUploadUrl,
                type: 'POST',
                data: formData,
                success: (data) => {
                    if (data.isFail) {
                        modalWindowHelper.showError('Ошибка при загрузке файлов: ' + data.message);
                        return;
                    }
                },
                cache: false,
                contentType: false,
                processData: false
            });
        });

        $('#upload-from-mail-button').click(() => {
            $.ajax({
                url: uploadHelper._uploadFromMailUrl,
                type: 'POST',
                success: (data) => {
                    if (data.isSuccess) {
                        modalWindowHelper.showInfo('Запущена загрузка файлов из почты');
                        return;
                    }
                }
            });
        });
    },

    _fileUploadUrl: '',
    _uploadFromMailUrl: ''
};
