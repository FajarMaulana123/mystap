var table;
function doc_pr(id, info) {
    if (id.length != 0) {
        $.ajax({
            url: "/doc_pr",
            method: "POST",
            data: {
                id: id,
                info: info
            },
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
                    table.ajax.reload();
                }
            }
        })
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

function remove_buyer(id) {
    if (id.length != 0) {
        $.ajax({
            url: "/remove_buyer",
            method: "POST",
            data: {
                id: id
            },
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
                    table.ajax.reload();
                }
            }
        })
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

$(document).ready(function () {

    $.ajaxSetup({
        headers: {
            'X-CSRF-TOKEN': $('meta[name="csrf-token"]').attr('content')
        }
    });

    $('body').on('click', '.check-all', function () {
        $("input[type='checkbox']").not(this).prop('checked', this.checked);
    });


    var targets_ = [];

    if (fungsi_user != 'TA') {
        targets_ = [13, 14];
    }

    if (fungsi_user == 'INVENTORY') {
        targets_ = [13, 15];
    }


    table = $('#table').DataTable({
        dom: '<"dataTables_wrapper dt-bootstrap"<"row"<"col-xl-9 d-block d-sm-flex d-xl-block justify-content-center"<"d-block d-lg-inline-flex me-0 me-md-3"l><"d-block d-lg-inline-flex"B>><"col-xl-3 d-flex d-xl-block justify-content-center"fr>>t<"row"<"col-md-5"i><"col-md-7"p>>>',
        processing: true,
        serverSide: true,
        ajax: {
            url: '/distribusi_pr_',
            method: 'POST',
            data: function (d) {
                d.project_filter = $('#project_filter').val();
                d.lldi_filter = $('#lld_filter').val();
                d.taoh_filter = $('#taoh_filter').val();
                d.eqTagNo_filter = $('#eqTagNo_filter').val();
                d.pr_filter = $('#pr_filter').val();
                d.wo_filter = $('#wo_filter').val();
                d.buyer_filter = $('#buyer_filter').val();
                d.status_pengadaan_filter = $('#status_pengadaan_filter').val();
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
            {
                
                "render": function (data, type, full, meta) {
                    var status = '<div class="form-check"><input class="form-check-input check" name="check" type="checkbox" value="' + full.id + '"/></div>';
                    return status;
                },
            },
            { data: 'pr', name: 'pr' },
            { data: 'pr_item', name: 'pr_item' },
            { data: 'qty_pr', name: 'qty_pr' },
            { data: 'reqmt_date', name: 'reqmt_date' },
            { data: 'order', name: 'order' },
            { data: 'material', name: 'material' },
            { data: 'material_description', name: 'material_description' },
            { data: 'po', name: 'po' },
            { data: 'lld', name: 'lld' },
            { data: 'equipment', name: 'equipment' },
            // {data: 'eqp_cat', name: 'eqp_cat'},
            // {data: 'manufacturer', name: 'manufacturer'},
            { data: 'buyer', name: 'buyer' },
            // {data: 'status_pr', name: 'status_pr'},
            { data: 'doc_pr', name: 'doc_pr' },
            { data: 'dt_ta', name: 'dt_ta' },
            { data: 'dt_iv', name: 'dt_iv' },
            { data: 'dt_purch', name: 'dt_purch' },
            {
                data: 'status_pengadaan', name: 'status_pengadaan',
                "render": function (data, type, full, meta) {
                    var status = "";
                    if (full.status_pengadaan == 'tunggu_pr') {
                        status = '<span class="badge bg-blue-300 shadow-none">Tunggu PR</span>';
                    } else if (full.status_pengadaan == 'evaluasi_dp3') {
                        status = '<span class="badge bg-indigo-300 shadow-none">Evaluasi DP3</span>';
                    } else if (full.status_pengadaan == 'inquiry_harga') {
                        status = '<span class="badge bg-purple-300 shadow-none">Inquiry Harga</span>';
                    } else if (full.status_pengadaan == 'hps_oe') {
                        status = '<span class="badge bg-cyan-300 shadow-none">HPS OE</span>';
                    } else if (full.status_pengadaan == 'bidder_list') {
                        status = '<span class="badge bg-teal-300 shadow-none">Bidder List</span>';
                    } else if (full.status_pengadaan == 'penilaian_kualifikasi') {
                        status = '<span class="badge bg-green-300 shadow-none">Penilaian Kualifikasi</span>';
                    } else if (full.status_pengadaan == 'rfq') {
                        status = '<span class="badge bg-lime-300 shadow-none">RFQ</span>';
                    } else if (full.status_pengadaan == 'pemasukan_penawaran') {
                        status = '<span class="badge bg-orange-300 shadow-none">Pemasukan Penawaran</span>';
                    } else if (full.status_pengadaan == 'pembukaan_penawaran') {
                        status = '<span class="badge bg-yellow-300 shadow-none">Pembukaan Penawaran</span>';
                    } else if (full.status_pengadaan == 'evaluasi_penawaran') {
                        status = '<span class="badge bg-red-300 shadow-none">Evaluasi Penawaran</span>';
                    } else if (full.status_pengadaan == 'klarifikasi_spesifikasi') {
                        status = '<span class="badge bg-pink-300 shadow-none">Klarfikasi Spesifikasi</span>';
                    } else if (full.status_pengadaan == 'evaluasi_teknis') {
                        status = '<span class="badge bg-teal-600 shadow-none">Evaluasi Teknis</span>';
                    } else if (full.status_pengadaan == 'evaluasi_tkdn') {
                        status = '<span class="badge bg-green-600 shadow-none">Evaluasi TKDN</span>';
                    } else if (full.status_pengadaan == 'negosiasi') {
                        status = '<span class="badge bg-lime-600 shadow-none">Negosiasi</span>';
                    } else if (full.status_pengadaan == 'lhp') {
                        status = '<span class="badge bg-orange-600 shadow-none">LHP</span>';
                    } else if (full.status_pengadaan == 'pengumuman_pemenang') {
                        status = '<span class="badge bg-yellow-600 shadow-none">Pengumuman Pemenang</span>';
                    } else if (full.status_pengadaan == 'penunjuk_pemenang') {
                        status = '<span class="badge bg-red-600 shadow-none">Penunjukan Pemenang</span>';
                    } else if (full.status_pengadaan == 'purchase_order') {
                        status = '<span class="badge bg-pink-600 shadow-none">Purchase Order</span>';
                    }

                    return status;
                },
            },
            { data: 'keterangan', name: 'keterangan' },
        ],
        order: [],
        columnDefs: [
            {
                target: targets_,
                visible: false,
                searchable: false,
            },
            {
                target: [0],
                orderable: false,
                searchable: false,
            },
            {
                targets: [17],
                createdCell: function (cell) {
                    var $cell = $(cell);


                    $(cell).contents().wrapAll("<div class='content'></div>");
                    var $content = $cell.find(".content");

                    $(cell).append($("<button style='border:none; color: blue; text-align: left; background: url();'>...</button>"));
                    $btn = $(cell).find("button");

                    $content.css({
                        "height": "30px",
                        "overflow": "hidden"
                    })
                    $cell.data("isLess", true);

                    $btn.click(function () {
                        var isLess = $cell.data("isLess");
                        $content.css("height", isLess ? "auto" : "30px")
                        $(this).text(isLess ? "Read less" : "...")
                        $cell.data("isLess", !isLess)
                    })
                }
            },
            (role_ == 'user') ? { "visible": false, "targets": [0] } : {},
        ],
        buttons: (role_ == 'superadmin' || role_ == 'admin' && fungsi_user == 'PURCHASING') ? [
            // {
            //     text: 'Set Buyer',
            //     className: 'btn btn-success',
            //     action: function(e, dt, node, config) {
            //         $('#buyer-form')[0].reset();
            //         $('#buyer').modal('show');
            //     }
            // },
            {
                text: 'Update Status',
                className: 'btn btn-info',
                action: function (e, dt, node, config) {
                    $('#status-form')[0].reset();
                    $('#status').modal('show');
                    $('.judul-modal').text('Update Status');
                    $('#hidden_status').val('add');
                }
            },

            {
                text: 'Set Diterima',
                className: 'btn btn-green',
                action: function (e, dt, node, config) {
                    var id = [];
                    $("input[name='check']:checked").each(function () {
                        id.push(this.value);
                    });
                    doc_pr(id, 'true');
                }
            },
            {
                text: 'Set Belum Diterima',
                className: 'btn btn-yellow',
                action: function (e, dt, node, config) {
                    var id = [];
                    $("input[name='check']:checked").each(function () {
                        id.push(this.value);
                    });
                    doc_pr(id, 'false');
                }
            },
            {
                text: 'Remove Buyer',
                className: 'btn btn-danger',
                action: function (e, dt, node, config) {
                    var id = [];
                    $("input[name='check']:checked").each(function () {
                        id.push(this.value);
                    });
                    remove_buyer(id);
                }
            },
            // {
            //     text: 'Keterangan',
            //     className: 'btn btn-secondary',
            //     action: function(e, dt, node, config) {
            //         $('#keterangan-form')[0].reset();
            //         $('#keterangan').modal('show');
            //     }
            // },
        ] : (fungsi_user == 'TA' || fungsi_user == 'INVENTORY') ? [{
            text: 'Isi DT',
            className: 'btn btn-info',
            action: function (e, dt, node, config) {
                $('#dt-form')[0].reset();
                $('#dt').modal('show');
            }
        }] : []
    });


    $(document).on('click', '#filter', function () {
        table.ajax.reload();
    })

    // $("#buyer-form").validate({
    //     errorClass: "is-invalid",
    //     // validClass: "is-valid",
    //     rules: {
    //         buyer: {
    //             required: true
    //         }

    //     },
    //     submitHandler: function(form) {
    //         let url;
    //         url = '/add_buyer';
    //         var form_data = new FormData(document.getElementById("buyer-form"));
    //         var id = [];
    //         $("input[name='check']:checked").each(function(){
    //             id.push(this.value);
    //         });
    //         form_data.append('id', id);
    //         if(id.length != 0){
    //             $.ajax({
    //                 url: url,
    //                 type: "POST",
    //                 data: form_data,
    //                 dataType: "JSON",
    //                 contentType: false,
    //                 cache: false,
    //                 processData: false,
    //                 beforeSend: function(){
    //                     swal.fire({
    //                         title: 'Harap Tunggu!',
    //                         allowEscapeKey: false,
    //                         allowOutsideClick: false,
    //                         showCancelButton: false,
    //                         showConfirmButton: false,
    //                         buttons: false,
    //                         didOpen: () => {
    //                             Swal.showLoading()
    //                         }
    //                     })
    //                 },
    //                 success: function(data) {
    //                     if (data.result != true) {
    //                         Swal.fire({
    //                             title: 'Gagal',
    //                             text: "Silahkan Hubungi Administrator !",
    //                             icon: 'error',
    //                             timer: 3000,
    //                             showCancelButton: false,
    //                             showConfirmButton: false,
    //                             buttons: false,
    //                         });
    //                     } else {
    //                         Swal.fire({
    //                             title: 'Berhasil',
    //                             icon: 'success',
    //                             timer: 3000,
    //                             showCancelButton: false,
    //                             showConfirmButton: false
    //                         });
    //                         $('#buyer').modal('hide');
    //                         table.ajax.reload();
    //                     }
    //                 },
    //                 error: function(jqXHR, textStatus, errorThrown) {
    //                     alert('Error adding / update data');
    //                 }
    //             });
    //         }else{
    //             Swal.fire({
    //                 title: 'Warning',
    //                 text: "Pilih Material !",
    //                 icon: 'warning',
    //                 timer: 3000,
    //                 showCancelButton: false,
    //                 showConfirmButton: false,
    //                 buttons: false,
    //             });
    //         }
    //     }
    // });

    $("#status-form").validate({
        errorClass: "is-invalid",
        // validClass: "is-valid",
        rules: {

        },
        submitHandler: function (form) {
            let url;
            url = '/add_status_pengadaan';
            var form_data = new FormData(document.getElementById("status-form"));
            var id = [];
            $("input[name='check']:checked").each(function () {
                id.push(this.value);
            });
            form_data.append('id', id);
            form_data.append('dt_status_pengadaan', $('#status_pengadaan').find(':selected').data('dt'));
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
                            $('#status').modal('hide');
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

    // $("#keterangan-form").validate({
    //     errorClass: "is-invalid",
    //     // validClass: "is-valid",
    //     rules: {
    //         keterangan: {
    //             required: true
    //         }

    //     },
    //     submitHandler: function(form) {
    //         let url;
    //         url = '/add_keterangan';
    //         var form_data = new FormData(document.getElementById("keterangan-form"));
    //         var id = [];
    //         $("input[name='check']:checked").each(function(){
    //             id.push(this.value);
    //         });
    //         form_data.append('id', id);
    //         if(id.length != 0){
    //             $.ajax({
    //                 url: url,
    //                 type: "POST",
    //                 data: form_data,
    //                 dataType: "JSON",
    //                 contentType: false,
    //                 cache: false,
    //                 processData: false,
    //                 beforeSend: function(){
    //                     swal.fire({
    //                         title: 'Harap Tunggu!',
    //                         allowEscapeKey: false,
    //                         allowOutsideClick: false,
    //                         showCancelButton: false,
    //                         showConfirmButton: false,
    //                         buttons: false,
    //                         didOpen: () => {
    //                             Swal.showLoading()
    //                         }
    //                     })
    //                 },
    //                 success: function(data) {
    //                     if (data.result != true) {
    //                         Swal.fire({
    //                             title: 'Gagal',
    //                             text: "Silahkan Hubungi Administrator !",
    //                             icon: 'error',
    //                             timer: 3000,
    //                             showCancelButton: false,
    //                             showConfirmButton: false,
    //                             buttons: false,
    //                         });
    //                     } else {
    //                         Swal.fire({
    //                             title: 'Berhasil',
    //                             icon: 'success',
    //                             timer: 3000,
    //                             showCancelButton: false,
    //                             showConfirmButton: false
    //                         });
    //                         $('#keterangan').modal('hide');
    //                         table.ajax.reload();
    //                     }
    //                 },
    //                 error: function(jqXHR, textStatus, errorThrown) {
    //                     alert('Error adding / update data');
    //                 }
    //             });
    //         }else{
    //             Swal.fire({
    //                 title: 'Warning',
    //                 text: "Pilih Material !",
    //                 icon: 'warning',
    //                 timer: 3000,
    //                 showCancelButton: false,
    //                 showConfirmButton: false,
    //                 buttons: false,
    //             });
    //         }
    //     }
    // });

    $("#dt-form").validate({
        errorClass: "is-invalid",
        // validClass: "is-valid",
        rules: {
            dt: {
                required: true
            }

        },
        submitHandler: function (form) {
            let url;
            url = '/add_dt';
            var form_data = new FormData(document.getElementById("dt-form"));
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
                                timer: 3000,
                                showCancelButton: false,
                                showConfirmButton: false,
                                buttons: false,
                            });
                        } else {
                            Swal.fire({
                                title: 'Berhasil',
                                icon: 'success',
                                timer: 3000,
                                showCancelButton: false,
                                showConfirmButton: false
                            });
                            $('#dt').modal('hide');
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
                    timer: 3000,
                    showCancelButton: false,
                    showConfirmButton: false,
                    buttons: false,
                });
            }
        }
    });
});