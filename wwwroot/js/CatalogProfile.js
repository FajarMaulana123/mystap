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
            url: 'catalog_profile_',
            method: 'POST',
        },
        columns: [
           /* { data: 'DT_RowIndex', name: 'DT_RowIndex', orderable: false, searchable: false },*/
            {
                "data": null, orderable: false, "render": function (data, type, full, meta) {
                    return meta.row + 1;
                }
            },
            { data: 'code', name: 'code' },

            { data: 'equipment_class', name: 'equipment_class' },
            { data: 'equipment_group', name: 'equipment_group' },
            { data: 'disiplin', name: 'disiplin' },
            { data: 'long_description', name: 'long_description' },
            {
                "render": function (data, type, full, meta) {
                    return '<div class="d-flex"><a href="javascript:void(0);" class="btn btn-warning  btn-xs edit mr-1" data-id="' + full.id + '" data-code="' + full.code + '" data-disiplin="' + full.disiplin + '" data-equipment_class="' + full.equipment_class + '" data-equipment_group="' + full.equipment_group + '" data-long_description="' + full.long_description + '" ><i class="fas fa-pen fa-xs"></i></a><a href = "javascript:void(0);" style = "margin-left:5px" class="btn btn-danger btn-xs delete " data-id="' + full.id + '" > <i class="fas fa-trash fa-xs"></i></a ></div > ';
                },
                orderable: false,
                searchable: false
            },
        ],
        //columnDefs: [
        //    (user_auth == 'user') ? { "visible": false, "targets": [6] } : {},
        //],
        buttons:/* (user_auth == 'superadmin' || user_auth == 'admin') ? */[{
            text: '<i class="far fa-edit"></i> New',
            className: 'btn btn-success',
            action: function (e, dt, node, config) {
                $('#add-form')[0].reset();
                $('#Modal').modal('show');
                $('#btn-sb').text('Tambah');
                $('.judul-modal').text('Tambah Catalog Profile');
                $('#hidden_status').val('add');
            }
        },

        {
            extend: 'excel',
            title: 'Catalog Profile',
            className: 'btn',
            text: '<i class="far fa-file-code"></i> Excel',
            titleAttr: 'Excel',
            exportOptions: {
                columns: ':not(:last-child)',
            }
        },
        ] /*: [{
            extend: 'excel',
            title: 'Catalog Profile',
            className: 'btn',
            text: '<i class="far fa-file-code"></i> Excel',
            titleAttr: 'Excel',
            exportOptions: {
                columns: ':not(:last-child)',
            }
        }]*/
    });
    // table.button( 0 ).nodes().css('height', '35px')

    $(document).on('click', '#tambah', function () {
        $('#add-form')[0].reset();
        $('.judul-modal').text('Tambah');
        $('#hidden_status').val('add');
    })

    $(document).on('click', '.edit', function () {
        $('#add-form')[0].reset();
        $('#Modal').modal('show');
        $('#btn-sb').text('Update');
        $('.judul-modal').text('Edit Catalog Profile');
        $('#hidden_status').val('edit');
        $('#hidden_id').val($(this).data('id'));
        $('#code').val($(this).data('code'));
        $('#disiplin').val($(this).data('disiplin'));
        $('#equipment_class').val($(this).data('equipment_class'));
        $('#equipment_group').val($(this).data('equipment_group'));
        $('#long_description').val($(this).data('long_description'));
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
                    url: "delete_catalog_profile",
                    type: "POST",
                    data: {
                        id: id
                    },
                    dataType: "JSON",
                    success: function (data) {
                        table.ajax.reload();
                        Swal.fire({
                            title: data.title,
                            html: '<b>' + data.status + "</b>",
                            icon: data.icon,
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
            code: {
                required: true
            },
            equipment_class: {
                required: true
            },
            equipment_group: {
                required: true
            },
            disiplin: {
                required: true
            },
            long_description: {
                required: true
            }


        },
        submitHandler: function (form) {
            let url;
            if ($('#hidden_status').val() == 'add') {
                url = 'create_catalog_profile';
            } else {
                url = 'update_catalog_profile';
            }
            // var form_data=new FormData(document.getElementById("add-form"));
            // form_data.append('disiplin',$('#disiplin').find(':selected').attr('data-disiplin'))
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

});