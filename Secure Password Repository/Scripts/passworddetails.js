$(function () {
   
    if (window.location.toString().indexOf('localhost') > 0) {
        ZeroClipboard.config({ swfPath: "http://www.fauxbank.co.uk/ZeroClipboard.swf" });
    } else {
        ZeroClipboard.config({ swfPath: "Flash/ZeroClipboard.swf" });
    }

    var client = new ZeroClipboard($('a.autobutton'));

    client.on('ready', function (event) {
        client.on('copy', function (event) {
            event.clipboardData.setData('text/plain', $(event.target).parent().parent().find('input').val());
            alert('Copied');
        });
    });

    var passwdclient = new ZeroClipboard($('a.passwordautobutton'));

    passwdclient.on('ready', function (event) {

        passwdclient.on('beforecopy', function (event) {
            var result = $.ajax({
                type: "POST",
                url: "/Password/GetEncryptedPassword",
                data: AddAntiForgeryToken({ PasswordId: $(event.target).data('passwordid') }),
                contentType: "application/x-www-form-urlencoded; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    $(event.target).parent().parent().parent().find('input').val(data.Password);
                }
            })
        });

        passwdclient.on('copy', function (event) {
            event.clipboardData.setData('text/plain', $(event.target).parent().parent().parent().find('input').val());
            $(event.target).parent().parent().parent().find('input').val('');
        });

        passwdclient.on('aftercopy', function (event) {
            $(event.target).parent().parent().parent().find('input').val('');
            alert('Copied');
        });

    });
    
});

var tabClick = function (tabName) {

}
