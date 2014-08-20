var strNewCategory = '<div><i class="glyphicon glyphicon-folder-open rightfolderpadding"></i><form method="post" class="newcategoryform" action="/Password/AddCategory" id="{1}"><input name="__RequestVerificationToken" type="hidden" value="{3}" /><input type="hidden" name="ParentId" value="{0}" /><input type="text" name="CategoryName" value="" /><span class="btn-group groupalign"><a href="#" onclick="saveNewCategory(\'#{2}\');return false;" class="btn btn-xs btn-primary"><i class="glyphicon glyphicon-save"></i>Save</a></span></form></div>';

//<div><i class="glyphicon glyphicon-folder-open rightfolderpadding"></i>Category 3<span class="btn-group"><a href="@Url.Action("Edit", "0")" class="btn btn-xs btn-primary"><i class="glyphicon glyphicon-edit"></i>Edit</a><a href="@Url.Action("Delete", "0")" class="btn btn-xs btn-danger"><i class="glyphicon glyphicon-remove"></i>Delete</a></span></div>

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
