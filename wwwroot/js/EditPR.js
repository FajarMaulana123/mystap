var editor;
function get_order(order) {
    $.ajax({
        url: "/get_work_order",
        method: "POST",
        data: {
            order: order
        },
        success: function (res) {
            $('#info_equipment').html(res.data.equipment);
            $('#info_revision').html(res.data.revision);
            $('#info_desc').html(res.data.description);
            $('#info_sys_stat').html(res.data.system_status);
            $('#info_user_stat').html(res.data.user_status);
        }
    })
}

// function get_material(order){
//     $.ajax({
//         url: "/get_material",
//         method: "POST",
//         data: {
//             order: order
//         },
//         success: function(res){
//             console.log(res);
//         }
//     })
// }

$(document).ready(function () {
    $.ajaxSetup({
        headers: {
            'X-CSRF-TOKEN': $('meta[name="csrf-token"]').attr('content')
        }
    });

    var wo = "";
    //var pr = "@(ViewBag.pr)";
    //var itm_pr = "@(ViewBag.item_pr)";
    //var check = "@(ViewBag.check)";
    

    var table = $('#table').DataTable({
        // dom: '<"dataTables_wrapper dt-bootstrap"<"row"<"col-xl-7 d-block d-sm-flex d-xl-block justify-content-center"<"d-block d-lg-inline-flex me-0 me-md-3"l><"d-block d-lg-inline-flex"B>><"col-xl-5 d-flex d-xl-block justify-content-center"fr>>t<"row"<"col-md-5"i><"col-md-7"p>>>',
        processing: true,
        serverSide: true,
        ajax: {
            url: '/get_material_',
            method: 'POST',
            data: function (d) {
                d.no_order = (wo == '') ? $('#txt_order').val() : wo;
                //d.pr = pr;
                //d.itm_pr = itm_pr;
                //d.check = check;
            }
        },
        deferLoading: 0,
        language: {
            "emptyTable": "Data tidak ditemukan - Silahkan Cari data No Order Reservasi terlebih dahulu !"
        },
        columns: [
            // { data: 'DT_RowIndex', name: 'DT_RowIndex', orderable: false, searchable: false },
            {
                data: 'action', name: 'action',
                "render": function (data, type, full, meta) {
                    var actionBtn = '<div class="d-flex"><a href="javascript:void(0);" class="btn btn-warning  btn-xs edit mr-1" data-id="' + full.id + '" data-order="' + full.order + '" data-material="' + full.material + '" data-itm="' + full.itm + '" data-pr="' + full.pr + '" data-pr_item="' + full.pr_item + '" data-qty_pr="' + full.pr_qty + '" data-stock="' + full.qty_res + '"><i class="fas fa-pen fa-xs"></i></a>';
                    return actionBtn;
                },

                
            },
            { data: 'material', name: 'material', },
            { data: 'description', name: 'description' },
            { data: 'itm', name: 'itm' },
            { data: 'reqmt_qty', name: 'reqmt_qty' },
            { data: 'qty_res', name: 'qty_res' },
            { data: 'bun', name: 'bun' },
            { data: 'reqmt_date', name: 'reqmt_date' },
            { data: 'pr', name: 'pr' },
            { data: 'pr_item', name: 'pr_item' },
            { data: 'pr_qty', name: 'pr_qty' },
            { data: 'pg', name: 'pg' },
            {
                data: 'status_qty', name: 'status_qty',
                render: function (data, type, full, meta) {
                    var s = "";
                    if (full.status_qty == 'balance') {
                        s = '<span class="badge bg-green-300 shadow-none">Balance</span>';
                    } else if (full.status_qty == 'not_balance') {
                        s = '<span class="badge bg-red-300 shadow-none">Not Balance</span>';
                    } else if (full.status_qty == 'fis') {
                        s = '<span class="badge bg-orange-300 shadow-none">FIs</span>';
                    } else {
                        s = '<span class="badge bg-gray-300 shadow-none">undefined</span>';
                    }

                    var status = s;
                    return status;
                }
            },
        ],
        columnDefs: [
            {
                targets: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12],
                orderable: false,
                searchable: false
            },
            //(pr != '') ? { "visible": false, "targets": [12] } : {},
            (role_ == 'user') ? { "visible": false, "targets": [0] } : {},
        ],

        order: [],

    });

    $(document).on('click', '.edit', function () {
        $('#edit-reservasi-form')[0].reset();

        $('#edit-reservasi').modal('show');
        $('.judul-modal').text('Update PR');
        // $('#hidden_status').val('edit');
        $('#hidden_order').val($(this).data('order'));
        $('#hidden_material').val($(this).data('material'));
        $('#hidden_itm').val($(this).data('itm'));
        $('#hidden_id').val($(this).data('id'));
        $('#pr').val($(this).data('pr'));
        $('#pr_item').val($(this).data('pr_item'));
        $('#qty_pr').val($(this).data('qty_pr'));
        $('#stock').val($(this).data('stock'));

    });

    if (wo != '') {
        var order = wo;
        get_order(order);
        table.ajax.reload();
    }

    $(document).on('click', '#cari', function () {
        var order = $('#txt_order').val();
        get_order(order);
        table.ajax.reload();
        // get_material(order);
    })

    $("#edit-reservasi-form").validate({
        errorClass: "is-invalid",
        // validClass: "is-valid",
        rules: {
        },
        submitHandler: function (form) {
            let url = '/update_pr';
            $.ajax({
                url: url,
                type: "POST",
                data: new FormData(document.getElementById("edit-reservasi-form")),
                dataType: "JSON",
                contentType: false,
                cache: false,
                processData: false,
                beforeSend: function () {
                    swal.fire({
                        title: 'Harap Tunggu!',
                        allowEscapeKey: false,
                        allowOutsideClick: false,
                        showCancelButton: false,
                        showConfirmButton: false,
                        buttons: false,
                        didOpen: () => {
                            Swal.showLoading()
                        }
                    })
                },
                success: function (data) {
                    if (data.result != true) {
                        Swal.fire({
                            title: data.title,
                            text: data.text,
                            icon: data.icon,
                            // timer: 3000,
                            showCancelButton: false,
                            showConfirmButton: true,
                            // buttons: false,
                        });
                        table.ajax.reload();
                    } else {
                        Swal.fire({
                            title: data.title,
                            // text: data.text,
                            icon: data.icon,
                            // timer: 3000,
                            showCancelButton: false,
                            showConfirmButton: true,
                            // buttons: false,
                        });
                        $('#edit-reservasi').modal('hide');
                        table.ajax.reload();
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert('Error adding / update data');
                }
            });
        }
    });
})