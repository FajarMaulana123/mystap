$(document).ready(function () {

    $.ajaxSetup({
        headers: {
            'X-CSRF-TOKEN': $('meta[name="csrf-token"]').attr('content')
        }
    });

    $('body').on('click', '.check-all', function () {
        $("input[type='checkbox']").not(this).prop('checked', this.checked);
    });

    var table = $('#table').DataTable({
        dom: '<"dataTables_wrapper dt-bootstrap"<"row"<"col-xl-7 d-block d-sm-flex d-xl-block justify-content-center"<"d-block d-lg-inline-flex me-0 me-md-3"l><"d-block d-lg-inline-flex"B>><"col-xl-5 d-flex d-xl-block justify-content-center"fr>>t<"row"<"col-md-5"i><"col-md-7"p>>>',
        processing: true,
        serverSide: true,
        ajax: {
            url: '/penggunaan_material_',
            method: 'POST',
            data: function (d) {
                d.project_filter = $('#project_filter').val();
                d.lldi_filter = $('#lld_filter').val();
            }
        },
        deferLoading: 0,
        language: {
            "emptyTable": "Data tidak ditemukan - Silahkan Filter data Outstanding Reservasi terlebih dahulu !"
        },
        "autoWidth": true,
        "lengthMenu": [
            [100, 200, 300, 500, -1],
            [100, 200, 300, 500, "All"]
        ],

        columns: [
            {
                data: 'check', name: 'check', orderable: false, searchable: false,
                "render": function (data, type, full, meta) {
                    var status = '<div class="form-check"><input class="form-check-input check" name="check" type="checkbox" value="' + full.id + '"/></div>';
                    return status;
                },
            },
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
            { data: 'prognosa_matl', name: 'prognosa_matl' },
        ],
        columnDefs: [
            (role_ == 'user') ? { "visible": false, "targets": [0] } : {},
        ],
        order: [],
        buttons: (role_ == 'superadmin' || role_ == 'admin') ?[
            {
                text: 'Set Penggunaan Material',
                className: 'btn btn-success',
                action: function (e, dt, node, config) {
                    $('#prognosa_material-form')[0].reset();
                    $('#prognosa_material').modal('show');
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

    $("#prognosa_material-form").validate({
        errorClass: "is-invalid",
        // validClass: "is-valid",
        rules: {

        },
        submitHandler: function (form) {
            let url;
            url = '/add_prognosa_material';
            var form_data = new FormData(document.getElementById("prognosa_material-form"));
            var id = [];
            $("input[name='check']:checked").each(function () {
                id.push(this.value);
            });
            form_data.append('id', id);
            if (id.length != 0) {
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
                            didOpen: () => {
                                Swal.showLoading()
                            }
                        })
                    },
                    success: function (data) {
                        if (data.result != true) {
                            Swal.fire({
                                title: 'Gagal',
                                text: "Silahkan Hubungi Administrator !",
                                icon: 'error',
                                // timer: 3000,
                                showCancelButton: false,
                                showConfirmButton: true,
                                // buttons: false,
                            });
                        } else {
                            Swal.fire({
                                title: 'Berhasil',
                                icon: 'success',
                                // timer: 3000,
                                showCancelButton: false,
                                showConfirmButton: true,
                                // buttons: false,
                            });
                            $('#prognosa_material').modal('hide');
                            table.ajax.reload();
                            $(".check-all").prop("checked", false);
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        alert('Error adding / update data');
                    }
                });
            } else {
                Swal.fire({
                    title: 'Warning',
                    text: "Pilih Material !",
                    icon: 'warning',
                    // timer: 3000,
                    showCancelButton: false,
                    showConfirmButton: true,
                    // buttons: false,
                });
            }
        }
    });


    $(document).on('click', '#filter', function () {
        table.ajax.reload();
    })
});