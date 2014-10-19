"use strict";

//submit form
var submitAjaxForm = function (event,formid) {
    event.preventDefault();
    $('#' + formid).submit();
}

//display the edit category form
var displayForm = function(event,categoryid) {
    event.preventDefault();
    $('#' + categoryid).find('.first').first().hide();
    $('#' + categoryid).find('.second').first().show();
}

//hide the form
var cancelAction = function (event, categoryid) {
    $('#' + categoryid).find('.first').first().show();
    $('#' + categoryid).find('.second').first().hide();
}

//category updates successfully
var updateCategorySuccess = function (event, data) {

    //update text
    $('#' + data.CategoryId).find('.categoryname').text(data.CategoryName);

    //hide the form
    cancelAction(event, data.CategoryId);
}

//called when category successfully created
var createCategorySuccess = function (data, parentid) {
    
    //append the new item to the list
    $('#addnew-' + parentid).parent().append(data);
    
    //move the new item to the bottom of the list
    $('#addnew-' + parentid).appendTo($('#addnew-' + parentid).parent());
    
    //remove the "add new password" button
    $('#addnew-password-' + parentid).remove();
    
    //clear add new category form
    $('#addnew-' + parentid).find('.second input[name="CategoryName"]').val('');

    //refresh treeview, so new item is part of sortable
    setupTreeView('treeview');

    //bind click events
    bindClickEvent();

    //hide the form
    cancelAction(Object, 'addnew-' + parentid);
}

//click event to delete category
var deleteCategory = function (event, categoryid) {

    event.preventDefault();

    //double confirm
    if (confirm('Are you sure you wish to delete this category?\n\nYou will not be able to view any passwords linked to it\n\nHowever, Administrators can undelete Categories\n\n'))
    {
        if(confirm('ARE YOU REALLY SURE YOU WISH TO DELETE THIS CATEGORY?\n\nYou will NOT be able to VIEW any PASSWORDS linked to it\n\n'))
        {

            var result = $.ajax({
                type: "POST",
                url: "/Password/DeleteCategory",
                data: AddAntiForgeryToken({ CategoryId: categoryid }),
                contentType: "application/x-www-form-urlencoded; charset=utf-8",
                dataType: "json",
                success: function (data) {

                    $('#' + data.CategoryId).remove();

                }
            });

        }
    }
}

var deletePassword = function (event, passwordid) {

    event.preventDefault();

    //double confirm
    if (confirm('Are you sure you wish to delete this password\n\nAdministrators can undelete Passwords\n\n')) {
        if (confirm('ARE YOU REALLY SURE YOU WISH TO DELETE THIS PASSWORD?\n\n')) {

            var result = $.ajax({
                type: "POST",
                url: "/Password/DeletePassword",
                data: AddAntiForgeryToken({ PasswordId: passwordid }),
                contentType: "application/x-www-form-urlencoded; charset=utf-8",
                dataType: "json",
                success: function (data) {

                    $('#Password-' + data.PasswordId).remove();

                }
            });

        }
    }
}