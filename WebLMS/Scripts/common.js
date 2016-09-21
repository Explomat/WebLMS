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
        $(this).ajaxSubmit(function (data) {
            $(".video-converter__overlay-loading").removeClass("video-converter__overlay-loading--show");
            if (data.error) {
                alert(data.error);
            }
            else if (data.filePath) {
                var a = document.getElementsByClassName("video-converter__download-video")[0];
                a.setAttribute('href', data.filePath);
                a.click();
            }
        })
    })

    $(".video-converter__input-file").change(function (e) {
        var val = this.value.split('\\').pop().split('/').pop();
        $(".video-converter__filename").text(val);
    })
})