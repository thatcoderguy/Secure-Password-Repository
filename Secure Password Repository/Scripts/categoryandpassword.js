//submit form
function submitAjaxForm(formid) {
    $('#' + formid).submit();
}

//display the edit category form
function displayForm(categoryid) {

    $('#' + categoryid).find('.first').hide();
    $('#' + categoryid).find('.second').show();

}

//hide the form
function cancelCategoryEdit(categoryid) {

    $('#' + categoryid).find('.first').show();
    $('#' + categoryid).find('.second').hide();

}

function failAlert() {

    alert('Sorry it looks like something went wrong, please press F5 - if you keep getting this error, please contact support');

}

function updateSuccess(data) {
    alert(data);
    //alert($('#' + categoryid).find('.second input').val());
    

}

//load children of the category clicked on
function treeListItemClick(event, listItem) {

        var result = $.ajax({
            type: "POST",
            url: "/Password/GetCategoryChildren",
            data: AddAntiForgeryToken({ ParentCategoryId: listItem.parent().parent().attr('id') }),
            contentType: "application/x-www-form-urlencoded; charset=utf-8",
            dataType: "html",
            success: function (data) {

                listItem.parent().addClass('ui-state-active');
                listItem.find('span').removeClass('treeviewplus').addClass('treeviewminus');

                //append the generated HTML to the category item
                listItem.parent().parent().append(data);

                //initialize new treeviews added from the newly generated HTML
                setupTreeView('treeview');

                listItem.slideDown();

            },
            failure: function (msg) {
                alert('Sorry it looks like something went wrong, please press F5 - if you keep getting this error, please contact support');
                return false;
            },
            error: function (xhr, err) {
                alert('Sorry it looks like something went wrong, please press F5 - if you keep getting this error, please contact support');
                return false;
            }
        });
}

//update the ordering position of the category
function updateCategoryPosition(event, ui)
{

    var categoryid = ui.item.attr('id');
    var newposition = ui.item.index() + 1;
    var oldposition = ui.item.data('previndex');
    ui.item.removeAttr('data-previndex');

    var result = $.ajax({
        type: "POST",
        url: "/Password/UpdateCategoryPosition",
        data: AddAntiForgeryToken({ CategoryId: categoryid, NewPosition: newposition, OldPosition: oldposition }),
        contentType: "application/x-www-form-urlencoded; charset=utf-8",
        dataType: "html",
        success: function (data) {

            if(data=='failed')
                alert('Sorry it looks like something went wrong, please press F5 - if you keep getting this error, please contact support');

        },
        failure: function (msg) {
            alert('Sorry it looks like something went wrong, please press F5 - if you keep getting this error, please contact support');
            return false;
        },
        error: function (xhr, err) {
            alert('Sorry it looks like something went wrong, please press F5 - if you keep getting this error, please contact support');
            return false;
        }
    });

}

//update the ordering position of the password
function updatePasswordPositition(event, ui)
{

    var passwordid = ui.item.data('id');
    var newposition = ui.item.index() + 1;

    var result = $.ajax({
        type: "POST",
        url: "/Password/UpdatePasswordPosition",
        data: AddAntiForgeryToken({ PasswordId: passwordid, NewPosition: newposition }),
        contentType: "application/x-www-form-urlencoded; charset=utf-8",
        dataType: "html",
        success: function (data) {

            if(data=='failed')
                alert('Sorry it looks like something went wrong, please press F5 - if you keep getting this error, please contact support');

        },
        failure: function (msg) {
            alert('Sorry it looks like something went wrong, please press F5 - if you keep getting this error, please contact support');
            return false;
        },
        error: function (xhr, err) {
            alert('Sorry it looks like something went wrong, please press F5 - if you keep getting this error, please contact support');
            return false;
        }
    });

}