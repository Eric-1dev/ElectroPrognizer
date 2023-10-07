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

    _fillResult: () => {
        previousPrognozesHelper._resultBlock.html('sdf');
    }
};
