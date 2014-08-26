//category treeview
$(function () {
    var icons = {
        header: "treeviewicon treeviewplus",
        activeHeader: "treeviewicon treeviewminus"
    };
    $(".treeview")
      .accordion({
          icons: icons,
          collapsible: true,
          active: false,
          heightStyle: "content",
      })
      .sortable({
          axis: "y",
          handle: "div",
          containment: 'parent',
          tolerance: 'pointer',
          cursor: 'n-resize',
          items: "li:not(.treeignore)",
          distance: 10,
          stop: function (event, ui) {
              // IE doesn't register the blur when sorting
              // so trigger focusout handlers to remove .ui-state-focus
              ui.item.children("li").triggerHandler("focusout");

              // Refresh accordion to handle new order
              $(this).accordion("refresh");
          },
          update: function (event, ui) {
    
              updateCategoryPosition(event, ui);

          }

      });

    //auto open "root"
    $('#ui-id-1').removeClass('ui-corner-all').addClass('ui-accordion-header-active').addClass('ui-state-active').addClass('ui-corner-top').find('span').removeClass('treeviewplus').addClass('treeviewminus');
    $('#ui-id-2').show();

});


//password treeview
$(function () {
    var icons = {
        header: "treeviewicon password",
        activeHeader: "treeviewicon password"
    };
    $(".treeview-password")
      .accordion({
          icons: icons,
          collapsible: true,
          active: false,
          heightStyle: "content"
      })
      .sortable({
          axis: "y",
          handle: "div",
          containment: 'parent',
          tolerance: 'pointer',
          cursor: 'n-resize',
          items: "li:not(.treeignore)",
          distance: 10,
          stop: function (event, ui) {
              // IE doesn't register the blur when sorting
              // so trigger focusout handlers to remove .ui-state-focus
              ui.item.children("li").triggerHandler("focusout");

              // Refresh accordion to handle new order
              $(this).accordion("refresh");
          },
          update: function (event, ui) {

              updatePasswordPositition(event, ui);

          }

      });

});


//when a tree item is clicked, load its children
$('.treelistitem').on('click', function (event) {

    //make sure item isnt already open and hasnt already has its children loaded
    if (!$(this).hasClass('ui-state-active') && !$(this).has('ul').length)
        treeListItemClick(event, $(this));

});