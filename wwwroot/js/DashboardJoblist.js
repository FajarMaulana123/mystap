let table_wo;
let table_jasa;


function grafik_readiness_joblist(project, project_rev) {
    $.ajax({
        url: '/grafik_readiness_joblist',
        method: 'POST',
        data: {
            project_filter: project,
            project_rev: project_rev
        },
        success: function (res) {
            grafik_readiness("chart_readiness", res.data);
        }
    })
}


function maybeDisposeRoot(div) {
    var data = am5.registry.rootElements;
    for (var i = 0; i < data.length; i++) {
        if (data[i].dom.id == div) {
            data[i].dispose();
        }
    }
}

//function grafik_joblist(div, datas) {
//    maybeDisposeRoot(div);
//    // Create root element
//    // https://www.amcharts.com/docs/v5/getting-started/#Root_element
//    var root = am5.Root.new(div);


//    // Set themes
//    // https://www.amcharts.com/docs/v5/concepts/themes/
//    root.setThemes([
//        am5themes_Animated.new(root)
//    ]);



//    // Create chart
//    // https://www.amcharts.com/docs/v5/charts/percent-charts/pie-chart/
//    var chart = root.container.children.push(am5percent.PieChart.new(root, {
//        layout: root.verticalLayout
//    }));


//    // Create series
//    // https://www.amcharts.com/docs/v5/charts/percent-charts/pie-chart/#Series
//    var series = chart.series.push(am5percent.PieSeries.new(root, {
//        radius: am5.percent(70),
//        valueField: "total",
//        categoryField: "unit",
//        legendLabelText: "{unit}",
//        legendValueText: ":{total} paket"

//    }));

//    // Set data
//    // https://www.amcharts.com/docs/v5/charts/percent-charts/pie-chart/#Setting_data
//    series.data.setAll(datas);


//    // Create legend
//    // https://www.amcharts.com/docs/v5/charts/percent-charts/legend-percent-series/
//    var legend = chart.children.push(am5.Legend.new(root, {
//        centerX: am5.p50,
//        x: am5.p50,
//        marginTop: 15,
//        marginBottom: 15
//    }));

//    legend.data.setAll(series.dataItems);


//    // Play initial series animation
//    // https://www.amcharts.com/docs/v5/concepts/animations/#Animation_of_series
//    series.appear(1000, 100);
//}

function grafik_readiness(div, datas) {
    maybeDisposeRoot(div);
    // Create root element
    // https://www.amcharts.com/docs/v5/getting-started/#Root_element
    var root = am5.Root.new(div);


    // Set themes
    // https://www.amcharts.com/docs/v5/concepts/themes/
    root.setThemes([
        am5themes_Animated.new(root)
    ]);


    // Create chart
    // https://www.amcharts.com/docs/v5/charts/percent-charts/pie-chart/
    var chart = root.container.children.push(am5percent.PieChart.new(root, {
        layout: root.verticalLayout
    }));


    // Create series
    // https://www.amcharts.com/docs/v5/charts/percent-charts/pie-chart/#Series
    var series = chart.series.push(am5percent.PieSeries.new(root, {
        radius: am5.percent(70),
        valueField: "total",
        categoryField: "status_tagno",
        legendLabelText: "{status_tagno}",
        legendValueText: ":{total} Item"
    }));

    series.get("colors").set("colors", [
        am5.color(0xff5b57),
        am5.color(0x00acac),
    ]);

    // Set data
    // https://www.amcharts.com/docs/v5/charts/percent-charts/pie-chart/#Setting_data
    series.data.setAll(datas);



    // series.set("fill", am5.color("#00ff00"));
    // Create legend
    // https://www.amcharts.com/docs/v5/charts/percent-charts/legend-percent-series/
    var legend = chart.children.push(am5.Legend.new(root, {
        centerX: am5.p50,
        x: am5.p50,
        marginTop: 15,
        marginBottom: 15
    }));

    legend.data.setAll(series.dataItems);


    // Play initial series animation
    // https://www.amcharts.com/docs/v5/concepts/animations/#Animation_of_series
    series.appear(1000, 100);
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
        columns: [
            
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
            { data: 'lld', name: 'lld' },
            { data: 'prognosa_', name: 'prognosa_' },
            { data: 'status_', name: 'status_' },
        ],
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

$(document).ready(function () {

    $.ajaxSetup({
        headers: {
            'X-CSRF-TOKEN': $('meta[name="csrf-token"]').attr('content')
        }
    });
    var table_readiness = $('#table_tagno').DataTable({
        dom: '<"dataTables_wrapper dt-bootstrap"<"row"<"col-xl-4 d-block d-sm-flex d-xl-block justify-content-center"<"d-block d-lg-inline-flex me-0 me-md-3"l><"d-block d-lg-inline-flex"B>><"col-xl-8 d-flex d-xl-block justify-content-center"fr>>t<"row"<"col-md-5"i><"col-md-7"p>>>',
        scrollY: '300px',
        scrollCollapse: true,
        paging: false,
        info: false,
        // searching:false,
        processing: true,
        serverSide: true,
        ajax: {
            url: '/get_readiness_joblist',
            method: 'POST',
            data: function (d) {
                d.project_filter = $('#project_filter').find(':selected').data('id');
                d.project_rev = $('#project_filter').find(':selected').data('rev');
            }
        },
        columns: [
            { data: 'no_add', name: 'no_add' },
            // {data: 'projectNo', name: 'projectNo'},
            // {data: 'description', name: 'description'},
            {
                data: 'tag_no', name: 'tag_no',
                render: function (data, type, full, meta) {
                    return full.tag_no + '<span class="popover_ text-primary" style="margin-left:10px"><i class="fas fa-info-circle fa-sm"></i></span>';
                },
            },
            { data: 'disiplin', name: 'disiplin' },
            {
                data: 'status_tagno', name: 'status_tagno',
                render: function (data, type, full, meta) {
                    var status = "";
                    if (full.status_tagno == 'ready') {
                        status = '<span class="badge bg-success">READY</span>';
                    } else if (full.status_tagno == 'not_ready') {
                        status = '<span class="badge bg-danger">NOT READY</span>';
                    } else {
                        status = '<span class="badge bg-secondary">UNDEFINED</span>';
                    }
                    return status;
                }
            },
            { data: 'status_tagno', name: 'status_tagno' },
        ],
        buttons: [
            {
                extend: 'excel',
                title: 'Readiness Joblist',
                className: 'btn',
                text: '<i class="far fa-file-code"></i> Excel',
                titleAttr: 'Excel',
            },


        ],
        columnDefs: [
            { className: 'text-center', targets: [3] },
            {
                targets: [4],
                visible: false
            }
        ],
        order: [],
        rowCallback: function (row, data) {
            $('.popover_', row).webuiPopover({

                title: 'Detail',
                animation: 'fade',
                width: '500px',
                height: '300px',
                html: true,
                cache: false,
                closeable: true,
                dismissible: false,
                type: 'async',
                url: '/readiness_detail_joblist/' + data.projectID + '/' + data.revision + '/' + data.id_paket,
                content: function (data) {
                    return data
                }
            });
        }
    });


    var project = $('#project_filter').val();
    var project_id = $('#project_filter').find(':selected').data('id');
    var project_name = $('#project_filter').find(':selected').data('desc');
    var project_rev = $('#project_filter').find(':selected').data('rev');

    var joblist = $('#job_filter').val();

    $('.judul_project').html('( ' + project_name + ' )');

    grafik_readiness_joblist(project_id, project_rev);

    $(document).on('change', '#status_', function () {
        var val = $(this).val();
        //   $('#status_').val(status)
        table_readiness.column(4).search(val).draw();
    })

    $(document).on('change', '#disiplin_', function () {
        var val = $(this).val();
        table_readiness.column(2).search(val).draw();
    })

    $(document).on('change', '#project_filter', function () {
        var project = $('#project_filter').val();
        var project_id = $('#project_filter').find(':selected').data('id');
        var project_name = $('#project_filter').find(':selected').data('desc');
        var project_rev = $('#project_filter').find(':selected').data('rev');

        var joblist = $('#job_filter').val();
        var area = $('#area_filter').val();
        $('.judul_project').html('( ' + project_name + ' )');

        grafik_readiness_joblist(project_id, project_rev);


        table_readiness.ajax.reload();
    })

    $(document).on('click', '.detail_material', function () {
        $('#table_order').DataTable().destroy();
        get_data_order($(this).data('id'));
        $('#MySecondmodal').modal('show');
    })
    $(document).on('click', '.detail_kontrak', function () {
        detail_jasa($(this).data('jasa'));
        $('#Modal_kontrak').modal('show');
    })
})