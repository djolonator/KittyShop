$(function () {

    showToasterGlobal();

    if (userIsLoggedIn()) {

        setItemNumberCookie();
        setCartBadgeValue();
    }
});

function showToasterGlobal() {
    let notyf = new Notyf();

    let successMessage = $(".successToaster").text();
    let errorMessage = $(".errorToaster").text();

    if (successMessage !== '' || errorMessage !== '') {
        if (successMessage !== '')
            notyf.success(successMessage);
        else
            notyf.error(errorMessage);
    }
}

function userIsLoggedIn() {

    var currentUrl = window.location.href;
    var notLoggedinUrl = "https://localhost:7071/";

    return (currentUrl == notLoggedinUrl) || currentUrl.includes("Register") ? false : true;
}

function setItemNumberCookie() {

    let isNumberOfItemsInCartUpdated = false;
    let numberOfItemsInCart = parseInt($(".numberOfItemsInCart").text(), 10)
    isNumberOfItemsInCartUpdated = numberOfItemsInCart || numberOfItemsInCart === 0;

    let cookie = Cookies.get('cartCount');

    if (isNumberOfItemsInCartUpdated)
        Cookies.set('cartCount', numberOfItemsInCart);
    else if (!cookie)
        Cookies.set('cartCount', 0);
}

function setCartBadgeValue() {

    let cartCount = 0;
    let cartCountString = Cookies.get('cartCount');

    cartCount = parseInt(cartCountString, 10);

    if (cartCount !== 0) {
        $(".numberOfItemsBadge").text(cartCount);
        $(".numberOfItemsBadge").removeClass("d-none");
    }
}

