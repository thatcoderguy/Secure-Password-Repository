"use strict";

//add the CSRF token to AJAX requests
var AddAntiForgeryToken = function (data) {
    data.__RequestVerificationToken = $('#_CRSFform input[name=__RequestVerificationToken]').val(); return data;
};

//replace verification token with the one for this client
var UpdateToClientVerificationToken = function (viewdata) {

    return viewdata.replace(/<input name="__RequestVerificationToken" type="hidden" value="(.*?)" \/>/g, '<input name="__RequestVerificationToken" type="hidden" value="' + $('#_CRSFform input[name=__RequestVerificationToken]').val() + '" />')
}

//global ajax error handling
$(document).ajaxError(function (event, jqxhr, settings, thrownError) {

    isPopulating = false;

    if (thrownError == 'Unauthorized') {
        window.location = '/Login?ReturnUrl=/Password';
    } else {
        alert('Sorry it looks like something went wrong, please press F5 - if you keep getting this error, please contact support');
        return false;
    }

});