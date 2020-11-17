var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#dt').DataTable({
        "ajax": {
            "url": "/item/getall",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            {
                "data": "icon",
                "width": "10%",
                "render": function (data) {
                    return `<img src="/images/${data}"/>`;
                }
            },
            {
                "data": "name",
                "width": "15%"
            },
            {
                "data": "price",
                "width": "15%"
            },
            {
                "data": "description",
                "width": "40%"
            },
            {
                "data": "id",
                "width": "10%",
                "render": function (data) {
                    return `<div class="text-center">
                        <a href="/item/Edit?id=${data}" class='btn btn-success text-white' style='cursor:pointer; width:70px;'>
                            Edit
                        </a>
                        </div>`;
                }
            },
            {
                "data": "id",
                "width": "10%",
                "render": function (data) {
                    return `<div class="text-center">
                        <a class='btn btn-danger text-white' style='cursor:pointer; width:70px;'
                            onclick=Delete('/item/Delete?id='+${data})>
                            Delete
                        </a>
                        </div>`;
                }
            }
        ],
        "language": {
            "emptyTable": "no data found"
        },
        "width": "100%"
    })
}

function Delete(url) {
    swal({
        title: "Are you sure?",
        text: "Once deleted, you will not be able to recover",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}