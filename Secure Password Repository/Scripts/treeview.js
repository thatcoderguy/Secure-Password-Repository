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
          heightStyle: "content"
      })
      .sortable({
          axis: "y",
          handle: "h3",
          stop: function (event, ui) {
              // IE doesn't register the blur when sorting
              // so trigger focusout handlers to remove .ui-state-focus
              ui.item.children("li").triggerHandler("focusout");

              // Refresh accordion to handle new order
              $(this).accordion("refresh");
          }
      });

    $('#ui-id-1').removeClass('ui-corner-all').addClass('ui-accordion-header-active').addClass('ui-state-active').addClass('ui-corner-top');
    $('#ui-id-2').show();
    
});
