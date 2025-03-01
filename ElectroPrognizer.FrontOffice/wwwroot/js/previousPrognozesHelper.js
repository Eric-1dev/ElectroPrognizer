/*jshint esversion: 6 */

$(document).ready(() => {
    previousPrognozesHelper.init();
});

let previousPrognozesHelper = {
    init: () => {
        previousPrognozesHelper._loadDataUrl = $('#prognizer-input-params').attr('load-data-url');

        $('.prognizer-param').change(() => {
            previousPrognozesHelper._loadData();
        });

        previousPrognozesHelper._loadData();
    },

    _loadDataUrl: '',
    _resultBlock: $('#prognizer-prev-data-result'),

    _loadData: () => {
        previousPrognozesHelper._resultBlock.html('');

        const substationId = $('#prognizer-substation-picker').val();
        const month = $('#prognizer-month-picker').val();
        const year = $('#prognizer-year-picker').val();

        $.ajax({
            url: previousPrognozesHelper._loadDataUrl,
            type: 'POST',
            data: { substationId: substationId, month: month, year: year },
            success: (data) => {
                if (data.isSuccess) {
                    previousPrognozesHelper._fillResult(data.entity);
                } else {
                    modalWindowHelper.showError(data.message);
                }
            }
        });
    },

    _fillResult: (data) => {
        const table = $('<table class="table table-bordered table-striped table-hover">');

        data.days.forEach((day) => {
            const date = day.date;

            day.hours.forEach((hour) => {
                const row = $('<tr>');

                const realDataCell = $('<td>');
                realDataCell.html(hour.realValue);

                const prognozedDataCell = $('<td>');
                prognozedDataCell.html(hour.prognozedValue);

                const percentCell = $('<td>');
                percentCell.html(hour.errorPercent);

                row.append(realDataCell);
                row.append(prognozedDataCell);
                row.append(percentCell);
                debugger
                table.append(row);
            });
        });

        previousPrognozesHelper._resultBlock.html(table);
    }
};
