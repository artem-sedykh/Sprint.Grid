!function ($) {
    $(function () {
        var $window = $(window);
        // Запускаем code pretty:
        window.prettyPrint && prettyPrint();
    });
}(window.jQuery);
$(document).ready(function () {
    $('.sprint-grid').sprintgrid();
});