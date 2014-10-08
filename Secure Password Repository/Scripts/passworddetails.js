var password = 'WAIT';

$(function () {
   
    if (window.location.toString().indexOf('localhost') > 0) {

        $('a.autobutton').zclip({
            path: 'http://www.steamdev.com/zclip/js/ZeroClipboard.swf',
            copy: function () {
                return $(this).parent().parent().find('input').val();
            }
        });

        $('a.passwordautobutton').zclip({
            path: 'http://www.steamdev.com/zclip/js/ZeroClipboard.swf',
            beforeCopy: function() {
            },
            copy: function () {
                return password;
            }
        });

    } else {

        $('a.autobutton').zclip({
            path: '/Flash/ZeroClipboard.swf',
            copy: function () {
                return $(this).parent().parent().find('input').val();
            }
        });

        $('a.passwordautobutton').zclip({
            path: '/Flash/ZeroClipboard.swf',
            copy: function () {
                var result = $.ajax({
                    type: "POST",
                    url: "/Password/GetEncryptedPassword",
                    data: AddAntiForgeryToken({ PasswordId: $(this).data('passwordid') }),
                    contentType: "application/x-www-form-urlencoded; charset=utf-8",
                    dataType: "json",
                    success: function (data) {

                        return data.Password;

                    }
                });
            }
        });

    }

});

var tabClick = function (tabName) {

}

var getPassword = function (buttonobj) {
    var result = $.ajax({
        type: "POST",
        url: "/Password/GetEncryptedPassword",
        data: AddAntiForgeryToken({ PasswordId: buttonobj.data('passwordid') }),
        contentType: "application/x-www-form-urlencoded; charset=utf-8",
        dataType: "json",
        async: false
    }).done(function (data) {
        password = data.Password;
        $('#ZeroClipboardMovie_3').click();
    });
}