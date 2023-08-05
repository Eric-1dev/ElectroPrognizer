/*jshint esversion: 6 */

$(document).ready(() => {
    modalHelper.init();
});
// todo переписать на нормальные модалки
let modalHelper = {
    init: () => {

    },

    showMessage: (message, params) => {
        alert(message);
    },

    showConfirm: (message, successCallback, params) => {
        if (confirm(message) === true) {
            successCallback();
        }
    }
};
