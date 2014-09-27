
$(function () {

    // the generated client-side hub proxy
    var connection = $.hubConnection();
    var pushnotifierProxy = connection.createHubProxy('CategoryAndPasswordHub');

    pushnotifierProxy.on('sendDeletedCategoryDetails', function (deletedCategory) {
        alert('q');
    });
    pushnotifierProxy.on('sendDeletedPasswordDetails', function (deletedPassword) {
        alert('f');
    });

    pushnotifierProxy.on('sendUpdatedCategoryDetails', function (updatedCategory) {
        alert('q');
    });
    pushnotifierProxy.on('sendUpdatedPasswordDetails', function (updatedPassword) {
        alert('f');
    });

    pushnotifierProxy.on('sendAddedCategoryDetails', function (addedCategory) {
        alert(addedCategory);
    });
    pushnotifierProxy.on('sendAddedPasswordDetails', function (addedPassword) {
        alert('f');
    });

    // Start the connection
    connection.start({ transport: ['foreverFrame', 'longPolling'] }).done(function () {
    });

});

