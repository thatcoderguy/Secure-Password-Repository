
//add the CSRF token to AJAX requests
AddAntiForgeryToken = function (data) {
    data.__RequestVerificationToken = $('#_CRSFform input[name=__RequestVerificationToken]').val(); return data;
};

$(document).ajaxError(function( event, jqxhr, settings, thrownError ) {
    if(thrownError=='Unauthorised') {
        window.location = '/';
    }
}

//AJAX FAIL

//AJAX ERROR