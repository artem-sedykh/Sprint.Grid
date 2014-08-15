!function ($) {
    $(function () {
        var $window = $(window);
        // Запускаем code pretty:
        window.prettyPrint && prettyPrint();
    });
}(window.jQuery);



$(document).ready(function () {
    $('#customer').sprintgrid({
        expandHierarchySuccess: function ($container) {
            var $orderGrid = $container.find('.sprint-grid');

            $orderGrid.sprintgrid();

            console.log($orderGrid);
        }
    });

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