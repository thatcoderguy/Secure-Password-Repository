//add the CSRF token to AJAX requests
AddAntiForgeryToken = function (data) {
    data.__RequestVerificationToken = $('#_CRSFform input[name=__RequestVerificationToken]').val(); return data;
};