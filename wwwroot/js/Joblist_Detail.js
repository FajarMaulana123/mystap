let table_wo;
let table_jasa;
var max_fields = 50;
var wrapper = $(".input_fields_wrap");
var add_button = $(".order_material_add");
var x = 1; //initlal text box count

function get_kontrak(id, v) {
    $.ajax({
        url: '/get_paket_kontrak',
        type: 'POST',
        data: {
            id: id
        },
        success: function (res) {
            $('#no_jasa').html(res);
            $('#no_jasa').val(v);
        }
    })
}

function get_wo(id) {
    $.ajax({
        url: '/get_joblist_wo',
        type: 'POST',
        data: {
            id: id
        },
        success: function (res) {
            $(".input_fields_wrap").html(res.isi);
            x = res.total;
        }
    })
}

function get_data_order(id) {

    table_wo = $('#table_order').DataTable({
        dom: '<"dataTables_wrapper dt-bootstrap"<"row"<"col-xl-7 d-block d-sm-flex d-xl-block justify-content-center"<"d-block d-lg-inline-flex me-0 me-md-3"l><"d-block d-lg-inline-flex"B>><"col-xl-5 d-flex d-xl-block justify-content-center"fr>>t<"row"<"col-md-5"i><"col-md-7"p>>>',
        processing: true,
        serverSide: true,
        ajax: {
            url: 'order_',
            method: 'POST',
            data: function (d) {
                d.id = id;
            }
        },
        "lengthMenu": [
            [30, 60, 100, 200, -1],
            [30, 60, 100, 200, "All"]
        ],
        columns: [

           
            //{
            //    "data": null, orderable: false, "render": function (data, type, full, meta) {
            //        return meta.row + 1;
            //    }
            //},
            { data: 'order', name: 'order' },
            { data: 'itm', name: 'itm' },
            { data: 'material', name: 'material' },
            { data: 'material_description', name: 'material_description' },
            { data: 'pr', name: 'pr' },
            { data: 'pr_item', name: 'pr_item' },
            { data: 'po', name: 'po' },
            { data: 'po_item', name: 'po_item' },
            { data: 'reqmt_qty', name: 'reqmt_qty' },
            { data: 'pr_qty', name: 'pr_qty' },
            { data: 'po_qty', name: 'po_qty' },
            { data: 'bun', name: 'bun' },
            { data: 'main_work_ctr', name: 'main_work_ctr' },
            { data: 'lld', name: 'lld' },
            { data: 'prognosa_', name: 'prognosa_' },
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
                    } else if (full.status_ == 'hps_oe') {
                        s = '<span class="badge bg-yellow-300 shadow-none">Penyusunan HPS/OE</span>';
                    } else if (full.status_ == 'proses_tender') {
                        s = '<span class="badge bg-green-200 shadow-none">Proses Tender</span>';
                    } else if (full.status_ == 'Penetapan Pemenang') {
                        s = '<span class="badge bg-green-600 shadow-none">Penetapan Pemenang</span>';
                    }

                    var status = s;
                    return status;
                }
            }
        ],
        // columnDefs: [
        //     {
        //         targets: [2],
        //     createdCell: function(cell) {
        //         var $cell = $(cell);


        //         $(cell).contents().wrapAll("<div class='content'></div>");
        //         var $content = $cell.find(".content");

        //         $(cell).append($("<button>Read more</button>"));
        //         $btn = $(cell).find("button");

        //         $content.css({
        //         "height": "50px",
        //         "overflow": "hidden"
        //         })
        //         $cell.data("isLess", true);

        //         $btn.click(function() {
        //         var isLess = $cell.data("isLess");
        //         $content.css("height", isLess ? "auto" : "50px")
        //         $(this).text(isLess ? "Read less" : "Read more")
        //         $cell.data("isLess", !isLess)
        //         })
        //     }
        //     }
        //  ],
        buttons: [
            {
                extend: 'excel',
                title: 'data wo',
                className: 'btn',
                text: '<i class="far fa-file-code"></i> Excel',
                titleAttr: 'Excel',
            },
        ]

    });
}

$(document).ready(function () {
    $(".select2").select2({});
    $('#Modal').on('show.bs.modal', function (event) {
        $(document).ready(function () {
            $('.search_dropdown').select2({
                dropdownParent: $('#Modal')
            });
        });
    });
    $.ajaxSetup({
        headers: {
            'X-CSRF-TOKEN': $('meta[name="csrf-token"]').attr('content')
        }
    });


    $(add_button).click(function (e) {
        e.preventDefault();
        if (x < max_fields) {
            x++;
            $(wrapper).append('<div class="row row-cols-lg-auto g-3 align-items-center mb-2"><div class="col-12"><div class="form-group"><input type="number" id="no_order" name="no_order[]" class="form-control form-control-sm"></div></div><a class="btn btn-sm btn-danger remove">-</a></div>');
        }
    });
    $(wrapper).on("click", ".remove", function (e) {
        e.preventDefault(); $(this).parent('div').remove(); x--;
    })

    $(document).on('click', '#jasa', function () {
        if (!$('#all_in_kontrak').is(':checked')) {
            if ($(this).is(':checked')) {
                document.getElementById('jasa_').style.display = 'block';
            } else {
                document.getElementById('jasa_').style.display = 'none';
            }
        }
    })

    $(document).on('click', '#all_in_kontrak', function () {
        if (!$('#jasa').is(':checked')) {
            if ($(this).is(':checked')) {
                document.getElementById('jasa_').style.display = 'block';
            } else {
                document.getElementById('jasa_').style.display = 'none';
            }
        }
    })

    $(document).on('click', '#material', function () {
        if ($(this).is(':checked')) {
            document.getElementById('material_').style.display = 'block';
        } else {
            document.getElementById('material_').style.display = 'none';
        }
    })



    var table = $('#table').DataTable({
        dom: '<"dataTables_wrapper dt-bootstrap"<"row"<"col-xl-7 d-block d-sm-flex d-xl-block justify-content-center"<"d-block d-lg-inline-flex me-0 me-md-3"l><"d-block d-lg-inline-flex"B>><"col-xl-5 d-flex d-xl-block justify-content-center"fr>>t<"row"<"col-md-5"i><"col-md-7"p>>>',
        processing: true,
        serverSide: true,
        deferLoading: 0,
        language: {
            "emptyTable": "Data tidak ditemukan - Silahkan Filter data Planning Joblist terlebih dahulu !"
        },
        "lengthMenu": [
            [30, 60, 100, 200, -1],
            [30, 60, 100, 200, "All"]
        ],
        ajax: {
            url: 'jobplan_',
            method: 'POST',
            data: function (d) {
                d.project = $('#project_filter').val();
                d.project_rev = $('#project_filter').find(':selected').data('rev');
                d.eqTagNo = $('#eqTagNo_filter').val();
                d.jobNo = $('#jobNo_filter').val();
                d.freezing = $('#freezing_filter').val();
                d.status_job = $('#status_job_filter').val();
                d.pic = $('#pic_filter').val();
                d.lldi = $('#lldi_filter').val();
                d.disiplin = $('#disiplin_filter').val();
                d.planning = $('#planning_filter').val();
                d.all_in_kontrak = $('#all_in_kontrak_filter').val();
                d.jasa = $('#jasa_filter').val();
                d.material = $('#material_filter').val();
                d.table = 'planning';
            }
        },
        columnDefs: [
            { className: 'bg-jasa', targets: [6, 12] },
            { className: 'bg-material', targets: [13, 15] },
            { className: 'bg-ready', targets: [20] },
            //{ className: 'text-center', targets: [7,11,15,16] },
            { className: 'text-center', targets: [5, 6, 12, 13, 15, 17, 18, 19, 20, 21] },
            { "visible": false, "targets": [7, 8, 9, 10, 14, 16, 11, 3] },
            {
                "targets": 2,
                "data": "description",
                render: function (data, type, row, meta) {
                    if (type === 'display') {
                        data = typeof data === 'string' && data.length > 50 ? data.substring(0, 25) + '...' : data;
                    }
                    return data;
                }
            },
            /*(user_auth == 'user') ? */{ "visible": false, "targets": [21] } ,
        ],
        columns: [
            // {data: 'check', name: 'check', orderable: false,},
            {
                class: 'dt-control',
                orderable: false,
                data: null,
                defaultContent: '',
            },
            // { data: 'DT_RowIndex', name: 'DT_RowIndex', orderable: false, searchable: false },
            { data: 'eqTagNo', name: 'eqTagNo' },
            { data: 'jobDesc', name: 'jobDesc' },
            { data: 'jobDesc', name: 'jobDesc' },
            { data: 'alias', name: 'alias' },
            {
                data: 'status', name: 'status',
                render: function (data, type, full, meta) {

                    var p = (full.project == 1) ? '<span class="badge bg-primary">Project</span>' : '';
                    var cp = (full.critical_job == 1) ? '<span class="badge bg-danger">Critical Job</span>' : '';
                    var f = (full.freezing == 1) ? '<span class="badge bg-info">Freezing</span>' : '';
                    var status = p+' '+cp+' '+f;
                    return status;

                },
            },
            // {data: 'no_memo', name: 'no_memo', orderable: false, searchable: false},
            {
                data: 'isjasa',
                name: 'isjasa',
                render: function (data, type, full, meta) {

                    var d = (full.jasa == 1) ? '<i class="fas fa-check-square text-primary"></i>' : '-';
                    return d;

                },
            },
            { data: 'noPaket', name: 'noPaket', orderable: false, searchable: false },
            { data: 'judul_paket', name: 'judul_paket', orderable: false, searchable: false },
            { data: 'wo_jasa', name: 'wo_jasa', orderable: false, searchable: false },
            { data: 'no_po', name: 'no_po', orderable: false, searchable: false },
            { data: 'no_sp', name: 'no_sp', orderable: false, searchable: false },
            {
                data: 'status_jasa', name: 'status_jasa',
                render: function (data, type, full, meta) {
                    var s = "";
                    if (full.status_jasa != null) {
                        if (full.status_jasa == 'COMPLETED') {
                            s = '<span class="badge bg-success shadow-none">Completed</span>';
                        } else if (full.status_jasa == 'NOT_COMPLETED') {
                            s = '<span class="badge bg-warning shadow-none">Not Completed</span>';
                        } else if (full.status_jasa == 'NOT_PLANNED') {
                            s = '<span class="badge bg-dark shadow-none">Not Planned</span>';
                        } else {
                            s = '<span class="badge bg-danger shadow-none">Not Identify</span>';
                        }
                    }
                    return s;

                },
            },
            {
                data: 'ismaterial',
                name: 'ismaterial',
                render: function (data, type, full, meta) {

                    var d = (full.material == 1) ? '<i class="fas fa-check-square text-primary"></i>' : '-';
                    return d;

                },
            },
            {
                orderable: false, searchable: false,
                render: function (data, type, full, meta) {
                    var status = '';
                    if (full.wo !=  null) {
                        var d = full.wo.split(","); 
                        var isi = '';
                        d.forEach(function (x) {
                            isi += x+";";
                        })
                        var status = isi;
                    }
                    return status;
                }
            },
            {
                data: 'status_material', name: 'status_material',
                render: function (data, type, full, meta) {
                    var s = "";
                    if (full.status_material != null) {
                        if (full.status_material == 'COMPLETED') {
                            s = '<span class="badge bg-success shadow-none">Completed</span>';
                        } else if (full.status_material == 'NOT_COMPLETED') {
                            s = '<span class="badge bg-warning shadow-none">Not Completed</span>';
                        } else if (full.status_material == 'NOT_PLANNED') {
                            s = '<span class="badge bg-dark shadow-none">Not Planned</span>';
                        } else {
                            s = '<span class="badge bg-danger shadow-none">Not Identify</span>';
                        }
                    }
                    return s;

                },
            },
            { data: 'ket_status_material', name: 'ket_status_material' },
            {
                data: 'all_in_kontrak', name: 'all_in_kontrak',
                render: function (data, type, full, meta) {

                    var d = (full.all_in_kontrak == 1) ? '<i class="fas fa-check-square text-primary"></i>' : '-';
                    return d;

                },
            },
            {
                data: 'lldi', name: 'lldi',
                render: function (data, type, full, meta) {

                    var d = (full.lldi == 1) ? '<i class="fas fa-check-square text-primary"></i>' : '-';
                    return d;

                },
            },
            // {data: 'alasan', name: 'alasan'},
            {
                data: 'status_job', name: 'status_job',
                render: function (data, type, full, meta) {
                    var s = "";
                    if (full.status_job == 'COMPLETED') {
                        s = '<span class="badge bg-success shadow-none">Completed</span>';
                    } else if (full.status_job == 'NOT_COMPLETED') {
                        s = '<span class="badge bg-warning shadow-none">Not Completed</span>';
                    } else if (full.status_job == 'NOT_PLANNED') {
                        s = '<span class="badge bg-dark shadow-none">Not Planned</span>';
                    } else {
                        s = '<span class="badge bg-danger shadow-none">Not Identify</span>';
                    }
                    return s;

                },

            },
            {
                data: 'sts_ready', name: 'sts_ready',
                render: function (data, type, full, meta) {
                    var s = "";
                    if (full.status == 'ready') {
                        s = '<span class="badge bg-success">Ready</span>';
                    } else if (full.status == 'not_ready') {
                        s = '<span class="badge bg-danger">Not Ready</span>';
                    } else if (full.status == 'not_identify') {
                        s = 'N/R';
                    } else if (full.status == 'not_execute') {
                        s = '<span class="badge bg-black">Not Execute</span>';
                    } else {
                        s = '<span class="badge bg-secondary">Undefined</span>';
                    }
                    
                    return s;

                },

            },
            { data: 'disiplin', name: 'disiplin' },
            {
                data: 'action',
                name: 'action',
                orderable: false,
                searchable: false,
                "render": function (data, type, full, meta) {
                    var actionBtn = '<div class="d-flex"><a href="javascript:void(0);" class="btn btn-xs waves-effect waves-light btn-outline-primary edit mr-1" data-id="' + full.id + '" data-id_project="' + full.id_project + '"  data-alasan="' + full.alasan + '" data-eqTagNo="' + full.eqTagNo + '" data-jobNo="' + full.jobNo + '" data-pic="' + full.pic + '" data-jobDesc="' + full.jobDesc + '" data-no_jasa="' + full.no_jasa + '" data-order_jasa="' + full.order_jasa + '" data-pekerjaan="' + full.pekerjaan + '" data-status_material="' + full.status_material + '" data-no_order="' + full.no_order + '" data-ket_status_material="' + full.ket_status_material + '" data-jasa="' + full.jasa + '" data-material="' + full.material + '" data-all_in_kontrak="' + full.all_in_kontrak + '" data-lldi="' + full.lldi + '" data-freezing="' + full.freezing +'"><i class="fas fa-pen fa-xs"></i></a>' +
                        '<a href="javascript:void(0);" style = "margin-left:5px" class="btn btn-xs waves-effect waves-light btn-outline-danger delete " data-id="' + full.id + '"> <i class="fas fa-trash fa-xs"></i></a>';
                    return actionBtn;
                },
            },
        ],
        order: [],

        buttons: [
            {
                extend: 'excel',
                title: 'Planning Joblist',
                className: 'btn',
                text: '<i class="far fa-file-code"></i> Excel',
                titleAttr: 'Excel',
                exportOptions: {
                    columns: ':not(:last-child)',
                    orthogonal: 'fullNotes'
                }
            }
        ]


    });

    function format(d) {
        let memo_ = '';
        if (d.no_memo != null) {
            var memo = d.no_memo;
            var arr_memo = memo.split(",");
            var file = d.file_memo;
            var arr_file = file.split(",");
            arr_memo.forEach((val, index) => {
                memo_ += '<a href="' + arr_file[index] + '" target="_blank">' + val + '</a><br>';
            });
        }
        let notif = '';
        if (d.notif != null) {
            notif += '<a href="' + d.link_rekomendasi + '" target="_blank">' + d.no_notif + '</a>';
        } else {
            notif += '-';
        }
        
        var re = "";
        if (d.revision == 'revision0') {
            re = 'Revision 0';
        } else if (d.revision == 'revision1') {
            re = 'Revision 1';
        } else if (d.revision == 'revision2') {
            re = 'Revision 2';
        } else if (d.revision == 'revision3') {
            re = 'Revision 3';
        } else if (d.revision == 'additional1') {
            re = 'Additional 1';
        } else if (d.revision == 'additional2') {
            re = 'Additional 2';
        } else if (d.revision == 'additional3') {
            re = 'Additional 3';
        } else if (d.revision == 'additional_execution') {
            re = 'Additional Execution';
        }

        //table jasa
        var s = "";
        if (d.status_jasa == 'COMPLETED') {
            s = '<span class="badge bg-success shadow-none">Completed</span>';
        } else if (d.status_jasa == 'NOT_COMPLETED') {
            s = '<span class="badge bg-warning shadow-none">Not Completed</span>';
        } else if (d.status_jasa == 'NOT_PLANNED') {
            s = '<span class="badge bg-dark shadow-none">Not Planned</span>';
        } else {
            s = '<span class="badge bg-danger shadow-none">'+d.status_jasa+'</span>';
        }

        var jasa = '';
        if (d.wo_jasa != '') {
            jasa = '<a class="detail_kontrak" data-jasa="'+d.no_jasa+'">'+d.noPaket+'</a>';
        }


        var isi_jasa = '<table class="table table-bordered  table-sm" style="--bs-table-accent-bg: #ffffff00;">';
        isi_jasa+= '<tr><td width="20%"><b>No Paket</b></td><td>: '+jasa+'</td></tr>';
        isi_jasa+= '<tr><td><b>Judul Paket</b></td><td>: '+d.judul_paket+'</td></tr>';
        isi_jasa+= '<tr><td><b>Order Jasa</b></td><td>: '+d.wo_jasa+'</td></tr>';
        isi_jasa+= '<tr><td><b>Status Jasa</b></td><td>: '+s+ '</td></tr>';
        isi_jasa += '</table>';

        //table material
        var m = "";
        if (d.status_material == 'COMPLETED') {
            m = '<span class="badge bg-success shadow-none">Completed</span>';
        } else if (d.status_material == 'NOT_COMPLETED') {
            m = '<span class="badge bg-warning shadow-none">Not Completed</span>';
        } else if (d.status_material == 'NOT_PLANNED') {
            m = '<span class="badge bg-dark shadow-none">Not Planned</span>';
        } else {
            m = '<span class="badge bg-danger shadow-none">'+d.status_material+'</span>';
        }

        var material = '';
        if (d.wo != '') {
            material = '<a class="order" data-id="'+ d.wo+'">'+d.wo+'</a>';
        }

        var bom = "";
        if (d.file_bom != null) {
            var bom_file = d.file_bom.split(";");
            bom_file.forEach(function (val) {
                bom += '<a href="' + val + '" class="badge bg-success" target="blank_"><i class="far fa-copy"></i> file</a>';
            });
        }


        isi_material = '<table class="table table-bordered  table-sm" style="--bs-table-accent-bg: #ffffff00;">';
        isi_material+= '<tr><td><b>Ket. Status Material</b></td><td>: '+d.ket_status_material+'</td></tr>';
        isi_material+= '<tr><td width="20%"><b>Order Material</b></td><td>: '+material+'</td></tr>';
        isi_material+= '<tr><td><b>Status Material</b></td><td>: '+m+ '</td></tr>';
        isi_material+= '<tr><td><b>Bom File</b></td><td>: '+bom+ '</td></tr>';
        isi_material+= '</table>';

        return '<table class="table text-nowrap" style="width:100%; background-color: lightblue; --bs-table-accent-bg: #ffffff00;">' +
            '<tr>' +
            '<td colspan="2">' +
            '<h5>Detail Planning JobList :</h5>' +
            '<table class="table table-bordered table-sm text-nowrap" style="--bs-table-accent-bg: #ffffff00;">' +
            '<tr><td width="9.5%"><b>Description </b></td><td style=" white-space: normal !important;">: <b> ' + d.jobDesc + '</b></td></tr>' +
            '<tr><td width="9.5%"><b>Memo </b></td><td>:' + memo_ + '</td></tr>' +
            '<tr><td width="9.5%"><b>Revision </b></td><td>: ' + re + '</td></tr>' +
            '<tr><td width="9.5%"><b>Notifikasi </b></td><td>:' + notif + '</td></tr>' +
            '</table>' +
            '</td></tr><tr><td width="50%">' +
            '<h6>Detail Jasa :</h6>' +
            isi_jasa +
            '</td><td>' +
            '<h6>Detail Material :</h6>' +
            isi_material +
            '</td></tr></table>';
    }

    function detail_jasa(wo) {
        $.ajax({
            url: '/detail_jasa',
            method: 'POST',
            data: {
                order_: wo
            },
            success: function (res) {
                // console.log(res
                $('#isi_table').html(res);
            }
        })
    }

    $('#table tbody').on('click', 'td.dt-control', function () {
        var tr = $(this).closest('tr');
        var row = table.row(tr);

        if (row.child.isShown()) {
            row.child.hide();
            tr.removeClass('shown');
        } else {
            row.child(format(row.data())).show();
            tr.addClass('shown');
        }
    });

    // var ss = $(".basic").select2({
    //         tags: true,
    // });
    // table.button( 0 ).nodes().css('height', '35px')

    // document.querySelectorAll('a.toggle-vis').forEach((el) => {
    //     el.addEventListener('click', function (e) {
    //         e.preventDefault();

    //         let columnIdx = e.target.getAttribute('data-column');
    //         let column = table.column(columnIdx);

    //         // Toggle the visibility
    //         column.visible(!column.visible());
    //     });
    // });

    $(document).on('click', '#filter', function () {
        var project = $('#project_filter').find(':selected').data('desc');
        $('#title-planning').html('Data Planning Joblist ( ' + project + ' )');
        table.ajax.reload();
    })

    $(document).on('click', '.order', function () {
        $('#table_order').DataTable().destroy();
        get_data_order($(this).data('id'));
        $('#MySecondmodal').modal('show');
    })

    $(document).on('click', '.edit', function () {
        $('#add-form')[0].reset();
        // $('.remove').parent('div').remove();
        // x = 0;
        get_kontrak($(this).data('id_project'), $(this).data('no_jasa'));
        get_wo($(this).data('id'));
        // console.log($(this).data('eqtagno'))
        $("#eqTagNo").val($(this).data('eqtagno'));
        $("#jobNo").val($(this).data('jobno'));
        $('#hidden_id').val($(this).data('id'));
        $('#pic').val($(this).data('pic'));
        $('#jobDesc').val($(this).data('jobdesc'));
        $('#alasan').val($(this).data('alasan'));

        $('#order_jasa').val($(this).data('order_jasa'));
        $('#pekerjaan').val($(this).data('pekerjaan'));
        $('#status_material').val($(this).data('status_material'));
        $('#no_order').val($(this).data('no_order'));
        $('#ket_status_material').val($(this).data('ket_status_material'));


        if ($(this).data('jasa') != 0) {
            $("input[name='jasa']").prop('checked', true);
            document.getElementById('jasa_').style.display = 'block';
        } else {
            $("input[name='jasa']").prop('checked', false);
            document.getElementById('jasa_').style.display = 'none';
        }

        if ($(this).data('all_in_kontrak') != 0) {
            $("input[name='all_in_kontrak']").prop('checked', true);
            if (!$('#jasa').is(':checked')) {
                if ($("#all_in_kontrak").is(':checked')) {
                    document.getElementById('jasa_').style.display = 'block';
                } else {
                    document.getElementById('jasa_').style.display = 'none';
                }
            }
        }

        if ($(this).data('material') != 0) {
            $("input[name='material']").prop('checked', true);
            document.getElementById('material_').style.display = 'block';
        } else {
            $("input[name='material']").prop('checked', false);
            document.getElementById('material_').style.display = 'none';
        }

        ($(this).data('lldi') != 0) ? $("input[name='lldi']").prop('checked', true) : $("input[name='lldi']").prop('checked', false);

        if ($(this).data('freezing') != 1) {
            Swal.fire({
                title: 'Gagal',
                text: 'Status Joblist Belum Freezing',
                icon: 'error',
                // timer: 3000,
                showCancelButton: false,
                showConfirmButton: true,
                // buttons: false,
            });
        } else {
            $('#Modal').modal('show');
            $('#btn-sb').text('Edit');
            $('.judul-modal').text('Update Joblist Detail');
            $('#hidden_status').val('edit');
        }

    })



    $(document).on('change', '#no_jasa', function () {
        var desc = $(this).find(':selected').attr('data-desc');
        var order = $(this).find(':selected').attr('data-order');
        $('#pekerjaan').val(desc)
        $('#order_jasa').val(order)
    })


    $(document).on('click', '.delete', function () {
        var id = $(this).data('id');
        swal.fire({
            title: 'apakah anda yakin?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            confirmButtonText: 'Ya, Hapus!',
            cancelButtonText: 'Tidak',
        }).then((result) => {
            if (result.value) {
                $.ajax({
                    url: "delete_joblist_detail",
                    type: "post",
                    data: {
                        id: id
                    },
                    datatype: "json",
                    success: function (data) {
                        table.ajax.reload();
                        swal.fire({
                            title: data.title,
                            html: '<b>' + data.status + "</b>",
                            icon: data.icon,
                            // timer: 3000,
                            showcancelbutton: false,
                            showconfirmbutton: true,
                            // buttons: false,
                        });
                    },
                    error: function (jqxhr, textstatus, errorthrown) {
                        alert('error');
                    }
                });
            }
        })
    });

    $("#add-form").validate({
        errorClass: "is-invalid",
        // validClass: "is-valid",
        rules: {
            pic: {
                required: true,
            },
        },
        submitHandler: function (form) {
            let url;
            url = 'update_jobplan';
            var form_data = new FormData(document.getElementById("add-form"));
            form_data.append('no_paket', $('#no_jasa').find(':selected').attr('data-no_paket'));
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

    $(document).on('click', '.detail_kontrak', function () {
        detail_jasa($(this).data('jasa'));
        $('#Modal_kontrak').modal('show');
    })


});