$(function () {

    //ZeroClipboard cannot run flash locally, so we need to run it form and external source
    if (window.location.toString().indexOf('localhost') > 0) {
        ZeroClipboard.config({ swfPath: "http://www.fauxbank.co.uk/ZeroClipboard.swf" });
    } else {
        ZeroClipboard.config({ swfPath: "Flash/ZeroClipboard.swf" });
    }

    //declare a new clipboard client
    var client = new ZeroClipboard($('a.autobutton'));

    //tell the clipboard where to copy data from and alert the user that it has been copied
    client.on('ready', function (event) {
        client.on('copy', function (event) {
            event.clipboardData.setData('text/plain', $(event.target).parent().parent().find('input').val());
            alert('Copied');
        });
    });

    //the password button works differently
    var passwdclient = new ZeroClipboard($('a.passwordautobutton'));

    passwdclient.on('ready', function (event) {

        //before copying, grab a decrypted copy of the password
        passwdclient.on('beforecopy', function (event) {
            var result = $.ajax({
                type: "POST",
                url: "/Password/GetEncryptedPassword",
                data: AddAntiForgeryToken({ PasswordId: $(event.target).data('passwordid') }),
                contentType: "application/x-www-form-urlencoded; charset=utf-8",
                dataType: "json",
                async: false,       //syncronous - this will lock up the UI, but will guarantee we have the password
                success: function (data) {
                    $(event.target).parent().parent().parent().find('input').val(data.PlainTextPassword);
                }
            })
        });

        //copy the password into clipboard
        passwdclient.on('copy', function (event) {
            event.clipboardData.setData('text/plain', $(event.target).parent().parent().parent().find('input').val());
            $(event.target).parent().parent().parent().find('input').val('');
        });

        //after copying, clear the password hidden input and alert the user that the password has been copied
        passwdclient.on('aftercopy', function (event) {
            $(event.target).parent().parent().parent().find('input').val('');
            alert('Copied');
        });

    });
    
});

var tabClick = function (tabName) {

}
