$(function () {
    showToasterGlobal();
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
