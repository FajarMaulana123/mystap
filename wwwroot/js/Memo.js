$(document).ready(function () {

    $.ajaxSetup({
        headers: {
            'X-CSRF-TOKEN': $('meta[name="csrf-token"]').attr('content')
        }
    });

    var table = $('#table').DataTable({
        dom: '<"dataTables_wrapper dt-bootstrap"<"row"<"col-xl-7 d-block d-sm-flex d-xl-block justify-content-center"<"d-block d-lg-inline-flex me-0 me-md-3"l><"d-block d-lg-inline-flex"B>><"col-xl-5 d-flex d-xl-block justify-content-center"fr>>t<"row"<"col-md-5"i><"col-md-7"p>>>',
        processing: true,
        serverSide: true,
        ajax: {
            url: '/request_memo_',
            method: 'POST',
            data: function (d) {
                d.project_filter = $('#project_filter').val();
                d.memo = $('#memo_filter').val();
            }
        },
        //columnDefs: [
        //    (user_auth == 'user') ? { "visible": false, "targets": [8] } : {},
        //],
        columns: [
            /*{ data: 'DT_RowIndex', name: 'DT_RowIndex', orderable: false, searchable: false },*/
            { data: 'reqNo', name: 'reqNo' },
            { data: 'reqDesc', name: 'reqDesc' },
            { data: 'reqDate', name: 'reqDate' },
            { data: 'projectName', name: 'project.description' },
            { data: 'requestorName', name: 'requestor.name', },
            { data: 'file', name: 'file' },
            {
                data: 'show', name: 'show',
                render: function (data, type, full, meta) {

                    d = (full.show == 1) ? '<span class="badge bg-success">Active</span>' : '<span class="badge bg-danger">In Active</span>';
                    full.show = d;
                    return full.show;
                },
            },
            { data: 'createBy', name: 'users.alias' },
            {
                "render": function (data, type, full, meta) {
                    return '<div class="d-flex"><a href="javascript:void(0);" class="btn btn-warning  btn-xs edit mr-1" data-id="' + full.id + '" data-projectID="' + full.projectID + '" data-reqNo="' + full.reqNo + '" data-reqDate="' + full.reqDate + '" data-reqDesc="' + full.reqDesc + '" data-attach="' + full.attach + '" data-requestor="' + full.requestors + '" data-showing="' + full.showing + '"><i class="fas fa-pen fa-xs"></i></a><a href = "javascript:void(0);" style = "margin-left:5px" class="btn btn-danger btn-xs delete " data-id="' + full.id + '" > <i class="fas fa-trash fa-xs"></i></a ></div > ';
                },
                orderable: false,
                searchable: false
            },
        ],
        buttons: /*(user_auth == 'superadmin' || user_auth == 'admin') ? */[{
            text: '<i class="far fa-edit"></i> New',
            className: 'btn btn-success',
            action: function (e, dt, node, config) {
                $('#add-form')[0].reset();
                $('#Modal').modal('show');
                $('#btn-sb').text('Tambah');
                $('.judul-modal').text('Tambah Request Memo');
                $('#hidden_status').val('add');
            }
        },

        {
            extend: 'excel',
            title: 'Requestor',
            className: 'btn',
            text: '<i class="far fa-file-code"></i> Excel',
            titleAttr: 'Excel',
            exportOptions: {
                columns: ':not(:last-child)',
            }
        },
        ] /*: [{
            extend: 'excel',
            title: 'Request Memo',
            className: 'btn',
            text: '<i class="far fa-file-code"></i> Excel',
            titleAttr: 'Excel',
            exportOptions: {
                columns: ':not(:last-child)',
            }
        }]*/

    });

    $(document).on('click', '.edit', function () {
        $('#add-form')[0].reset();
        $('#Modal').modal('show');
        $('#btn-sb').text('Update');
        $('.judul-modal').text('Edit Request Memo');
        $('#hidden_status').val('edit');
        $('#hidden_id').val($(this).data('id'));
        $('#projectID').val($(this).data('projectid'));
        $('#reqNo').val($(this).data('reqno'));
        $('#reqDesc').val($(this).data('reqdesc'));
        $('#reqDate').val($(this).data('reqdate'));
        $('#requestor').val($(this).data('requestor'));
        $('#showing').val($(this).data('showing'));
        $('#attach_').val($(this).data('attach'));
    });


    $(document).on('click', '.delete', function () {
        var id = $(this).data('id');
        Swal.fire({
            title: 'Apakah Anda Yakin?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            confirmButtonText: 'Ya, Hapus!',
            cancelButtonText: 'Tidak',
        }).then((result) => {
            if (result.value) {
                $.ajax({
                    url: "delete_request_memo",
                    type: "POST",
                    data: {
                        id: id
                    },
                    dataType: "JSON",
                    success: function (data) {
                        table.ajax.reload();
                        Swal.fire({
                            title: data.title,
                            text: '<b>' + data.status + "</b>",
                            type: data.icon,
                            // timer: 3000,
                            showCancelButton: false,
                            showConfirmButton: true,
                            // buttons: false,
                        });
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        alert('Error');
                    }
                });
            }
        })
    });

    $("#add-form").validate({
        errorClass: "is-invalid",
        // validClass: "is-valid",
        rules: {
            projectID: {
                required: true
            },
            reqNo: {
                required: true
            },
            reqDesc: {
                required: true
            },
            reqDate: {
                required: true
            },
            requestor: {
                required: true
            },
            // attach: {
            //     required: true
            // }

        },
        submitHandler: function (form) {
            let url;
            if ($('#hidden_status').val() == 'add') {
                url = '/create_request_memo';
            } else {
                url = '/update_request_memo';
            }
            $.ajax({
                url: url,
                type: "POST",
                data: new FormData(document.getElementById("add-form")),
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
                        timer: 2000,
                        didOpen: () => {
                            Swal.showLoading()
                        }
                    })
                },
                success: function (data) {
                    if (data.result != true) {
                        Swal.fire({
                            title: 'Gagal',
                            text: "Gagal Tambah / Update User",
                            icon: 'error',
                            // timer: 3000,
                            showCancelButton: false,
                            showConfirmButton: true,
                            // buttons: false,
                        });
                        table.ajax.reload();
                    } else {
                        Swal.fire({
                            title: 'Berhasil',
                            icon: 'success',
                            // timer: 3000,
                            showCancelButton: false,
                            showConfirmButton: true,
                            // buttons: false,
                        });
                        $('#Modal').modal('hide');
                        table.ajax.reload();
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert('Error adding / update data');
                }
            });
        }
    });

    $(document).on('click', '#filter', function () {
        table.ajax.reload();
    })

});