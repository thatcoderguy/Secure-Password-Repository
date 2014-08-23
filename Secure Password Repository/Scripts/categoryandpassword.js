
function createNewCategory(holderid) {

    //generate a random number, so the multiple forms can be on the same page
    var random = parseInt(Math.random() * 1000).toString();
    //grab the parent category id
    var parentid = holderid.split('-')[1];

    $(holderid).html(strNewCategoryForm.replace('[1]', random).replace('[2]', random).replace('[3]', parentid));

}

function saveNewCategory(formid) {
    $(formid).submit();
}

// Setup CSRF safety for AJAX:
$.ajaxPrefilter(function (options, originalOptions, jqXHR) {
    if (options.type.toUpperCase() === "POST") {
        // We need to add the verificationToken to all POSTs
        var token = $("input[name^=__RequestVerificationToken]").first();
        if (!token.length) return;

        var tokenName = token.attr("name");

        // If the data is JSON, then we need to put the token in the QueryString:
        if (options.contentType.indexOf('application/json') === 0) {
            // Add the token to the URL, because we can't add it to the JSON data:
            options.url += ((options.url.indexOf("?") === -1) ? "?" : "&") + token.serialize();
        } else if (typeof options.data === 'string' && options.data.indexOf(tokenName) === -1) {
            // Append to the data string:
            options.data += (options.data ? "&" : "") + token.serialize();
        }
    }
});
