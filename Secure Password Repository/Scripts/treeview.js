"use strict";

//semaphor - do stop double population
var isPopulating = false;

//category treeview - called upon script load
$(function () {

    checkActionButtons();
    setupTreeView('treeview');
    bindClickEvent();
    
});

var setupTreeView = function(className) {

    //if users can edit categories, then they can re-order categories
    if (!canEditCategory)
        return false;

    //make the treeview sortable
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

//fired after more items have been added
var refreshTreeView = function(className) {

    $("." + className).sortable('refresh');
}

//load the fancy box when "add new password" is clicked
var bindPasswordClickEvent = function() {

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

//bind click even to all treeview items - this is what pulls in the children when an item is clicked
var bindClickEvent = function() {

    //unbind all of the click events - so that we dont get multiple events per item
    $('.clickable').unbind('click');

    //when a tree item is clicked, load its children
    $('.clickable').on('click', function (event) {     

        //make sure item isnt already open and hasnt already has its children loaded
        if (!$(this).parent().hasClass('ui-state-active') && !$(this).parent().parent().has('ul').length) {

            //only call the click even if we arent already populating
            if (!isPopulating) {
                isPopulating = true;
                treeListItemClick(event, $(this));
            }

        //category is not open
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