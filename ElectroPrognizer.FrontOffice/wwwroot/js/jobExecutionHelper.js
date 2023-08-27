/*jshint esversion: 6 */

$(document).ready(() => {
    jobExecutionHelper.init();
});

let jobExecutionHelper = {
    init: () => {
        jobExecutionHelper._executeForm = $('#prognizer-job-execution-form');
        jobExecutionHelper._executeUrl = jobExecutionHelper._executeForm.attr('prognizer-job-execution-url');

        $('.prognizer-job-execute-button').click((event) => {
            let button = $(event.target);

            let jobName = button.attr('prognizer-job-name');

            modalWindowHelper.showConfirmDialog('Подтвердите запуск задачи', () => { jobExecutionHelper._executeJob(jobName); return true; });
        });
    },

    _executeUrl: '',

    _executeJob: (jobName) => {
        $.ajax({
            url: jobExecutionHelper._executeUrl,
            type: 'POST',
            data: { jobName: jobName },
            success: (data) => {
                if (data.isSuccess) {
                    modalWindowHelper.showInfo(data.message);
                } else {
                    modalWindowHelper.showError(data.message);
                }
            }
        });
    }
};
