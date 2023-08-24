/*jshint esversion: 6 */

$(document).ready(() => {
    progressBarHelper.init();
});

let progressBarHelper = {
    init: () => {
        progressBarHelper._getStatusUrl = progressBarHelper._progressBarForm.attr('get-status-url');
        progressBarHelper._cancelUploadUrl = progressBarHelper._progressBarForm.attr('cancel-upload-url');


        progressBarHelper._hubConnection = new signalR.HubConnectionBuilder()
            .withUrl('/status')
            .withAutomaticReconnect([10000, 10000, 10000])
            .build();

        progressBarHelper._hubConnection.on('ReceiveStatus', (status) => {
            progressBarHelper._handleStatus(status);
        });

        progressBarHelper._hubConnection.start()
            .then(() => {
                console.log('SignalR connected');
            })
            .catch((error) => {
                return console.error(error.toString());
            });

        $('#progress-bar-cancel-button').click(() => {
            modalWindowHelper.showConfirmDialog('Действительно хотите отменить импорт данных?', () => {
                progressBarHelper._hubConnection.invoke('CancelUpload');
                progressBarHelper._setPercents(0);
                return true;
            });
        });
    },

    _hubConnection: {},
    _progressBarFormWrapper: $('#progress-bar-form-wrapper'),
    _progressBarForm: $('#progress-bar-form'),

    _progressUpdateInterval: 3000,
    _timer: {},

    _handleStatus: (status) => {
        if (status.message) {
            modalWindowHelper.showInfo(status.message);
        }

        if (status.isFinished) {
            progressBarHelper._hideProgressBar();
        } else if (status.percents) {
            progressBarHelper._setPercents(status.percents);
        }
    },

    _hideProgressBar: () => {
        progressBarHelper._progressBarFormWrapper.hide();
    },

    _setPercents: (percents) => {
        progressBarHelper._progressBarFormWrapper.show();
        $('#prognizer-progress-bar > .progress-bar').css("width", percents + '%');
    },
};
