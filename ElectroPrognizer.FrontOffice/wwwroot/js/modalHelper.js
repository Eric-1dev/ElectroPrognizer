/*jshint esversion: 6 */

$(document).ready(() => {
    modalHelper.init();
});

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