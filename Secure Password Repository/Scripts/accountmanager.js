"use strict";

var authoriseAccount = function (event, accountid) {

    event.preventDefault();

    var result = $.ajax({
        type: "POST",
        url: "/UserManager/AuthoriseAccount",
        data: AddAntiForgeryToken({ UserId: accountid }),
        contentType: "application/x-www-form-urlencoded; charset=utf-8",
        dataType: "json",
        success: function (data) {

            $('#' + accountid).find('.btn-authorise').remove();

        }
    });

}