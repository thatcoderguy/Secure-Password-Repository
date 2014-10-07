$(function () {
    
    $('#tabbedpanelcontainer').tabbedPanels({
        eleTabText: 'h2:first'
    });

    if (window.location.toString().indexOf('localhost') > 0) {
        $('a.btn').zclip({
            path: 'http://www.steamdev.com/zclip/js/ZeroClipboard.swf',
            copy: function () { return $(this).parent().parent().find('input').val(); }
        });
    } else {
        $('a.btn').zclip({
            path: '/Flash/ZeroClipboard.swf',
            copy: function () { return $(this).parent().parent().find('input').val(); }
        });
    }

});

var tabClick = function (tabName) {

}

var copyPassword = function () {

}