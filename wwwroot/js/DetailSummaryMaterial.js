var table_wo;
$(document).ready(function () {

    $.ajaxSetup({
        headers: {
            'X-CSRF-TOKEN': $('meta[name="csrf-token"]').attr('content')
        }
    });

    var rev = $("#rev").val();
    var filter = $("#filter").val();
    var lldi = $("#lldi").val();

    table_wo = $('#table_order').DataTable({
        dom: '<"dataTables_wrapper dt-bootstrap"<"row"<"col-xl-7 d-block d-sm-flex d-xl-block justify-content-center"<"d-block d-lg-inline-flex me-0 me-md-3"l><"d-block d-lg-inline-flex"B>><"col-xl-5 d-flex d-xl-block justify-content-center"fr>>t<"row"<"col-md-5"i><"col-md-7"p>>>',
        processing: true,
        serverSide: true,
        paging: false,
        ordering: false,
        info: false,
        ajax: {
            url: '../../../detail_summary_material_',
            method: 'POST',
            data: function (d) {
                d.rev = rev;
                d.filter = filter;
                d.lldi = lldi;
            }
        },
        columns: [

            //{ data: 'DT_RowIndex', name: 'DT_RowIndex', orderable: false, searchable: false },
            {
                "data": null, "sortable": false,
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            { data: 'order', name: 'order' },
            { data: 'itm', name: 'itm' },
            { data: 'material', name: 'material' },
            { data: 'description', name: 'description' },
            { data: 'pr', name: 'pr' },
            { data: 'pr_item', name: 'pr_item' },
            { data: 'po', name: 'po' },
            { data: 'po_item', name: 'po_item' },
            { data: 'reqmt_qty', name: 'reqmt_qty' },
            { data: 'pr_qty', name: 'pr_qty' },
            { data: 'po_qty', name: 'po_qty' },
            { data: 'bun', name: 'bun' },
            { data: 'lld', name: 'lld' },
            { data: 'dt_', name: 'dt_' },
            {
                data: 'prognosa_', name: 'prognosa_',
                render: function (data, type, full, meta) {
                    var date = full.prognosa_;
                    if (date != null) {
                        return date.split('T')[0];
                    } else {
                        return "";
                    }
                }
            },
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
        ],
        buttons: [
            {
                extend: 'excel',
                title: 'Detail Material ',
                className: 'btn',
                text: '<i class="far fa-file-code"></i> Excel',
                titleAttr: 'Excel',
            },
            {
                extend: 'pdfHtml5',
                title: 'Detail Material',
                className: 'btn',
                text: '<i class="far fa-file-code"></i> PDF',
                orientation: 'landscape',
                titleAttr: 'PDF',
                download: 'open'
            },
        ]

    });

})