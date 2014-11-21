var showPasswordSpinner = function (obj, formid) {
    
    $('#' + formid).submit();

    setTimeout(function () {
        $(obj).parent().after('<span class="loaderplaceholder"><img src="/images/ajax-loader.gif"> Saving...</span>').remove();
    }, 10);

}