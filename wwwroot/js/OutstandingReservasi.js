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
            url: '/outstanding_reservasi_',
            method: 'POST',
            data: function (d) {
                d.project_filter = $('#project_filter').val();
                d.lldi_filter = $('#lld_filter').val();
                d.taoh_filter = $('#taoh_filter').val();
            }
        },
        deferLoading: 0,
        language: {
            "emptyTable": "Data tidak ditemukan - Silahkan Filter data Outstanding Reservasi terlebih dahulu !"
        },
        "autoWidth": true,
        "lengthMenu": [
            [10, 25, 50, 100, -1],
            [10, 25, 50, 100, "All"]
        ],

        columns: [
            { data: 'order', name: 'order' },
            { data: 'reserv_no', name: 'reserv_no' },
            { data: 'revision', name: 'revision' },
            { data: 'material', name: 'material' },
            { data: 'material_desc', name: 'material_desc' },
            { data: 'itm', name: 'itm' },
            { data: 'reqmt_qty', name: 'reqmt_qty', searchable: false },
            { data: 'bun', name: 'bun' },
            { data: 'reqmt_date', name: 'reqmt_date' },
            { data: 'pr', name: 'pr', searchable: false },
            { data: 'pr_item', name: 'pr_item', searchable: false },
            { data: 'pr_qty', name: 'pr_qty', searchable: false },
            { data: 'pg', name: 'pg' },
        ],
        "order": [],
        buttons: (role_ == 'superadmin' || role_ == 'admin') ? [
            {
                text: '<i class="far fa-edit"></i>Upload PR',
                className: 'btn btn-warning',
                action: function (e, dt, node, config) {
                    $('#import-form')[0].reset();
                    $('#import').modal('show');
                    $('.judul-modal').text('Import Data');
                    $('#hidden_status').val('add');
                    $('#hidden_info').val('pr_inv');
                }
            },
            {
                extend: 'excel',
                title: 'Outstanding Reservasi',
                className: 'btn',
                text: '<i class="far fa-file-code"></i> Excel',
                titleAttr: 'Excel',
            },
        ] : [{
            extend: 'excel',
            title: 'Outstanding Reservasi',
            className: 'btn',
            text: '<i class="far fa-file-code"></i> Excel',
            titleAttr: 'Excel',
        }]
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
            url = '/import_material';

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
                        didOpen: () => {
                            Swal.showLoading()
                        }
                    })
                },
                success: function (data) {
                    if (data.result == false) {
                        Swal.fire({
                            title: 'Gagal',
                            text: data.text,
                            icon: 'error',
                            // timer: 3000,
                            showCancelButton: false,
                            showConfirmButton: true,
                            // buttons: false,
                        });

                    }

                    if (data.result == true) {
                        Swal.fire({
                            title: 'Berhasil',
                            icon: 'success',
                            // timer: 3000,
                            showCancelButton: false,
                            showConfirmButton: true,
                            // buttons: false,
                        });
                        $('#import').modal('hide');
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