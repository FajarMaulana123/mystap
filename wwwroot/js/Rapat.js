$(document).ready(function () {

    var table = $('#table').DataTable({
        dom: '<"dataTables_wrapper dt-bootstrap"<"row"<"col-xl-7 d-block d-sm-flex d-xl-block justify-content-center"<"d-block d-lg-inline-flex me-0 me-md-3"l><"d-block d-lg-inline-flex"B>><"col-xl-5 d-flex d-xl-block justify-content-center"fr>>t<"row"<"col-md-5"i><"col-md-7"p>>>',
        processing: true,
        serverSide: true,
        /*deferLoading: 0,
        language: {
            "emptyTable": "Data tidak ditemukan - Silahkan Filter data Rapat terlebih dahulu !"
        },*/
        ajax: {
            "url": "/rapat_",
            "type": "POST",
            "datatype": "json"
           
        },
       /* columnDefs: [
            { className: 'text-center', targets: [4, 5] },
            (user_auth == 'user') ? { "visible": false, "targets": [6] } : {},
        ],*/
        columns: [
            { data: 'judul', name: 'judul' },
            { data: 'tanggal', name: 'tanggal' },
            { data: 'name', name: 'name' },
            { data: 'created_date', name: 'created_date' },
            { data: 'materi', name: 'materi' },
            { data: 'notulen', name: 'notulen' },
            {
                "render": function (data, type, full, meta) {
                    return '<div class="d-flex"><a href="javascript:void(0);" class="btn btn-warning  btn-xs edit mr-1" data-id="' + full.id + '" data-project="' + full.id_project + '" data-tanggal="' + full.tanggal + '" data-judul="' + full.judul + '" data-materi="' + full.materi + '" data-notulen="' + full.notulen + '" ><i class="fas fa-pen fa-xs"></i></a><a href = "javascript:void(0);" style = "margin-left:5px" class="btn btn-danger btn-xs delete " data-id="' + full.id + '" > <i class="fas fa-trash fa-xs"></i></a ></div > ';
                },
                orderable: false,
                searchable: false
            },
        ],
        buttons: /*(user_auth == 'superadmin' || user_auth == 'admin') ?*/ [{
            text: '<i class="far fa-edit"></i> New',
            className: 'btn btn-success',
            action: function (e, dt, node, config) {
                $('#add-form')[0].reset();
                $('#Modal').modal('show');
                $('#btn-sb').text('Tambah');
                $('.judul-modal').text('Tambah Data Rapat');
                $('#hidden_status').val('add');
            }
        },

        {
            extend: 'excel',
            title: 'Data Rapat',
            className: 'btn',
            text: '<i class="far fa-file-code"></i> Excel',
            titleAttr: 'Excel',
            exportOptions: {
                columns: ':not(:last-child)',
            }
        },


        ] /*: [{
            extend: 'excel',
            title: 'Data Rapat',
            className: 'btn',
            text: '<i class="far fa-file-code"></i> Excel',
            titleAttr: 'Excel',
            exportOptions: {
                columns: ':not(:last-child)',
            }
        }]*/

    });
    // table.button( 0 ).nodes().css('height', '35px')

    $(document).on('click', '#filter', function () {
        var project = $('#project_filter').find(':selected').data('desc');
        $('#title-rapat').html('Data Rapat ( ' + project + ' )');
        table.ajax.reload();
    })

    $(document).on('click', '#tambah', function () {
        $('#add-form')[0].reset();
        $('.judul-modal').text('Tambah');
        $('#hidden_status').val('add');
    })

    $(document).on('click', '.edit', function () {
        $('#add-form')[0].reset();
        $('#Modal').modal('show');
        $('#btn-sb').text('Update');
        $('.judul-modal').text('Edit Steerco');
        $('#hidden_status').val('edit');
        $('#hidden_id').val($(this).data('id'));
        $('#judul').val($(this).data('judul'));
        $('#id_project').val($(this).data('id_project'));
        $('#tanggal').val($(this).data('tanggal'));
        $('#materi_').val($(this).data('materi'));
        $('#notulen_').val($(this).data('notulen'));
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
                    url: "/delete_rapat",
                    type: "POST",
                    data: {
                        id: id
                    },
                    dataType: "JSON",
                    success: function (data) {
                        table.ajax.reload();
                        Swal.fire({
                            title: data.title,
                            text: data.status,
                            type: data.icon,
                            timer: 3000,
                            showCancelButton: false,
                            showConfirmButton: true,
                            /*buttons: false,*/
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
            project_id: {
                required: true
            },
            judul: {
                required: true
            },
            tanggal: {
                required: true
            }

        },
        submitHandler: function (form) {
            let url;
            if ($('#hidden_status').val() == 'add') {
                url = '/create_rapat';
            } else {
                url = '/update_rapat';
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
                            timer: 3000,
                            showCancelButton: false,
                            showConfirmButton: true,
                            /*buttons: false,*/
                        });
                        table.ajax.reload();
                    } else {
                        Swal.fire({
                            title: 'Berhasil',
                            icon: 'success',
                            showCancelButton: false,
                            showConfirmButton: true
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