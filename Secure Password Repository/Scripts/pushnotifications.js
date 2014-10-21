
$(function () {

    // the generated client-side hub proxy
    var connection = $.hubConnection();
    var pushnotifierProxy = connection.createHubProxy('BroadcastHub');

    //when a category is deleted, this method picks up the details
    pushnotifierProxy.on('sendDeletedCategoryDetails', function (deletedCategory) {
        $('#' + deletedCategory.CategoryId).remove();
    });

    //when a password is deleted, this method picks up the details
    pushnotifierProxy.on('sendDeletedPasswordDetails', function (deletedPassword) {
        $('#Password-' + deletedPassword.PasswordId).remove();
    });

    //when a category is updated, this method picks up the details
    pushnotifierProxy.on('sendUpdatedCategoryDetails', function (updatedCategory) {
        $('#' + updatedCategory.CategoryId).find('.categoryname').text(updatedCategory.CategoryName);                   //update the display
        $('#' + updatedCategory.CategoryId).find('form input[name=CategoryName]').val(updatedCategory.CategoryName);    //update the form
    });

    //when a password is deleted, this method picks up the details
    pushnotifierProxy.on('sendUpdatedPasswordDetails', function (updatedPassword) {

        //if the password exists in the UI
        if ($('#Password-' + updatedPassword.PasswordId).length>0)
            $('#Password-' + updatedPassword.PasswordId).find('.passwordname').text(updatedPassword.Description);           //update the display

    });


    //a new category has been added to the system - request the details from the server
    pushnotifierProxy.on('newCategoryAdded', function (newCategoryId) {
        pushnotifierProxy.invoke('getNewCategoryDetails', newCategoryId);
    });

    //received the new category details - so now add it into the list
    pushnotifierProxy.on('sendAddedCategoryDetails', function (addedCategoryView, parentId) {

        //if the user has acceess to add a category - stick the new password item before the button
        if ($('#addnew-' + parentId).length > 0)
        {
            //append the new item to the list
            $('#addnew-' + parentId).parent().append(addedCategoryView);

            //move the new item to the bottom of the list
            $('#addnew-' + parentId).appendTo($('#addnew-' + parentId).parent());

            //remove the "add new password" button
            $('#addnew-password-' + parentId).remove();
        }
        //user does not have access to add category - so stick new catagory item at the end of the list
        else {

            $('#parent-' + parentId).append(addedCategoryView);
        }


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
    pushnotifierProxy.on('sendAddedPasswordDetails', function (addedPasswordView, passwordParentId, passwordId) {

        var originalindex = 0;

        //if the item already exists (this may occur on a permission change), then just remove and re-add
        if ($('#Password-' + passwordId).length > 0) {

            //get position of item before removing it
            originalindex = $('#Password-' + passwordId).index();
            $('#Password-' + passwordId).remove();

            //insert item at it's original position
            $('#parent-' + passwordParentId + ' li').eq(originalindex).before(addedPasswordView);

        } else {

            //user has access to add new password
            if ($('#addnew-password-' + passwordParentId).length > 0) {

                //append the new item to the list
                $('#addnew-password-' + passwordParentId).parent().append(addedPasswordView);

                //move the new item to the bottom of the list
                $('#addnew-password-' + passwordParentId).appendTo($('#addnew-password-' + passwordParentId).parent());

                //remove the "add new category" button
                $('#addnew-' + passwordParentId).remove();

            }
            //user does not have access to add new password
            else {

                $('#parent-' + passwordParentId).append(addedPasswordView);

            }

        }
            
        //refresh treeview, so new item is part of sortable
        setupTreeView('treeview');

        //bind click events
        bindClickEvent();
        bindPasswordClickEvent();

    });


    //when a category or password's position has been updated, broadcast it to all clients
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

