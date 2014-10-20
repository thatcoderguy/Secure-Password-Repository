var showPasswordSpinner = function (obj, formid) {
    $(obj).parent().after('<span class="loaderplaceholder"><img src="/images/ajax-loader.gif"> Saving...</span>').remove();

    setTimeout(function () {
        $('#' + formid).submit();
    }, 500);
}