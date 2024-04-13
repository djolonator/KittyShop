function addToCart(productId) {
    $.ajax({
        url: '/Shop/AddToCart',
        type: 'POST',
        data: { productId: productId },
        success: function (response) {
            if (response.success) {
                // Update the cart view or show a success message
            } else {
                // Show an error message
            }
        }
    });
}