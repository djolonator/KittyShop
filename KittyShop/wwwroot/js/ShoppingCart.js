$(function () {
    $('.quantitySelect').on('change', function () {
        let selectedValue = $(this).val();
        enableUpdateButton(this);
    });
});


function enableUpdateButton(selectElement) {
    let button = $(selectElement).siblings(".btn");
    $(button).prop("disabled", false);
}

