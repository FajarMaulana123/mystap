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
            url: '/notifikasi_',
            method: 'POST',
        },
        columns: [
            { data: 'notifikasi', name: 'notifikasi' },
            { data: 'order', name: 'order' },
            { data: 'description', name: 'description' },
            { data: 'created_by', name: 'created_by' },
            { data: 'user_status', name: 'user_status' },
            { data: 'system_status', name: 'system_status' },
            { data: 'functional_location', name: 'functional_location' },
            { data: 'equipment', name: 'equipment' },
            { data: 'location', name: 'location' },
            { data: 'main_work_center', name: 'main_work_center' },
            {
                "render": function (data, type, full, meta) {
                    return '<a href="javascript:void(0);" class="btn text-primary rekomendasi " data-id="' + full.id + '"  data-rekomendasi="' + full.rekomendasi + '"><i class="far fa-copy"></i> file</a>';
                },
                orderable: false,
                searchable: false
            },
            // {
            //     data: 'action', 
            //     name: 'action', 
            //     orderable: false, 
            //     searchable: false
            // },
        ],
        columnDefs: [
            {
                targets: [3, 9],
                className: 'text-wrap width-200'


            },
           /* (user_auth == 'user') ? { "visible": false, "targets": [10] } : {},*/
        ],
        buttons: /*(user_auth == 'superadmin' || user_auth == 'admin') ?*/ [{
            text: '<i class="far fa-edit"></i> Import',
            className: 'btn btn-success',
            action: function (e, dt, node, config) {
               /* $('#import-form')[0].reset();*/
                $('#import').modal('show');
                $('.judul-modal').text('Import Data');
                $('#hidden_status').val('add');
            }
        },

        {
            extend: 'excel',
            title: 'Plant',
            className: 'btn',
            text: '<i class="far fa-file-code"></i> Excel',
            titleAttr: 'Excel',
            // exportOptions: {
            //     columns: ':not(:last-child)',
            // }
        },] /*: [{
            extend: 'excel',
            title: 'Plant',
            className: 'btn',
            text: '<i class="far fa-file-code"></i> Excel',
            titleAttr: 'Excel',
            // exportOptions: {
            //     columns: ':not(:last-child)',
            // }
        }]*/
    });
    // table.button( 0 ).nodes().css('height', '35px')
    $(document).on('click', '.rekomendasi', function () {
        $('#add-form')[0].reset();
        $('#Modal').modal('show');
        $('.judul-modal').text('Update Rekomendasi');
        // $('#hidden_status').val('');
        $('#hidden_id').val($(this).data('id'));
        $('#rekomendasi_').val($(this).data('rekomendasi'));
        if ($(this).data('rekomendasi')) {
            $('#lihat_file').html("<a href='" + $(this).data('rekomendasi') + "' target='_blank'>* Lihat File Rekomendasi</a>")
        } else {
            $('#lihat_file').html("");
        }
    })

    // $(document).on('click', '.delete', function() {
    //     var id = $(this).data('id');
    //     Swal({
    //         title: 'Apakah Anda Yakin?',
    //         type: 'warning',
    //         showCancelButton: true,
    //         confirmButtonColor: '#3085d6',
    //         confirmButtonText: 'Ya, Hapus!',
    //         cancelButtonText: 'Tidak',
    //     }).then((result) => {
    //         if (result.value) {
    //             $.ajax({
    //                 url: "/delete_notifikasi",
    //                 type: "POST",
    //                 data: {
    //                     id: id
    //                 },
    //                 dataType: "JSON",
    //                 success: function(data) {
    //                     table.ajax.reload();
    //                     Swal.fire({
    //                         title: data.title,
    //                         html: '<b>' + data.status + "</b>",
    //                         icon: data.icon,
    //                         timer: 3000,
    //                         showCancelButton: false,
    //                         showConfirmButton: false,
    //                         buttons: false,
    //                     });
    //                 },
    //                 error: function(jqXHR, textStatus, errorThrown) {
    //                     alert('Error');
    //                 }
    //             });
    //         }
    //     })
    // });

    $("#add-form").validate({
        errorClass: "is-invalid",
        // validClass: "is-valid",
        rules: {
            rekomendasi: {
                required: true
            }

        },
        submitHandler: function (form) {
            let url;
            url = '/update_rekomendasi';

            $.ajax({
                url: url,
                type: "POST",
                data: new FormData(document.getElementById("add-form")),
                dataType: "JSON",
                contentType: false,
                cache: false,
                processData: false,
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

    $("#import-form").validate({
        errorClass: "is-invalid",
        // validClass: "is-valid",
        rules: {
            file: {
                required: true
            }

        },
        submitHandler: function (form) {
            let url;
            url = '/import_notifikasi';

            $.ajax({
                url: url,
                type: "POST",
                data: new FormData(document.getElementById("import-form")),
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
                            text: "Gagal Import Notifikasi",
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
                            showConfirmButton: true
                        });
                        $('#import').modal('hide');
                        table.ajax.reload();
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert('Error adding / update data');
                }
            });
            $(document).ready(function () {
                $("#GridView1").prepend($("<thead></thead>").append($(this).find("tr:first"))).dataTable();
            });  
        }
    });

});