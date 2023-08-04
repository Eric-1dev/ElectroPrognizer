/*jshint esversion: 6 */

$(document).ready(() => {
    prognizerHelper.init();

    prognizerHelper.getTableContent();
});

let prognizerHelper = {
    init: () => {
        prognizerHelper._getTableContentUrl = $('#prognizer-date-selector').attr('table-content-url');

        prognizerHelper._resultTable = $('#prognizer-result-table');

        $('.prognizer-date-control').change(prognizerHelper.getTableContent);
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
    _resultTable: {},

    _drawTable: (tableData) => {
        prognizerHelper._resultTable.html('');

        // строим таблицу

        let table = $('<table>');
        table.addClass('prognizer-table table table-bordered table-striped table-hover');

        let tHead = $('<thead>');

        let dayCount = tableData.dayDatas.length;

        let tr = $('<tr>');

        tr.append($('<th>'));

        for (let dayNumber = 1; dayNumber < dayCount + 1; dayNumber++) {
            let th = $('<th>');

            th.html(prognizerHelper._formarDate(dayNumber, tableData.month, tableData.year));
            tr.append(th);
        }

        let nextDayHeader = $('<th>');
        nextDayHeader.html(prognizerHelper._formarDate(dayCount + 1, tableData.month, tableData.year));

        let twoDayHeader = $('<th>');
        twoDayHeader.html(prognizerHelper._formarDate(dayCount + 2, tableData.month, tableData.year));

        tr.append(nextDayHeader);
        tr.append(twoDayHeader);

        tHead.append(tr);
        table.append(tHead);

        let tBody = $('<tbody>');

        for (let hourNumber = 0; hourNumber < 24; hourNumber++) {
            let tr = $('<tr>');

            let tdHourNumber = $('<td>');
            tdHourNumber.html(('0' + hourNumber).slice(-2) + ':00');
            tr.append(tdHourNumber);

            for (let dayNumber = 1; dayNumber < dayCount + 1; dayNumber++) {
                let td = $('<td>');
                td.attr('hour', hourNumber);
                td.attr('day', dayNumber);

                tr.append(td);
            }

            tBody.append(tr);
            table.append(tBody);
        }

        prognizerHelper._resultTable.append(table);

        // заполняем данные

        let dataArray = Object(tableData.dayDatas);

        dataArray.forEach((day) => {
            let hourArray = Object(day.hourDatas);

            hourArray.forEach((hourData) => {
                $('td[hour="' + hourData.hour + '"][day="' + day.dayNumber + '"]').html(hourData.value);
            });
        });
    },

    _formarDate: (day, month, year) => {
        let dayFormat = ('0' + day).slice(-2);
        let monthFormat = ('0' + month).slice(-2);

        return `${dayFormat}.${monthFormat}.${year}`;
    }
};
