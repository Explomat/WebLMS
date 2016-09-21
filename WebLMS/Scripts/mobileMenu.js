$(document).ready(function () {
    var button = $('.header__mobile-menu');
    var menu = $('.header__menu');
    var menuList = $('.header__menu-list');
    var menuItem = $('.header__menu-list-item');

    button.click(function () {
        try {
            var headerWidth = $('.header__bg-overlay').height();
            menuList.css({ 'top': headerWidth + 'px' });
            menu.toggleClass('header__menu--display');
        }
        catch (e) { alert(e) }
    });
    menuItem.click(function () {
        menu.removeClass('header__menu--display');
    });
});