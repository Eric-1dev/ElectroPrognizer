$(document).ready(() => {
    progressBarHelper.init();
});

let progressBarHelper = {
    init: () => {
        progressBarHelper._getStatusUrl = progressBarHelper._progressBarForm.attr('get-status-url');
        progressBarHelper._cancelUploadUrl = progressBarHelper._progressBarForm.attr('cancel-upload-url');

        progressBarHelper._updateStatus();

        $('#progress-bar-cancel-button').click(() => {

        });
    },

    _getStatusUrl: '',
    _cancelUploadUrl: '',
    _progressBarFormWrapper: $('#progress-bar-form-wrapper'),
    _progressBarForm: $('#progress-bar-form'),

    _updateStatus: () => {
        $.ajax({
            url: progressBarHelper._getStatusUrl,
            type: 'POST',
            success: (data) => {
                if (data.isComplete) {
                    progressBarHelper._hideProgressBar()
                } else {
                    progressBarHelper._setPercents(data.percents);
                }
            }
        });
    },

    _hideProgressBar: () => {
        progressBarHelper._progressBarFormWrapper.hide();
    },

    _setPercents: (percents) => {
        progressBarHelper._progressBarFormWrapper.show();
        $('#prognizer-progress-bar > .progress-bar').css("width", percents + '%');
    },
};