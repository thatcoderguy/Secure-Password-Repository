
function createNewCategory(holderid) {

    var random = parseInt(Math.random() * 1000).toString();
    var parentid = holderid.split('-')[1];
    var token = $("input[name^=__RequestVerificationToken]").first();

    $(holderid).html(strNewCategory.replace('{0}',parentid).replace('{1}',random).replace('{2}',random).replace('{3}',token.val()));

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
