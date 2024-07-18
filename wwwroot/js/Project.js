﻿$(document).ready(function () {

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
            url: '/project_',
            method: 'POST',
            data: function (d) {
                d.status = $('#status_filter').val();
                d.taoh = $('#taoh_filter').val();
            }
        },

        columns: [
            { data: 'DT_RowIndex', name: 'DT_RowIndex', orderable: false, searchable: false },
            { data: 'projectNo', name: 'projectNo' },
            { data: 'description', name: 'description' },
            { data: 'month', name: 'month' },
            { data: 'year', name: 'year' },
            { data: 'date', name: 'date' },
            { data: 'revision', name: 'revision' },
            { data: 'status', name: 'status' },
            { data: 'durasiTABrick', name: 'durasiTABrick' },
            { data: 'taoh', name: 'taoh' },

            {
                data: 'action',
                name: 'action',
                orderable: false,
                searchable: false
            },
        ],
        columnDefs: [
            {
                targets: [2, 5, 9],
                className: 'text-wrap width-200'

            },
            (user_auth == 'user') ? { "visible": false, "targets": [10] } : {},
        ],
        buttons: (user_auth == 'superadmin' || user_auth == 'admin') ? [{
            text: '<i class="far fa-edit"></i> New',
            className: 'btn btn-success',
            action: function (e, dt, node, config) {
                $('#add-form')[0].reset();
                $('#Modal').modal('show');
                $('#btn-sb').text('Tambah');
                $('.judul-modal').text('Tambah Project');
                $('#hidden_status').val('add');
            }
        },
        {
            extend: 'excel',
            title: 'Project',
            className: 'btn',
            text: '<i class="far fa-file-code"></i> Excel',
            titleAttr: 'Excel',
            exportOptions: {
                columns: ':not(:last-child)',
            }
        }] : [{
            extend: 'excel',
            title: 'Project',
            className: 'btn',
            text: '<i class="far fa-file-code"></i> Excel',
            titleAttr: 'Excel',
            exportOptions: {
                columns: ':not(:last-child)',
            }
        }]

    });
    // table.button( 0 ).nodes().css('height', '35px')

   /* $(document).on('click', '#tambah', function () {
        $('#add-form')[0].reset();
        $('.judul-modal').html('Tambah');
        $('#hidden_status').val('add');
    })

    $(document).on('click', '.status', function () {
        var id = $(this).data('id');
        Swal.fire({
            title: 'Apakah Anda Yakin?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            confirmButtonText: 'Ya!',
            cancelButtonText: 'Tidak',
        }).then((result) => {
            if (result.value) {
                $.ajax({
                    url: "/status_project",
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
                            icon: data.icon,
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
    })

    $(document).on('click', '.edit', function () {
        $('#add-form')[0].reset();
        $('#Modal').modal('show');
        $('.judul-modal').text('Update');
        var year = $(this).data('year') + '-' + $(this).data('month');
        $('#hidden_status').val('edit');
        $('#hidden_id').val($(this).data('id'));
        $('#plant').val($(this).data('plant'));
        $('#year').val(year);
        $('#execution_date').val($(this).data('execution_date'));
        $('#finish_date').val($(this).data('finish_date'));
        $('#revision').val($(this).data('revision'));
        $('#description').val($(this).data('description'));
        $('#durasiTABrick').val($(this).data('durasitabrick'));
        $('#section').val($(this).data('section'));

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
                    url: "/delete_project",
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
                            icon: data.icon,
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
            plant: {
                required: true
            },
            year: {
                required: true
            },
            execution_date: {
                required: true
            },
            finish_date: {
                required: true
            },
            revision: {
                required: true
            },
            description: {
                required: true
            },
            durasiTABrick: {
                required: true
            },
            section: {
                required: true
            }


        },
        submitHandler: function (form) {
            let url;
            if ($('#hidden_status').val() == 'add') {
                url = '/create_project';
            } else {
                url = '/update_project';
            }

            var form_data = new FormData(document.getElementById("add-form"));
            form_data.append('kode_plant', $('#plant').find(':selected').attr('data-plant'));

            $.ajax({
                url: url,
                type: "POST",
                data: form_data,
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
                            showCancelButton: false,
                            showConfirmButton: true,
                            // buttons: false,
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

    $(document).on('click', '#filter', function () {
        table.ajax.reload();
    })*/

});