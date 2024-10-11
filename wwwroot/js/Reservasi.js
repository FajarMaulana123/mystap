var table;

$(document).ready(function () {

    $.ajaxSetup({
        headers: {
            'X-CSRF-TOKEN': $('meta[name="csrf-token"]').attr('content')
        }
    });

    $('body').on('click', '.check-all', function () {
        $("input[type='checkbox']").not(this).prop('checked', this.checked);
    });

    if ($('#project_filter').find(':selected').data('taoh') == 'OH') {
        $('#prog_md').html('Penggunaan Matl');
    } else {
        $('#prog_md').html('Mech_Day');
    }

    table = $('#table').DataTable({
        dom: '<"dataTables_wrapper dt-bootstrap"<"row"<"col-xl-9 d-block d-sm-flex d-xl-block justify-content-center"<"d-block d-lg-inline-flex me-0 me-md-3"l><"d-block d-lg-inline-flex"B>><"col-xl-3 d-flex d-xl-block justify-content-center"fr>>t<"row"<"col-md-5"i><"col-md-7"p>>>',
        processing: true,
        serverSide: true,
        deferLoading: 0,
        language: {
            "emptyTable": "Data tidak ditemukan - Silahkan Filter data Outstanding Reservasi terlebih dahulu !"
        },
        "autoWidth": true,
        "lengthMenu": [
            [100, 200, 300, 500, -1],
            [100, 200, 300, 500, "All"]
        ],
        ajax: {
            url: '/material_',
            method: 'POST',
            data: function (d) {
                d.project_filter = $('#project_filter').val();
                d.status_ready = $('#status_ready').val();
            }
        },
        columns: [
            {
                data: 'status_', name: 'status_',
                render: function (data, type, full, meta) {
                    var s = "";
                    if (full.status_ == 'create_pr') {
                        s = '<span class="badge bg-red-300 shadow-none">Create PR</span>';
                    } else if (full.status_ == 'outstanding_pr') {
                        s = '<span class="badge bg-orange-700 shadow-none">Outstanding PR</span>';
                    } else if (full.status_ == 'tunggu_onsite') {
                        s = '<span class="badge bg-blue-300 shadow-none">Tunggu Onsite</span>';
                    } else if (full.status_ == 'onsite') {
                        s = '<span class="badge bg-blue-600 shadow-none">Onsite</span>';
                    } else if (full.status_ == 'terpenuhi_stock') {
                        s = '<span class="badge bg-purple-300 shadow-none">Stock</span>';
                    } else if (full.status_ == 'inquiry_harga') {
                        s = '<span class="badge bg-orange-400 shadow-none">Inquiry Harga</span>';
                    } else if (full.status_ == 'tunggu_pr') {
                        s = '<span class="badge bg-orange-300 shadow-none">Tunggu PR</span>';
                    } else if (full.status_ == 'evaluasi_dp3') {
                        s = '<span class="badge bg-indigo-300 shadow-none">Evaluasi DP3</span>';
                    } else if (full.status_ == 'inquiry_harga') {
                        s = '<span class="badge bg-purple-600 shadow-none">Inquiry Harga</span>';
                    } else if (full.status_ == 'hps_oe') {
                        s = '<span class="badge bg-yellow-300 shadow-none">HPS OE</span>';
                    } else if (full.status_ == 'bidder_list') {
                        s = '<span class="badge bg-teal-300 shadow-none">Bidder List</span>';
                    } else if (full.status_ == 'penilaian_kualifikasi') {
                        s = '<span class="badge bg-yellow-600 shadow-none">Penilaian Kualifikasi</span>';
                    } else if (full.status_ == 'rfq') {
                        s = '<span class="badge bg-lime-300 shadow-none">RFQ</span>';
                    } else if (full.status_ == 'pemasukan_penawaran') {
                        s = '<span class="badge bg-orange-600 shadow-none">Pemasukan Penawaran</span>';
                    } else if (full.status_ == 'pembukaan_penawaran') {
                        s = '<span class="badge bg-yellow-300 shadow-none">Pembukaan Penawaran</span>';
                    } else if (full.status_ == 'evaluasi_penawaran') {
                        s = '<span class="badge bg-red-300 shadow-none">Evaluasi Penawaran</span>';
                    } else if (full.status_ == 'klarifikasi_spesifikasi') {
                        s = '<span class="badge bg-pink-300 shadow-none">Klarfikasi Spesifikasi</span>';
                    } else if (full.status_ == 'evaluasi_teknis') {
                        s = '<span class="badge bg-teal-600 shadow-none">Evaluasi Teknis</span>';
                    } else if (full.status_ == 'evaluasi_tkdn') {
                        s = '<span class="badge bg-orange-900 shadow-none">Evaluasi TKDN</span>';
                    } else if (full.status_ == 'negosiasi') {
                        s = '<span class="badge bg-lime-600 shadow-none">Negosiasi</span>';
                    } else if (full.status_ == 'lhp') {
                        s = '<span class="badge bg-orange-600 shadow-none">LHP</span>';
                    } else if (full.status_ == 'pengumuman_pemenang') {
                        s = '<span class="badge bg-green-300 shadow-none">Pengumuman Pemenang</span>';
                    } else if (full.status_ == 'penunjuk_pemenang') {
                        s = '<span class="badge bg-green-600 shadow-none">Penunjukan Pemenang</span>';
                    } else if (full.status_ == 'purchase_order') {
                        s = '<span class="badge bg-gray-400 shadow-none">Purchase Order</span>';
                    }

                    var status = s;
                    return status;
                }
            },
            {
                data: 'status_ready', name: 'status_ready',
                render: function (data, type, full, meta) {
                    var s = "";
                    if (full.status_ready == 'ready') {
                        s = '<span class="badge bg-blue-300 shadow-none">Ready</span>';
                    } else if (full.status_ready == 'on_track') {
                        s = '<span class="badge bg-green-300 shadow-none">On Track</span>';
                    } else if (full.status_ready == 'delay') {
                        s = '<span class="badge bg-orange-300 shadow-none">Delay</span>';
                    } else {
                        s = '<span class="badge bg-gray-300 shadow-none">undefined</span>';
                    }

                    var status = s;
                    return status;
                }
            },
            { data: 'order', name: 'order' },
            { data: 'wo_description', name: 'wo_description' },
            { data: 'main_work_ctr', name: 'main_work_ctr' },
            { data: 'revision', name: 'revision' },
            { data: 'reserv_no', name: 'reserv_no' },
            { data: 'material', name: 'material' },
            { data: 'description', name: 'description' },
            { data: 'itm', name: 'itm' },
            { data: 'pr', name: 'pr' },
            { data: 'pr_item', name: 'pr_item' },
            { data: 'po', name: 'po' },
            { data: 'po_item', name: 'po_item' },
            { data: 'reqmt_qty', name: 'reqmt_qty' },
            { data: 'pr_qty', name: 'pr_qty' },
            { data: 'po_qty', name: 'po_qty' },
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
            { data: 'dt_', name: 'dt_' },
            { data: 'prognosa_', name: 'prognosa_' },
            { data: 'md', name: 'md' },
            { data: 'action', name: 'action', orderable: false, searchable: false },

        ],
       /* columnDefs: [
            (user_auth == 'user' || user_auth == 'admin') ? { "visible": false, "targets": [21] } : {},+
        ],*/
        "order": [],
        buttons: /*(user_auth == 'superadmin' || user_auth == 'admin') ?*/ [
            {
                text: '<i class="fas fa-upload"></i> ORDER',
                className: 'btn btn-danger',
                action: function (e, dt, node, config) {
                    $('#import-form')[0].reset();
                    $('#import').modal('show');
                    $('.judul-modal').text('Import Data Order');
                    $('#hidden_status').val('add');
                    $('#hidden_info').val('wo');
                }
            },
            {
                text: '<i class="fas fa-upload"></i> ZPM01',
                className: 'btn btn-warning',
                action: function (e, dt, node, config) {
                    $('#import-form')[0].reset();
                    $('#import').modal('show');
                    $('.judul-modal').text('Import Data ZPM01');
                    $('#hidden_status').val('add');
                    $('#hidden_info').val('zpm01');
                }
            },
            {
                text: '<i class="fas fa-upload"></i> ZPM02',
                className: 'btn btn-lime',
                action: function (e, dt, node, config) {
                    $('#import-form')[0].reset();
                    $('#import').modal('show');
                    $('.judul-modal').text('Import Data ZPM02');
                    $('#hidden_status').val('add');
                    $('#hidden_info').val('zpm02');
                }
            },
            {
                text: '<i class="fas fa-upload"></i> ZPM03',
                className: 'btn btn-info',
                action: function (e, dt, node, config) {
                    $('#import-form')[0].reset();
                    $('#import').modal('show');
                    $('.judul-modal').text('Import Data ZPM03');
                    $('#hidden_status').val('add');
                    $('#hidden_info').val('zpm03');
                }
            },
            {
                text: '<i class="fas fa-upload"></i> ZPM05',
                className: 'btn btn-success',
                action: function (e, dt, node, config) {
                    $('#import-form')[0].reset();
                    $('#import').modal('show');
                    $('.judul-modal').text('Import Data ZPM05');
                    $('#hidden_status').val('add');
                    $('#hidden_info').val('zpm05');
                }
            },
            {
                text: '<i class="fas fa-upload"></i> ZPM07',
                className: 'btn btn-indigo',
                action: function (e, dt, node, config) {
                    $('#import-form')[0].reset();
                    $('#import').modal('show');
                    $('.judul-modal').text('Import Data ZPM07');
                    $('#hidden_status').val('add');
                    $('#hidden_info').val('zpm07');
                }
            },
            {
                text: '<i class="fas fa-upload"></i> MB25',
                className: 'btn btn-purple',
                action: function (e, dt, node, config) {
                    $('#import-form')[0].reset();
                    $('#import').modal('show');
                    $('.judul-modal').text('Import Data MB25');
                    $('#hidden_status').val('add');
                    $('#hidden_info').val('mb25');
                }
            },

            {
                extend: 'excel',
                title: 'Data Reservasi',
                className: 'btn',
                text: '<i class="far fa-file-code"></i> Export Excel',
                titleAttr: 'Excel',
            },

            // {
            //     text: '<i class="fas fa-upload"></i> MB51',
            //     className: 'btn btn-yellow',
            //     action: function(e, dt, node, config) {
            //         $('#import-form')[0].reset();
            //         $('#import').modal('show');
            //         $('.judul-modal').text('Import Data');
            //         $('#hidden_status').val('add');
            //         $('#hidden_info').val('mb51');
            //     }
            // },     

        ] /*: [{
            extend: 'excel',
            title: 'Data Reservasi',
            className: 'btn',
            text: '<i class="far fa-file-code"></i> Export Excel',
            titleAttr: 'Excel',
        }]*/
    });

    $(document).on('click', '.delete', function () {
        var order = $(this).data('order');
        var material = $(this).data('material');
        var itm = $(this).data('itm');
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
                    url: "delete_reservasi",
                    type: "POST",
                    data: {
                        order: order,
                        material: material,
                        itm: itm,
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

                    if (data.result == 'undefined') {
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
        if ($('#project_filter').find(':selected').data('taoh') == 'OH') {
            $('#prog_md').html('Penggunaan Matl');
        } else {
            $('#prog_md').html('Mech_Day');
        }
    })

});