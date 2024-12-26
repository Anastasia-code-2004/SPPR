$(document).ready(function () {
    // Проверка, что мы находимся на странице каталога
    if (window.location.href.includes("/Catalog")) {

        // Обработка кликов по ссылкам пейджера только на странице каталога
        $(document).on('click', '.page-link', function (e) {
            var a = window.location.href;
            e.preventDefault(); // Предотвращаем переход по ссылке

            var url = $(this).attr('href'); // Получаем URL, указанный в ссылке

            // Выполняем асинхронный запрос
            $.ajax({
                url: url,
                type: 'GET',
                success: function (result) {
                    // Обновляем контейнер с товарами и пейджером
                    $('#productList').html(result);
                },
                error: function (xhr, status, error) {
                    console.log("Произошла ошибка при выполнении Ajax-запроса: " + error);
                }
            });
        });
    }
});