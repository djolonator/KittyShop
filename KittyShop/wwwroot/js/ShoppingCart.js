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

function updateQuantity(productId, thisButton) {
    let cartId = $("#cartId").val();
    let quantity = $(thisButton).siblings(".quantitySelect").val();

    updateCart(cartId, productId, quantity);
}

function updateCart(cartId, productId, quantity) {
    $.ajax({
        url: '/Shop/UpdateCart',
        type: 'POST',
        data: { cartId: cartId, productId: productId, quantity: quantity },
        success: function (response) {
            if (response.success) {
                // Update the cart view or show a success message
            } else {
                // Show an error message
            }
        }
    });
}