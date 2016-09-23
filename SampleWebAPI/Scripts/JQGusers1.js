$(function () {
    $("#grid").jqGrid({
        url: "/JQGUsers/GetUsersLists",
        datatype: 'json',
        mtype: 'Get',
        colNames: ['uid', 'Firs Name', 'Last Name']//,'DOB','Gender','Active'],
        colModel: [
            { key: true, hidden: true, name: 'Id', index: 'Id', editable: true },
            { key: false, name: 'First Name', index: 'firstName', editable: true },
            { key: false, name: 'Last Name', index: 'lastName', editable: true }
			//, { key: false, name: 'DOB', index: 'DOB', editable: true, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            //{ key: false, name: 'Gender', index: 'Gender', editable: true, edittype: 'select', editoptions: { value: { 'F': 'Female', 'M': 'Male' } } },
            //{ key: false, name: 'Active', index: 'Active', editable: true, edittype: 'select', editoptions: { value: { 'A': 'Active', 'I': 'InActive' } } }
			],
        pager: jQuery('#pager'),
        rowNum: 10,
        rowList: [10, 20, 30, 40],
        height: '100%',
        viewrecords: true,
        caption: 'Users',
        emptyrecords: 'No records to display',
        jsonReader: {
            root: "rows",
            page: "page",
            total: "total",
            records: "records",
            repeatitems: false,
            Id: "0"
        },
        autowidth: true,
        multiselect: false
    }).navGrid('#pager', { edit: true, add: true, del: true, search: false, refresh: true },
        {
            // edit options
            zIndex: 100,
            url: '/JQGUser/Edit',
            closeOnEscape: true,
            closeAfterEdit: true,
            recreateForm: true,
            afterComplete: function (response) {
                if (response.responseText) {
                    alert(response.responseText);
                }
            }
        },
        {
            // add options
            zIndex: 100,
            url: "/JQGUser/Create",
            closeOnEscape: true,
            closeAfterAdd: true,
            afterComplete: function (response) {
                if (response.responseText) {
                    alert(response.responseText);
                }
            }
        },
        {
            // delete options
            zIndex: 100,
            url: "/JQGUser/Delete",
            closeOnEscape: true,
            closeAfterDelete: true,
            recreateForm: true,
            msg: "Are you sure you want to delete this task?",
            afterComplete: function (response) {
                if (response.responseText) {
                    alert(response.responseText);
                }
            }
        });
});