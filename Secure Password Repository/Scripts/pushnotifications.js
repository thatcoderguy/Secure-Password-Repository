
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
        $('#' + updatedCategory.CategoryId).find('.categoryname').text(updatedCategory.CategoryName);
    });

    pushnotifierProxy.on('sendUpdatedPasswordDetails', function (updatedPassword) {
        alert('f');
    });

    pushnotifierProxy.on('sendAddedCategoryDetails', function (addedCategoryView, parentId) {

        //append the new item to the list
        $('#addnew-' + parentId).parent().append(addedCategoryView);

        //move the new item to the bottom of the list
        $('#addnew-' + parentId).appendTo($('#addnew-' + parentId).parent());

        //remove the "add new password" button
        $('#addnew-password-' + parentId).remove();

        checkActionButtons();

        //refresh treeview, so new item is part of sortable
        refreshTreeView('treeview');

        //bind click events
        bindClickEvent();
    });

    pushnotifierProxy.on('sendAddedPasswordDetails', function (addedPassword) {
        alert('f');
    });

    // Start the connection
    connection.start().done(function () {
    });

});

