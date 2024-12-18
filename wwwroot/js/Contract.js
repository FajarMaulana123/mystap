﻿function count_progress_jasa() {
    $.ajax({
        url: '/count_progress_jasa',
        method: 'POST',
        data: {
            project_filter: $('#project_filter').val(),
            unit_filter: $('#unit_filter').val(),
            pic_filter: $('#pic_filter').val()
        },
        success: function (res) {
            $('.on_track').html(res.data.on_track);
            $('.potensi_delay').html(res.data.potensi_delay);
            $('.delay').html(res.data.delay);
            $('.sp').html(res.data.sp);
        }
    })
}
$(document).ready(function () {

    $.ajaxSetup({
        headers: {
            'X-CSRF-TOKEN': $('meta[name="csrf-token"]').attr('content')
        }
    });

    // count_progress_jasa();
    var table = $('#table').DataTable({
        dom: '<"dataTables_wrapper dt-bootstrap"<"row"<"col-xl-7 d-block d-sm-flex d-xl-block justify-content-center"<"d-block d-lg-inline-flex me-0 me-md-3"l><"d-block d-lg-inline-flex"B>><"col-xl-5 d-flex d-xl-block justify-content-center"fr>>t<"row"<"col-md-5"i><"col-md-7"p>>>',
        processing: true,
        serverSide: true,
        deferLoading: 0,
        language: {
            "emptyTable": "Data tidak ditemukan - Silahkan Filter data Paket Jasa terlebih dahulu !"
        },
        ajax: {
            url: '/contract_',
            method: 'POST',
            data: function (d) {
                d.project_filter = $('#project_filter').val();
                d.unit_filter = $('#unit_filter').val();
                d.pic_filter = $('#pic_filter').val();
            }
        },
        columns: [
            { data: 'contract.noPaket', name: 'contract.noPaket' },
            {
                render: function (data, type, full, meta) {
                    return full.contract.wo + " " + full.contract.pr + " " + full.contract.po + " " + full.contract.noSP;
                }
            },

            { data: 'judul_pekerjaan', name: 'judul_pekerjaan' },
            { data: 'contract.pic', name: 'contract.pic' },
            {
                data: 'targetCO', name: 'targetCO',
                render: function (data, type, full, meta) {
                    var date = full.contract.targetCO;
                    return date.split('T')[0];
                }
            },
            {
                data: 'aktualCO', name: 'aktualCO',
                render: function (data, type, full, meta) {
                    var date = full.contract.aktualCO;
                    if (date != null) {
                        return date.split('T')[0];
                    } else {
                        return "";
                    }
                }
            },
            {
                data: 'floatDaysCO', name: 'floatDaysCO',
                render: function (data, type, full, meta) {
                    const target = new Date(full.contract.targetCO);
                    var aktual;
                    if (full.contract.aktualCO != null) {
                        aktual = new Date(full.contract.aktualCO);
                    } else {
                        aktual = new Date();
                    }
                    const diffTime = Math.abs(target - aktual);
                    const hari = Math.floor(diffTime / (1000 * 60 * 60 * 24)); 
                    var isi = "";
                    if (hari > 20) {
                        isi = '<span class="badge bg-green-600 ">'+hari+'</span>';
                    } else if (hari <= 20 && hari >= 0) {
                        isi = '<span class="badge bg-warning ">'+hari+'</span>';
                    } else if (hari < 0) {
                        isi = '<span class="badge bg-danger ">'+hari+'</span>';
                    } else {
                        isi = hari;
                    }

                    return isi;
                }
            },
            {
                data: 'targetBukaPH', name: 'targetBukaPH',
                render: function (data, type, full, meta) {
                    var date = full.contract.targetBukaPH;
                    if (date != null) {
                        return date.split('T')[0];
                    } else {
                        return "";
                    }
                }
            },
            {
                data: 'aktualBukaPH', name: 'aktualBukaPH',
                render: function (data, type, full, meta) {
                    var date = full.contract.aktualBukaPH;
                    if (date != null) {
                        return date.split('T')[0];
                    } else {
                        return "";
                    }
                }
            },
            {
                data: 'FDBukaPH', name: 'FDBukaPH',
                render: function (data, type, full, meta) {
                    const target = new Date(full.contract.targetBukaPH);
                    var aktual;
                    if (full.contract.aktualBukaPH != null) {
                        aktual = new Date(full.contract.aktualBukaPH);
                    } else {
                        aktual = new Date();
                    }
                    const diffTime = Math.abs(target - aktual);
                    const hari = Math.floor(diffTime / (1000 * 60 * 60 * 24));
                    var isi = "";
                    if (hari > 20) {
                        isi = '<span class="badge bg-green-600 ">' + hari + '</span>';
                    } else if (hari <= 20 && hari >= 0) {
                        isi = '<span class="badge bg-warning ">' + hari + '</span>';
                    } else if (hari < 0) {
                        isi = '<span class="badge bg-danger ">' + hari + '</span>';
                    } else {
                        isi = hari;
                    }

                    return isi;
                }
            },
            {
                data: 'targetSP', name: 'targetSP',
                render: function (data, type, full, meta) {
                    var date = full.contract.targetSP;
                    if (date != null) {
                        return date.split('T')[0];
                    } else {
                        return "";
                    }
                }
            },
            {
                data: 'aktualSP', name: 'aktualSP',
                render: function (data, type, full, meta) {
                    var date = full.contract.aktualSP;
                    if (date != null) {
                        return date.split('T')[0];
                    } else {
                        return "";
                    }
                }
            },
            {
                data: 'FDSP', name: 'FDSP',
                render: function (data, type, full, meta) {
                    const target = new Date(full.contract.targetSP);
                    var aktual;
                    if (full.contract.aktualCO != null) {
                        aktual = new Date(full.contract.aktualSP);
                    } else {
                        aktual = new Date();
                    }
                    const diffTime = Math.abs(target - aktual);
                    const hari = Math.floor(diffTime / (1000 * 60 * 60 * 24));
                    var isi = "";
                    if (hari > 20) {
                        isi = '<span class="badge bg-green-600 ">' + hari + '</span>';
                    } else if (hari <= 20 && hari >= 0) {
                        isi = '<span class="badge bg-warning ">' + hari + '</span>';
                    } else if (hari < 0) {
                        isi = '<span class="badge bg-danger ">' + hari + '</span>';
                    } else {
                        isi = hari;
                    }

                    return isi;
                }
            },
            {
                data: 'T.lightcurr', name: 'T.lightcurr',
                render: function (data, type, full, meta) {
                    var isi = "";
                    if (full.contract.aktualSP != null) {
                        isi = '<span class="text-primary"><i class="fa fa-circle fs-20px fa-fw me-5px"></i></span>';
                    } else {
                        if (full.contract.t_light > 30) {
                            isi = '<span class="text-green-600"><i class="fa fa-circle fs-20px fa-fw me-5px"></i></span>';
                        } else if (full.contract.t_light <= 30 && full.contract.t_light > 20) {
                            isi = '<span class="text-warning "><i class="fa fa-circle fs-20px fa-fw me-5px"></i></span>';
                        } else if (full.contract.t_light <= 20) {
                            isi = '<span class="text-danger"><i class="fa fa-circle fs-20px fa-fw me-5px"></i></span>';
                        } else {
                            isi = "-";
                        }
                    }
                    return isi;
                }
            },
            {
                data: 'deadLine', name: 'deadLine',
                render: function (data, type, full, meta) {
                    var date = full.contract.deadLine;
                    if (date != null) {
                        return date.split('T')[0];
                    } else {
                        return "";
                    }
                }
            },

            { data: 'contract.currStat', name: 'contract.currStat' },
            { data: 'contract.currStatDesc', name: 'contract.currStatDesc' },
            {
                data: 'file_sp', name: 'file_sp',
                render: function (data, type, full, meta) {
                    var isi = "";
                    if (full.contract.file_sp != null) {
                        isi = '<a href="' + full.contract.file_sp+'" class="btn btn-info btn-sm" target="_blank">File</a>';
                    } else {
                        isi = '';
                    }
                    return isi;
                }
            },

            {
                data: 'action',
                name: 'action',
                orderable: false,
                searchable: false,
                render: function (data, type, full, meta) {
                    var val = '<div class="d-flex">';
                    if (role_ == "superadmin" || role_ == "admin") {
                        val += '<a href="/edit_contract/' + full.contract.idPaket + '" class="btn btn-xs waves-effect waves-light btn-outline-warning edit mr-1" ><i class="fas fa-pen fa-xs"></i></a>';
                    }
                    if (role_ == "superadmin" ) {
                        val += '<a href="javascript:void(0);" style="margin-left:5px" class="btn btn-danger btn-xs delete " data-id="' + full.contract.idPaket + '"><i class="fas fa-trash fa-xs"></i></a>';
                    }
                    val += '</div>';
                    return val;
                }
            },
        ],
        order: [],
        columnDefs: [

            //    { className: 'text-center', targets: [7,11,15,16] },
            { className: 'text-center', targets: [13] },
            {
                targets: [2],
                createdCell: function (cell) {
                    var $cell = $(cell);


                    $(cell).contents().wrapAll("<div class='content'></div>");
                    var $content = $cell.find(".content");

                    $(cell).append($("<button style='border:none; color: blue; text-align: left; background: url();'>... Readmore</button>"));
                    $btn = $(cell).find("button");

                    $content.css({
                        "height": "50px",
                        "overflow": "hidden"
                    })
                    $cell.data("isLess", true);

                    $btn.click(function () {
                        var isLess = $cell.data("isLess");
                        $content.css("height", isLess ? "auto" : "50px")
                        $(this).text(isLess ? "Read less" : "... Readmore")
                        $cell.data("isLess", !isLess)
                    })
                }
            },
            (role_ == 'user') ? { "visible": false, "targets": [18] } : {},
        ],
        buttons: (role_ == 'superadmin' || role_ == 'admin') ? [{
            text: '<i class="far fa-edit"></i> New',
            className: 'btn btn-success',
            action: function (e, dt, node, config) {
                window.location.href = 'create_contract';
            }
        },
        {
            extend: 'excel',
            title: 'Kontrak Jasa',
            className: 'btn',
            text: '<i class="far fa-file-code"></i> Excel',
            titleAttr: 'Excel',
            exportOptions: {
                columns: ':not(:last-child)',
            }
        },
        {
            text: '<i class="far fa-file"></i> Summary Progress',
            className: 'btn btn-warning',
            action: function (e, dt, node, config) {
                window.location.href = 'progress';
            }
        },


        ] : [{
            extend: 'excel',
            title: 'Kontrak Jasa',
            className: 'btn',
            text: '<i class="far fa-file-code"></i> Excel',
            titleAttr: 'Excel',
            exportOptions: {
                columns: ':not(:last-child)',
            }
        },
        {
            text: '<i class="far fa-file"></i> Summary Progress',
            className: 'btn btn-warning',
            action: function (e, dt, node, config) {
                window.location.href = 'progress';
            }
        }]

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
                    url: "delete_contracttracking",
                    type: "POST",
                    data: {
                        id: id
                    },
                    dataType: "JSON",
                    success: function (data) {
                        table.ajax.reload();
                        Swal.fire({
                            title: data.title,
                            html: data.status,
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

    $(document).on('click', '#filter', function () {
        count_progress_jasa();
        table.ajax.reload();
    })
});