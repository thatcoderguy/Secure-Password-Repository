//semaphor - do stop double population
var isPopulating = false;


//category treeview
$(function () {

    setupTreeView('treeview');
    bindClickEvent();
    
});

function setupTreeView(className) {

    $("." + className)
      .sortable({
          axis: "y",
          handle: "div",
          containment: 'parent',
          tolerance: 'pointer',
          cursor: 'n-resize',
          items: "li:not(.treeignore)",
          distance: 10,
          start: function (event, ui) {

              ui.item.data('previndex', ui.item.index() + 1);

          },
          stop: function (event, ui) {
              // IE doesn't register the blur when sorting
              // so trigger focusout handlers to remove .ui-state-focus
              ui.item.children("li").triggerHandler("focusout");
          },
          update: function (event, ui) {

              updatePosition(event, ui);

          }

      });

}

function refreshTreeView(className) {

    $("." + className).sortable('refresh');
}

function bindPasswordClickEvent() {

    //unbind all of the click events - so that we dont get multiple events per item
    $('a.fancyboxbutton').unbind('click');

    $('a.fancyboxbutton').on('click', function (event) {
        $.fancybox({
            padding: 0,
            'href': this.href,
            'type': 'iframe'
        });
    });

}

function bindClickEvent() {

    //unbind all of the click events - so that we dont get multiple events per item
    $('.clickable').unbind('click');

    //when a tree item is clicked, load its children
    $('.clickable').on('click', function (event) {     

        //make sure item isnt already open and hasnt already has its children loaded
        if (!$(this).parent().hasClass('ui-state-active') && !$(this).parent().parent().has('ul').length) {

            if (!isPopulating) {
                isPopulating = true;
                treeListItemClick(event, $(this));
            }

            //not open
        } else if (!$(this).parent().hasClass('ui-state-active')) {

            $(this).parent().parent().find('ul').slideDown();
            $(this).parent().addClass('ui-state-active');
            $(this).find('span').removeClass('treeviewplus').addClass('treeviewminus').parent().find('i').removeClass('glyphicon-folder-close').addClass('glyphicon-folder-open');

            //already populated and open
        } else {

            $(this).parent().parent().find('ul').slideUp();
            $(this).parent().removeClass('ui-state-active');
            $(this).find('span').removeClass('treeviewminus').addClass('treeviewplus').parent().find('i').removeClass('glyphicon-folder-open').addClass('glyphicon-folder-close');

        }

    });

}