//display the new category form
function createNewCategory(holderid) {

    //generate a random number, so the multiple forms can be on the same page
    var random = parseInt(Math.random() * 1000).toString();
    //grab the parent category id
    var parentid = holderid.split('-')[1];

    $(holderid).html(strNewCategoryForm.replace('[1]', random).replace('[2]', random).replace('[3]', parentid));

}

function saveNewCategory(formid) {
    $(formid).submit();
}

//load children of the category clicked on
function treeListItemClick(event, listItem) {

    var result = $.ajax({
        type: "POST",
        url: "/Password/GetCategoryChildren",
        data: AddAntiForgeryToken({ ParentCategoryId: listItem.data('id') }),
        contentType: "application/x-www-form-urlencoded; charset=utf-8",
        dataType: "json",
        success: function (data) {

            var itemList = '<ul class="treeview">';

            if(data.CategoryItems.length > 0) {

                for(i = 0; i < data.CategoryItems.length; i++)
                {
                    itemList += strCategoryItem.replace('[1]', data.CategoryItems[i].CategoryId).replace('[2]', data.CategoryItems[i].CategoryName);
                }

            } else if (data.PasswordItems.length > 0) {


                for (i = 0; i < data.PasswordItems.length; i++) {
                    //itemList += strCategoryItem.replace('[1]', data.CategoryItems[i].CategoryId).replace('[2]', data.CategoryItems[i].CategoryName);
                }

            }

            itemList += strAddNewCategoryButton.replace('[1]', listItem.data('id')).replace('[2]', listItem.data('id'));
            itemList += '</ul>';

            listItem.append(itemList)
            //

            //add new category button
            //addNewCategoryButton.replace('[1]','').replace('[2]','');

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

//update the ordering position of the category
function updateCategoryPosition(event, ui)
{

    var categoryid = ui.item.data('id');
    var newposition = ui.item.index() + 1;

    var result = $.ajax({
        type: "POST",
        url: "/Password/UpdateCategoryPosition",
        data: AddAntiForgeryToken({ CategoryId: categoryid, NewPosition: newposition }),
        contentType: "application/x-www-form-urlencoded; charset=utf-8",
        dataType: "json",
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

//update the ordering position of the password
function updatePasswordPositition(event, ui)
{

    var passwordid = ui.item.data('id');
    var newposition = ui.item.index() + 1;

    var result = $.ajax({
        type: "POST",
        url: "/Password/UpdatePasswordPosition",
        data: AddAntiForgeryToken({ PasswordId: passwordid, NewPosition: newposition }),
        contentType: "application/x-www-form-urlencoded; charset=utf-8",
        dataType: "json",
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