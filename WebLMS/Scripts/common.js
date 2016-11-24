$(document).ready(function () {
    var MENU_CLASS = 'header__menu';
    var MENU_DISPLAY_CLASS = MENU_CLASS + '--display';

    $(document).click(function (e) {
        var menuElem = document.getElementsByClassName(MENU_CLASS)[0];

        if ($(menuElem).hasClass(MENU_DISPLAY_CLASS) && !e.target.contains(menuElem) && !$(e.target).hasClass('header__mobile-menu-icon')) {
            $(menuElem).removeClass(MENU_DISPLAY_CLASS);
        }
    });

    $('#convertForm').submit(function (e) {
        e.preventDefault();
        $(".video-converter__overlay-loading").addClass("video-converter__overlay-loading--show");

        var options = {
            timeout: 6000000,
            success: function (data) {
                if (data.error) {
                    alert(data.error);
                }
                else if (data.status && data.status === 'process' && data.message) {
                    alert(data.message);
                }
                $(".video-converter__overlay-loading").removeClass("video-converter__overlay-loading--show");
            },
            error: function (err1, err2) {
                $(".video-converter__overlay-loading").removeClass("video-converter__overlay-loading--show");
                alert(err2);
            }
        }
        $(this).ajaxSubmit(options);
    })

    $(".video-converter__input-file").change(function (e) {
        var val = this.value.split('\\').pop().split('/').pop();
        $(".video-converter__filename").text(val);
    })
})