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
        deferLoading: 0,
        language: {
            "emptyTable": "Data tidak ditemukan - Silahkan Filter data Steerco terlebih dahulu !"
        },
        ajax: {
            url: '/steerco_',
            method: 'POST',
            data: function (d) {
                d.project = $('#project_filter').val();
            }
        },
        columnDefs: [
            { className: 'text-center', targets: [5, 6] },
           /* (user_auth == 'user') ? { "visible": false, "targets": [6] } : {},*/
        ],
        columns: [
            {
                "data": null, orderable: false, "render": function (data, type, full, meta) {
                    return meta.row + 1;
                }
            },
            { data: 'judul', name: 'judul' },
            {
                data: 'tanggal', name: 'tanggal', render: function (data, type, full, meta) {
                    var date = new Date(full.created_date);
                    var string = date.getDate() + "-" + date.getMonth() + "-" + date.getFullYear();
                    return string;
                } },
            { data: 'nama_', name: 'nama_' },
            {
                data: 'created_date', name: 'created_date',
                render: function (data, type, full, meta) {
                    var date = new Date(full.created_date);
                    var string = date.getDate() + "-" + date.getMonth() + "-" + date.getFullYear();
                    return string;
                }
            },
            {
                "render": function (data, type, full, meta) {
                    return '<a href="' + full.materi + '" class="badge bg-info" target="blank_"><i class="far fa-copy"></i> file</a>';
                },
                orderable: false,
                searchable: false
            },
            {
                "render": function (data, type, full, meta) {
                    return '<a href="' + full.notulen + '" class="badge bg-info" target="blank_"><i class="far fa-copy"></i> file</a>';
                },
                orderable: false,
                searchable: false
            },
            {
                "render": function (data, type, full, meta) {
                    return '<div class="d-flex"><a href="javascript:void(0);" class="btn btn-warning  btn-xs edit mr-1" data-id="' + full.id + '"data-id_project="' + full.id_project + '" data-judul="' + full.judul + '" data-tanggal="' + full.tanggal + '" data-materi="' + full.materi + '" data-notulen="' + full.notulen + '" ><i class="fas fa-pen fa-xs"></i></a><a href = "javascript:void(0);" style = "margin-left:5px" class="btn btn-danger btn-xs delete " data-id="' + full.id + '" > <i class="fas fa-trash fa-xs"></i></a ></div > ';
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
                $('.judul-modal').text('Tambah Data Steerco');
                $('#hidden_status').val('add');
            }
        },

        {
            extend: 'excel',
            title: 'Steerco',
            className: 'btn',
            text: '<i class="far fa-file-code"></i> Excel',
            titleAttr: 'Excel',
            exportOptions: {
                columns: ':not(:last-child)',
            }
        },


        ] /*: [{
            extend: 'excel',
            title: 'Steerco',
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
        $('#title-steerco').html('Data Steerco ( ' + project + ' )');
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
        var tgl = $(this).data('tanggal').split("T");
        $('#hidden_status').val('edit');
        $('#hidden_id').val($(this).data('id'));
        $('#judul').val($(this).data('judul'));
        $('#id_project').val($(this).data('id_project'));
        $('#tanggal').val(tgl[0]);
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
                    url: "/delete_steerco",
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
                            showConfirmButton: false,
                            buttons: false,
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
                url = '/create_steerco';
            } else {
                url = '/update_steerco';
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
                            showConfirmButton: false,
                            buttons: false,
                        });
                        table.ajax.reload();
                    } else {
                        Swal.fire({
                            title: 'Berhasil',
                            icon: 'success',
                            timer: 3000,
                            showCancelButton: false,
                            showConfirmButton: false
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