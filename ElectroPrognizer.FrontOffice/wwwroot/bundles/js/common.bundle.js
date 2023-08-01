$(document).ready(() => {
    progressBarHelper.init();
});

let progressBarHelper = {
    init: () => {
        progressBarHelper._getStatusUrl = progressBarHelper._progressBarForm.attr('get-status-url');
        progressBarHelper._cancelUploadUrl = progressBarHelper._progressBarForm.attr('cancel-upload-url');

        progressBarHelper._updateStatus();

        $('#progress-bar-cancel-button').click(() => {
            progressBarHelper._cancelUpload();
        });
    },

    startMonitoring: () => {
        progressBarHelper._setPercents(0);
        progressBarHelper._updateStatus();
    },

    _getStatusUrl: '',
    _cancelUploadUrl: '',
    _progressBarFormWrapper: $('#progress-bar-form-wrapper'),
    _progressBarForm: $('#progress-bar-form'),

    _progressUpdateInterval: 3000,

    _updateStatus: () => {
        $.ajax({
            url: progressBarHelper._getStatusUrl,
            type: 'POST'
        }).always((data) => {
            if (data.isFinished) {
                progressBarHelper._hideProgressBar();

                if (data.message) {
                    alert(data.message);
                }
            } else {
                if (data.percents) {
                    progressBarHelper._setPercents(data.percents);
                }

                setTimeout(() => progressBarHelper._updateStatus(progressBarHelper._onStatusRecieved), progressBarHelper._progressUpdateInterval);
            };
        });
    },

    _cancelUpload: () => {
        $.ajax({
            url: progressBarHelper._cancelUploadUrl,
            type: 'POST',
            success: () => {
                progressBarHelper._setPercents(0);
            }
        }).fail(() => {
            progressBarHelper._updateStatus();
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