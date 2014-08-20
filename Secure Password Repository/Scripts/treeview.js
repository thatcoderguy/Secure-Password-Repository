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
          handle: "li",
          containment: 'parent',
          tolerance: 'pointer',
          cursor: 'n-resize',
          items: ":not(.treeignore)",
          distance: 10,
          stop: function (event, ui) {
              // IE doesn't register the blur when sorting
              // so trigger focusout handlers to remove .ui-state-focus
              ui.item.children("li").triggerHandler("focusout");

              // Refresh accordion to handle new order
              $(this).accordion("refresh");
          },
          update: function (event, ui) {
    
              var categoryid = ui.item.data('id');
              var newposition = ui.item.index() + 1;

              var result = $.ajax({
                  type: "POST",
                  url: "/Password/UpdateCategoryPosition",
                  data: { CategoryId: categoryid, NewPosition: newposition },
                  contentType: "application/x-www-form-urlencoded; charset=utf-8",
                  dataType: "json",
                  headers: {
                      'RequestVerificationToken': '@TokenHeaderValue()'
                  },
                  success: function (data) {
                      
                  },
                  failure: function (msg) {
                      alert('Sorry it looks like something went wrong, please press F5 - if you keep getting this error, please contact support');
                      return false;
                  },
                  error: function (xhr, err) {
                      alert('Sorry it looks like something went wrong, please press F5 - if you keep getting this error, please contact support');
                      return false;
                  }
              });
          }
      });

    //auto open "root"
    $('#ui-id-1').removeClass('ui-corner-all').addClass('ui-accordion-header-active').addClass('ui-state-active').addClass('ui-corner-top').find('span').removeClass('treeviewplus').addClass('treeviewminus');
    $('#ui-id-2').show();

});
