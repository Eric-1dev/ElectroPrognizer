/*jshint esversion: 6 */

$(document).ready(() => {
    prognizerHelper.init();

    //prognizerHelper.getTableContent();
});

let prognizerHelper = {
    init: () => {
        prognizerHelper._getTableContentUrl = $('#prognizer-date-selector').attr('table-content-url');

        prognizerHelper._resultTable = $('#prognizer-result-table');

        $('.prognizer-date-control').change(prognizerHelper.getTableContent);

        $('#prognizer-button-calculate').click(prognizerHelper.getTableContent);
    },

    getTableContent: () => {
        prognizerHelper._resultTable.html('');

        let substationId = $('#prognizer-substation-selector').val();
        let calculationDate = $('#prognizer-date-picker').val();

        $.ajax({
            url: prognizerHelper._getTableContentUrl,
            type: 'POST',
            data: { substationId: substationId, calculationDate: calculationDate },
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

        // заголовок

        let tHead = $('<thead>');

        let dayCount = tableData.dayDatas.length;

        let tr = $('<tr>');

        tr.append($('<th>'));

        for (let i = 0; i < dayCount; i++) {
            let th = $('<th>');

            th.html(prognizerHelper._formatDate(tableData.dayDatas[i].date));
            tr.append(th);
        }

        tHead.append(tr);
        table.append(tHead);

        // тело

        let tBody = $('<tbody>');

        for (var hourCounter = 0; hourCounter < 26; hourCounter++) {
            let tr = $('<tr>');

            let td = $('<td>');

            if (hourCounter === 24) {
                td.addClass('fw-bold');
                td.html('Итого за день:')
            } else if (hourCounter === 25) {
                td.addClass('fw-bold');
                td.html('Нарастающий итог:')
            } else {
                td.html(('0' + hourCounter).slice(-2) + ':00');
            }

            tr.append(td);

            for (var dayCounter = 0; dayCounter < dayCount; dayCounter++) {
                let td = $('<td>');

                if (hourCounter === 24) {
                    td.addClass('fw-bold');
                    td.html(prognizerHelper._valueToMegaWatt(tableData.dayDatas[dayCounter].total));
                } else if (hourCounter === 25) {
                    td.addClass('fw-bold');
                    td.html(prognizerHelper._valueToMegaWatt(tableData.dayDatas[dayCounter].cumulativeTotal));
                } else {
                    let value = tableData.dayDatas[dayCounter].hourDatas[hourCounter].value;

                    if (value == null) {
                        value = '-';
                    } else {
                        value = prognizerHelper._valueToMegaWatt(value);
                        if (tableData.dayDatas[dayCounter].isRealData) {
                            td.addClass('prognizer-real-data')
                        } else {
                            td.addClass('prognizer-prognozed-data')
                        }
                    }

                    td.html(value);
                }

                tr.append(td);
            }

            tBody.append(tr);
        }

        table.append(tBody);

        prognizerHelper._resultTable.append(table);
    },

    _formatDate: (dateInput) => {

        let date = new Date(dateInput);

        let day = date.getDate();
        let month = date.getMonth() + 1;
        let year = date.getFullYear();

        if (day < 10) {
            day = '0' + day;
        }

        if (month < 10) {
            month = `0${month}`;
        }

        let formatDate = `${day}.${month}.${year}`;

        return formatDate;
    },

    _valueToMegaWatt: (value) => {
        return (value / 1000).toFixed(3);
    }
};
