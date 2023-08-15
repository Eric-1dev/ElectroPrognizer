/*
    params: {
          title - (string) заголовок окна. Default: 'Сообщение'
          width - (int) ширина окна. Default: 600
          maxHeight - (int) максимальная высота окна.Default: 700
          showCloseButton - (bool) - флаг, определяющий, надо ли отображать стандартную кнопку "Закрыть". Default: true
          closeButtonName - (string) - название кнопки, закрывающей окно. Default: 'Закрыть'
          closeCallback - (function(target)) - вызывается при нажатии стандартной кнопки закрыть. В агумент передается JQ-объект кнопки. Default: window.close(). Если вернет true - окно закроется
          buttons: [ - (array) - массив конопок. Располагаются в порядке указания. Default: empty
              {
                  name - (string) название кнопки. Обязательно для заполнения
                  callback - (function(target)) - вызывается при нажатии кнопки. В агумент передается JQ-объект кнопки. Если вернет true - окно закроется. Обязательно для заполнения
                  isSubmit - (bool) - признак нажмется ли кнопка при нажатии Enter. Может быть только одна. Default: false
              }
          ]
     }
*/

/*jshint esversion: 6 */

$(document).ready(() => {
    modalWindowHelper.init();
});

let modalWindowHelper = {
    init: () => {
        $('body').on('keyup', (event) => {
            if (event.which === 13) { // Enter
                let activeBlocker = modalWindowHelper._findActiveWindow();

                if (activeBlocker) {
                    let submitBtn = $(activeBlocker).find('button[type="submit"]');
                    submitBtn.click();
                }
            } else if (event.which === 27) { // Escape
                let activeBlocker = modalWindowHelper._findActiveWindow();

                if (activeBlocker) {
                    modalWindowHelper.close(activeBlocker);
                }
            }
        });
    },

    show: (message, params) => {
        // чтение параметров
        if (!params) {
            params = {};
        }

        let title = params.title ? params.title : 'Сообщение';
        let width = params.width ? params.width : 600;
        let minHeight = params.minHeight;

        let showCloseButton = params.showCloseButton ? params.showCloseButton : true;

        let closeButtonName = params.closeButtonName ? params.closeButtonName : 'Закрыть';

        let closeCallback = params.closeCallback ? params.closeCallback : () => { return true; };

        let buttons = params.buttons ? params.buttons : [];

        // создание блокера
        let blockerBlock = $('<div>', {
            class: 'custom-modal-window-blocker'
        });

        let zIndex = modalWindowHelper._calcZIndex();

        blockerBlock.css('zIndex', zIndex);
        blockerBlock.css('display', 'none');

        // создание блока окна
        let windowBlock = $('<div>', {
            class: 'custom-modal-window'
        });

        let windowContainer = $('<div>', {
            class: 'custom-modal-window-container'
        });

        windowContainer.css('width', width);
        if (minHeight) {
            windowContainer.css('min-height', minHeight);
        }

        // создание блока заголовка
        let titleBlock = $('<div>', {
            class: 'custom-modal-window-title'
        });

        titleBlock.html(title);

        // создание блока тела
        let bodyBlock = $('<div>', {
            class: 'custom-modal-window-body'
        });

        bodyBlock.html(message);

        // создание блока кнопок
        let buttonsBlock = $('<div>', {
            class: 'custom-modal-window-buttons-block'
        });

        let hasSubmitButton = false;

        buttons.forEach((button) => {
            let buttonType;

            if (button.isSubmit && !hasSubmitButton) {
                buttonType = 'submit';
                hasSubmitButton = true;
            } else {
                buttonType = 'button';
            }

            let but = $('<button>', {
                class: 'custom-modal-window-button btn btn-primary',
                type: buttonType
            });
            but.html(button.name);
            but.click((event) => {
                event.preventDefault();

                let callbackResult = button.callback(event);
                if (callbackResult) {
                    modalWindowHelper.close(but);
                }
            });

            buttonsBlock.append(but);
        });

        if (showCloseButton) {
            let closeButton = $('<button>', {
                class: 'custom-modal-window-button btn btn-secondary',
                type: 'button'
            });

            closeButton.html(closeButtonName);
            closeButton.click((event) => {
                let callbackResult = closeCallback(event.target);
                if (callbackResult) {
                    modalWindowHelper.close(closeButton);
                }
            });

            buttonsBlock.append(closeButton);
        }

        // сборка окна
        blockerBlock.append(windowBlock);
        windowBlock.append(windowContainer);
        windowContainer.append(titleBlock);
        windowContainer.append(bodyBlock);
        windowContainer.append(buttonsBlock);

        $('body').append(blockerBlock);

        blockerBlock.fadeIn(200);

        return windowBlock;
    },

    close: (button) => {
        let windowBlock = modalWindowHelper._getWindowByEventTarget(button);
        windowBlock.fadeOut(200, () => { windowBlock.remove(); });
    },

    showError: (message) => {
        let windowParams = {
            title: 'Ошибка',
            showCloseButton: true,
            isSubmit: true
        };

        modalWindowHelper.show(message, windowParams);
    },

    showInfo: (message) => {
        let windowParams = {
            title: 'Информация',
            showCloseButton: true,
            isSubmit: true
        };

        modalWindowHelper.show(message, windowParams);
    },

    showConfirmDialog: (message, confirmCallback) => {
        let windowParams = {
            title: 'Подтверждение действия',
            showCloseButton: true,
            buttons: [
                {
                    name: 'Подтвердить',
                    callback: confirmCallback,
                    isSubmit: true
                }
            ]
        };

        modalWindowHelper.show(message, windowParams);
    },

    _startIndex: 10000,

    _getWindowByEventTarget: (target) => {
        return $(target).closest('.custom-modal-window-blocker');
    },

    _findActiveWindow: () => {
        let maxZIndex = modalWindowHelper._getMaxZIndex();

        if (!maxZIndex) {
            return null;
        }

        let activeBlock = $('.custom-modal-window-blocker').filter((_, elem) => {
            return $(elem).css('zIndex') == maxZIndex;
        });

        return activeBlock;
    },

    _getMaxZIndex: () => {
        let elements = $('.custom-modal-window-blocker');

        if (elements.length === 0) {
            return null;
        }

        return Math.max.apply(null, $.map(elements, (elem) => {
            return parseInt($(elem).css('zIndex')) || 1;
        }));
    },

    _calcZIndex: () => {
        let maxZIndex = modalWindowHelper._getMaxZIndex();

        if (!maxZIndex) {
            return modalWindowHelper._startIndex;
        }

        return maxZIndex + 1;
    }
};
