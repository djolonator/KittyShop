

function addToCart(productId) {

    callShopAddToCart(productId);
}

function callShopAddToCart(productId) {
    $.ajax({
        url: '/Shop/AddToCart',
        type: 'POST',
        data: { productId: productId },
        success: function (response) {

            if (response.success) {
                handleResponse(response.message, response.success);
            }
            else
                showToasterShopPage(response.message);
        }
    });
}

function handleResponse(message = '', isSuccess = false) {

    showToasterShopPage(message, isSuccess);
    updateItemNumberCookie();
    setCartBadgeValue();
}

function showToasterShopPage(message, error = false) {

    let notyf = new Notyf();

    if (!error)
        notyf.error(message);
    else
        notyf.success(message);
}

