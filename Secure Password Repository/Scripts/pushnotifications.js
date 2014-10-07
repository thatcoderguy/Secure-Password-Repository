
$(function () {

    // the generated client-side hub proxy
    var connection = $.hubConnection();
    var pushnotifierProxy = connection.createHubProxy('BroadcastHub');


    pushnotifierProxy.on('sendDeletedCategoryDetails', function (deletedCategory) {
        $('#' + deletedCategory.CategoryId).remove();
    });


    pushnotifierProxy.on('sendDeletedPasswordDetails', function (deletedPassword) {
        $('#password-' + deletedCategory.CategoryId).remove();
    });


    pushnotifierProxy.on('sendUpdatedCategoryDetails', function (updatedCategory) {
        $('#' + updatedCategory.CategoryId).find('.categoryname').text(updatedCategory.CategoryName);                   //update the display
        $('#' + updatedCategory.CategoryId).find('form input[name=CategoryName]').val(updatedCategory.CategoryName);    //update the form
    });


    pushnotifierProxy.on('sendUpdatedPasswordDetails', function (updatedPassword) {
        alert('f');
    });


    //a new category has been added to the system - request the details from the server
    pushnotifierProxy.on('newCategoryAdded', function (newCategoryId) {

        pushnotifierProxy.invoke('getNewCategoryDetails', newCategoryId);

    });

    //received the new category details - so now add it into the list
    pushnotifierProxy.on('sendAddedCategoryDetails', function (addedCategoryView, parentId) {

        //append the new item to the list
        $('#addnew-' + parentId).parent().append(addedCategoryView);

        //move the new item to the bottom of the list
        $('#addnew-' + parentId).appendTo($('#addnew-' + parentId).parent());

        //remove the "add new password" button
        $('#addnew-password-' + parentId).remove();

        //refresh treeview, so new item is part of sortable
        setupTreeView('treeview');

        //bind click events
        bindClickEvent();

    });


    //a new password has been added to the system - request the details from the server
    pushnotifierProxy.on('newPasswordAdded', function (newPasswordId) {

        pushnotifierProxy.invoke('getNewPasswordDetails', newPasswordId);

    });


    //received the new password details - so now add it into the list
    pushnotifierProxy.on('sendAddedPasswordDetails', function (addedPasswordView, passwordParentId) {

        //append the new item to the list
        $('#addnew-password-' + passwordParentId).parent().append(addedPasswordView);

        //move the new item to the bottom of the list
        $('#addnew-password-' + passwordParentId).appendTo($('#addnew-password-' + passwordParentId).parent());

        //remove the "add new category" button
        $('#addnew-' + passwordParentId).remove();

        //refresh treeview, so new item is part of sortable
        setupTreeView('treeview');

        //bind click events
        bindClickEvent();

    });


    pushnotifierProxy.on('sendUpdatedItemPosition', function (ItemID, NewPosition, OldPosition) {

        var html = $('#' + ItemID)[0].outerHTML;   //store html

        var parent = $('#' + ItemID).parent('ul').attr('id'); //grab the id of the parent

        $('#' + ItemID).remove();   //remove the item from the list

        $('#' + parent + ' li:nth-child(' + NewPosition + ')').before(html);     //reinsert item at new position

    });


    // Start the connection
    connection.start().done(function () {
    });

});

