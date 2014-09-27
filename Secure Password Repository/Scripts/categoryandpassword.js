"use strict";

//submit form
var submitAjaxForm = function(formid) {
    $('#' + formid).submit();
}

//display the edit category form
var displayForm = function(categoryid) {

    $('#' + categoryid).find('.first').first().hide();
    $('#' + categoryid).find('.second').first().show();

}

//hide the form
var cancelAction = function(categoryid) {

    $('#' + categoryid).find('.first').first().show();
    $('#' + categoryid).find('.second').first().hide();

}

//category updates successfully
var updateCategorySuccess = function(data) {

    //update text
    $('#' + data.CategoryId).find('.categoryname').text(data.CategoryName);

    //hide the form
    cancelAction(data.CategoryId);

}

//called when category successfully created
var createCategorySuccess = function(data, parentid) {

    //append the new item to the list
    $('#addnew-' + parentid).parent().append(data);
    
    //move the new item to the bottom of the list
    $('#addnew-' + parentid).appendTo($('#addnew-' + parentid).parent());
    
    //remove the "add new password" button
    $('#addnew-password-' + parentid).remove();
    
    //clear add new category form
    $('#addnew-' + parentid).find('.second input[name="CategoryName"]').val('');

    //refresh treeview, so new item is part of sortable
    refreshTreeView('treeview');

    //bind click events
    bindClickEvent();

    //hide the form
    cancelAction('addnew-' + parentid);

}

//click event to delete category
var deleteCategory = function(categoryid) {

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

//load children of the category clicked on
var treeListItemClick = function(event, listItem) {

        var result = $.ajax({
            type: "POST",
            url: "/Password/GetCategoryChildren",
            data: AddAntiForgeryToken({ ParentCategoryId: listItem.parent().parent().attr('id') }),
            contentType: "application/x-www-form-urlencoded; charset=utf-8",
            dataType: "html",
            success: function (data) {

                listItem.parent().addClass('ui-state-active');
                listItem.find('span').removeClass('treeviewplus').addClass('treeviewminus').parent().find('i').removeClass('glyphicon-folder-close').addClass('glyphicon-folder-open');

                //append the generated HTML to the category item
                listItem.parent().parent().append(data);

                //initialize new treeviews added from the newly generated HTML
                setupTreeView('treeview');

                //bind click events
                bindClickEvent();
                bindPasswordClickEvent();

                //open the parent item
                listItem.slideDown();

                isPopulating = false;

            }
        });
}

//update the ordering position of the category
var updatePosition = function(event, ui)
{
    var itemid = ui.item.attr('id');
    var newposition = ui.item.index() + 1;
    var oldposition = ui.item.data('previndex');
    ui.item.removeAttr('data-previndex');

    var result = $.ajax({
        type: "POST",
        url: "/Password/UpdatePosition",
        data: AddAntiForgeryToken({ ItemID: itemid, NewPosition: newposition, OldPosition: oldposition, isCategoryItem: (ui.item.data('type').toString().toLowerCase() == 'category') }),
        contentType: "application/x-www-form-urlencoded; charset=utf-8",
        dataType: "html",
        success: function (data) {

            if (data.Status == 'Failed')
                alert('Sorry it looks like something went wrong, please press F5 - if you keep getting this error, please contact support');

        }
    });

}