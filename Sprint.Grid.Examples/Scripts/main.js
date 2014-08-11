!function ($) {
    $(function () {
        var $window = $(window);
        // Запускаем code pretty:
        window.prettyPrint && prettyPrint();
    });
}(window.jQuery);



$(document).ready(function () {
    $('.sprint-grid').sprintgrid();

    $('body').on('click', '.js-refresh-customer', function () {
        $('#customer').sprintgrid('refresh', function(data) {
            alert('refresh callback');
        });
        return false;
    });

    $('body').on('click', '.js-reset-customer', function () {
        
        $('#customer').sprintgrid('reset');
        return false;
    });
});