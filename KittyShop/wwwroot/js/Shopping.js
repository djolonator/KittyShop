function addToCart(productId) {
    $.ajax({
        url: '/Shop/AddToCart',
        type: 'POST',
        data: { productId: productId },
        success: function (response) {

            if (response.success) 
                showToasterShopPage(response.message, response.success);
            else 
                showToasterShopPage(response.message);
        }
    });
}

function showToasterShopPage(message, error = false) {

    let notyf = new Notyf();

    if (!error)
        notyf.error(message);
    else
        notyf.success(message);
}