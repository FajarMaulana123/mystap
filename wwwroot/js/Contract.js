function count_progress_jasa() {
    $.ajax({
        url: '/count_progress_jasa',
        method: 'POST',
        data: {
            project_filter: $('#project_filter').val(),
            unit_filter: $('#unit_filter').val(),
            pic_filter: $('#pic_filter').val()
        },
        success: function (res) {
            $('.on_track').html(res.on_track);
            $('.potensi_delay').html(res.potensi_delay);
            $('.delay').html(res.delay);
            $('.sp').html(res.sp);
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
            {
                "data": null, orderable: false, "render": function (data, type, full, meta) {
                    return meta.row + 1;
                }
            },
            { data: 'noPaket', name: 'noPaket' },
            { data: 'nomor', name: 'nomor' },

            { data: 'judulPekerjaan', name: 'judulPekerjaan' },
            { data: 'pic', name: 'pic' },
            { data: 'targetCO', name: 'targetCO' },
            { data: 'aktualCO', name: 'aktualCO' },
            { data: 'floatDaysCO', name: 'floatDaysCO' },
            { data: 'targetBukaPH', name: 'targetBukaPH' },
            { data: 'aktualBukaPH', name: 'aktualBukaPH' },
            { data: 'FDBukaPH', name: 'FDBukaPH' },
            { data: 'targetSP', name: 'targetSP' },
            { data: 'aktualSP', name: 'aktualSP' },
            { data: 'FDSP', name: 'FDSP' },
            { data: 'T.lightcurr', name: 'T.lightcurr' },
            { data: 'deadLine', name: 'deadLine' },

            { data: 'currStat', name: 'currStat' },
            { data: 'currStatDesc', name: 'currStatDesc' },
            { data: 'file_sp', name: 'file_sp' },

            {
                data: 'action',
                name: 'action',
                orderable: false,
                searchable: false
            },
        ],
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
           /* (user_auth == 'user') ? { "visible": false, "targets": [17] } : {},*/
        ],
        buttons: (user_auth == 'superadmin' || user_auth == 'admin') ? [{
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