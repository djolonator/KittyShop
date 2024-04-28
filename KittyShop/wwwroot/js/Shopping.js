

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
                showToasterShopPage(response.message, response.success);
                /*hideProductDivFromList(productId);*/
            }
            else
                showToasterShopPage(response.message);
        }
    });
}

function hideProductDivFromList(productId) {

    let div = $("#" + productId);
    $(div).attr('style', 'display: none !important');
}


function showToasterShopPage(message, error = false) {

    let notyf = new Notyf();

    if (!error)
        notyf.error(message);
    else
        notyf.success(message);
}