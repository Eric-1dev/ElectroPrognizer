/*jshint esversion: 6 */

$(document).ready(() => {
    prognizerHelper.init();

    prognizerHelper.getTableContent();
});

let prognizerHelper = {
    init: () => {
        prognizerHelper._getTableContentUrl = $('#prognizer-date-selector').attr('table-content-url');

        $('.prognizer-date-control').change(prognizerHelper._getTableContent);
    },

    getTableContent: () => {
        prognizerHelper._resultTable.html('');

        let month = $('#prognizer-month-selector').val();
        let year = $('#prognizer-year-selector').val();

        $.ajax({
            url: prognizerHelper._getTableContentUrl,
            type: 'POST',
            data: { month, year },
            success: (data) => {
                if (data.isFail) {
                    prognizerHelper._resultTable.html(data.message);
                    return;
                }

                prognizerHelper._drawTable(data.entity);
            }
        }).fail(() => {
            modalHelper.showMessage('Произошла неизвестная ошибка');
        });
    },

    _getTableContentUrl: '',
    _resultTable: $('#prognizer-result-table'),

    _drawTable: (tableData) => {
        prognizerHelper._resultTable.html(tableData);
    }
};