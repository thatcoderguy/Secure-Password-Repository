"use strict";

//used to stop double population
var populatingItemDictionary = {};

//category treeview - called upon script load
$(function () {

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

//load the popup when "add new password" is clicked
var bindPasswordClickEvent = function() {

    $('a.magnificbutton').magnificPopup({ type: 'iframe' });

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
            if (!populatingItemDictionary.hasOwnProperty($(this).find('.categoryname').text()) || !populatingItemDictionary[$(this).find('.categoryname').text()]) {
                populatingItemDictionary[$(this).find('.categoryname').text()] = true;
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

//load children of the category clicked on
var treeListItemClick = function (event, listItem) {

    event.preventDefault();

    listItem.parent().find('.loaderplaceholder').show();

    var result = $.ajax({
        type: "POST",
        url: "/Password/GetCategoryChildren",
        data: AddAntiForgeryToken({ ParentCategoryId: listItem.parent().parent().attr('id') }),
        contentType: "application/x-www-form-urlencoded; charset=utf-8",
        dataType: "html",
        success: function (data) {

            listItem.parent().find('.loaderplaceholder').hide();

            listItem.parent().addClass('ui-state-active');
            listItem.find('span').removeClass('treeviewplus').addClass('treeviewminus').parent().find('i').removeClass('glyphicon-folder-close').addClass('glyphicon-folder-open');

            //append the generated HTML to the category item
            listItem.parent().parent().append(data);

            //initialize new treeviews added from the newly generated HTML
            setupTreeView('treeview');

            //bind click events
            bindClickEvent();
            bindPasswordClickEvent();

            //open the parent item
            listItem.slideDown();

            populatingItemDictionary[listItem.find('.categoryname').text()] = false;

        }
    });
}

//update the ordering position of the category
var updatePosition = function (event, ui) {
    var itemid = ui.item.attr('id');
    var newposition = ui.item.index() + 1;
    var oldposition = ui.item.data('previndex');
    ui.item.removeAttr('data-previndex');
    
    var result = $.ajax({
        type: "POST",
        url: "/Password/UpdatePosition",
        data: AddAntiForgeryToken({ ItemId: itemid.replace('Password',''), NewPosition: newposition, OldPosition: oldposition, isCategoryItem: (ui.item.data('type').toString().toLowerCase() == 'category') }),
        contentType: "application/x-www-form-urlencoded; charset=utf-8",
        dataType: "html",
        success: function (data) {

            if (data.Status == 'Failed')
                alert('Sorry it looks like something went wrong, please press F5 - if you keep getting this error, please contact support');

        }
    });

}

var showSpinner = function (itemid) {
    $('#' + itemid).find('.btn-group').hide().after('<span class="loaderplaceholder"><img src="/Images/ajax-loader.gif"> Saving...</span>');
}

var hideSpinner = function (itemid) {
    $('#' + itemid).find('.loaderplaceholder').remove();
    $('#' + itemid).find('.btn-group').show();
}