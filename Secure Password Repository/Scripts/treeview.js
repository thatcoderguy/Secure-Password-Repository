//category treeview
$(function () {

    setupTreeView('treeview');

});

function setupTreeView(className) {

    var icons = {
        header: "treeviewicon treeviewplus",
        activeHeader: "treeviewicon treeviewminus"
    };
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

             ui.item.data('previndex', ui.item.index()+1);

          },
          stop: function (event, ui) {
              // IE doesn't register the blur when sorting
              // so trigger focusout handlers to remove .ui-state-focus
              ui.item.children("li").triggerHandler("focusout");
          },
          update: function (event, ui) {
    
              updateCategoryPosition(event, ui);

          }

      });

}

//when a tree item is clicked, load its children
$('.clickable').on('click', function (event) {

    //make sure item isnt already open and hasnt already has its children loaded
    if (!$(this).parent().hasClass('ui-state-active') && !$(this).parent().parent().has('ul').length)
        treeListItemClick(event, $(this));
    //not open
    else if (!$(this).parent().hasClass('ui-state-active')) {

        $(this).parent().parent().find('ul').slideDown();
        $(this).parent().addClass('ui-state-active');
        $(this).find('span').removeClass('treeviewplus').addClass('treeviewminus');

    } else {

        $(this).parent().parent().find('ul').slideUp();
        $(this).parent().removeClass('ui-state-active');
        $(this).find('span').removeClass('treeviewminus').addClass('treeviewplus');

    }

});