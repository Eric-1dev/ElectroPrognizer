/*jshint esversion: 6 */

$(document).ready(() => {
    downloadLogHelper.init();
});

let downloadLogHelper = {
    init: () => {
        let form = $('#prognizer-download-log-form');

        downloadLogHelper._filterForm = form;

        downloadLogHelper._loadDataUrl = form.attr('prognizer-load-data-url');

        downloadLogHelper._resultTableBody = $('#prognizer-log-table').find('tbody');

        form.submit(downloadLogHelper._onFilterSubmit);

        downloadLogHelper._loadData(1, null, null);
    },

    _loadDataUrl: '',
    _resultTableBody: {},
    _filterForm: {},

    _loadData: () => {
        if (!downloadLogHelper._filterForm.valid()) {
            return;
        }

        let formData = new FormData(downloadLogHelper._filterForm[0]);

        $.ajax({
            url: downloadLogHelper._loadDataUrl,
            type: 'POST',
            data: formData,
            success: (data) => {
                if (data.isSuccess) {
                    downloadLogHelper._handleResult(data.entity);
                } else {
                    modalWindowHelper.showError(data.message);
                }
            },
            processData: false,
            contentType: false
        });
    },

    _handleResult: (result) => {
        downloadLogHelper._resultTableBody.html('');

        result.entities.forEach((log) => {
            let tr = $('<tr>')

            switch (log.logLevel) {
                case 1:
                    tr.addClass('alert alert-success');
                    break;
                case 2:
                    tr.addClass('alert alert-warning');
                    break;
                case 3:
                    tr.addClass('alert alert-danger');
                    break;
                default:
                    break;
            }

            let created = new Date(log.created);
            let dateTd = $('<td class="text-center">');
            dateTd.html(created.toLocaleString('ru-RU'));

            let levelTd = $('<td class="text-center">');
            levelTd.html(downloadLogHelper._parseLogLevel(log.logLevel));

            let messageTd = $('<td>');
            messageTd.html(log.message);

            tr.append(dateTd);
            tr.append(levelTd);
            tr.append(messageTd);

            downloadLogHelper._resultTableBody.append(tr);
        });
    },

    _parseLogLevel: (logLevel) => {
        switch (logLevel) {
            case 1:
                return 'Информация';
            case 2:
                return 'Предупреждение';
            case 3:
                return 'Ошибка';
        }
    },

    _onFilterSubmit: (event) => {
        event.preventDefault();

        downloadLogHelper._loadData();
    }
};
