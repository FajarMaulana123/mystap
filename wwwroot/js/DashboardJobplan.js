function get_joblist(project, joblist) {
    $.ajax({
        url: '/grafik_jobplan',
        method: 'POST',
        data: {
            project: project,
            kategori: joblist
        },
        success: function (res) {
            grafik_joblist("chart_joblist", res.data);
        }
    })
}

function get_proggres_jobplan(project) {
    $.ajax({
        url: '/proggres_jobplan',
        method: 'POST',
        data: {
            project: project
        },
        success: function (res) {
            console.log(res)
            grafik_proggres_jobplan("chart_proggres_jobplan", res);
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

function grafik_joblist(div, datas) {
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
        categoryField: "unit",
        legendLabelText: "{unit}",
        legendValueText: ":{total} paket"

    }));

    // Set data
    // https://www.amcharts.com/docs/v5/charts/percent-charts/pie-chart/#Setting_data
    series.data.setAll(datas);


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

function grafik_proggres_jobplan(div, datas) {
    maybeDisposeRoot(div);
    var root = am5.Root.new(div);

    // Set themes
    // https://www.amcharts.com/docs/v5/concepts/themes/
    root.setThemes([
        am5themes_Animated.new(root)
    ]);

    var data = JSON.parse(datas);

    // Create wrapper container
    var container = root.container.children.push(am5.Container.new(root, {
        width: am5.percent(100),
        height: am5.percent(100),
        layout: root.verticalLayout
    }));

    // Create series
    // https://www.amcharts.com/docs/v5/charts/hierarchy/#Adding
    var series = container.children.push(am5hierarchy.ForceDirected.new(root, {
        valueField: "value",
        categoryField: "name",
        childDataField: "children",
        xField: "x",
        yField: "y",
        // centerStrength: 0.1,
        minRadius: 20,
        maxRadius: am5.percent(7)
    }));

    // series.get("colors").setAll({
    //     step: 2
    // });

    // series.links.template.set("strength", 0.5);
    series.nodes.template.setAll({
        draggable: false
    });

    series.data.setAll([data]);

    series.set("selectedDataItem", series.dataItems[0]);


    // Make stuff animate on load
    series.appear(1000, 100);
}




$(document).ready(function () {

    $.ajaxSetup({
        headers: {
            'X-CSRF-TOKEN': $('meta[name="csrf-token"]').attr('content')
        }
    });

    var project = $('#project_filter').val();
    var project_id = $('#project_filter').find(':selected').data('rev');
    var project_name = $('#project_filter').find(':selected').data('desc');
    $('.judul_project').html('( ' + project_name + ' )');


    var table_material = $('#table_material').DataTable({
        dom: '<"dataTables_wrapper dt-bootstrap"<"row"<"col-xl-7 d-block d-sm-flex d-xl-block justify-content-center"<"d-block d-lg-inline-flex me-0 me-md-3"l><"d-block d-lg-inline-flex"B>><"col-xl-5 d-flex d-xl-block justify-content-center"fr>>t<"row"<"col-md-5"i><"col-md-7"p>>>',
        processing: true,
        serverSide: true,
        ajax: {
            url: '/data_material',
            method: 'POST',
            data: function (d) {
                d.project_filter = $('#project_filter').find(':selected').data('id');
                d.lldi = $('#lldi_filter').val();
            }
        },
        lengthMenu: [[5, 10, 25, 50, -1], [5, 10, 25, 50, "All"]],
        columns: [
            //{ data: 'DT_RowIndex', name: 'DT_RowIndex', orderable: false, searchable: false },
            { data: 'alias', name: 'alias' },
            {
                data: 'not_planned', name: 'not_planned',
                render: function (data, type, full, meta) {
                    var jum = full.not_planned + full.not_completed + full.completed;
                    var persen = (full.not_planned / jum) * 100;
                    var isi = full.not_planned+ ' | '+persen+ '%';
                    return isi;
                }
            },
            {
                data: 'not_completed', name: 'not_completed', 
                render: function (data, type, full, meta) {
                    var jum = full.not_planned + full.not_completed + full.completed;
                    var persen = (full.not_completed / jum) * 100;
                    var isi = full.not_completed + ' | ' + persen + '%';
                    return isi;
                }
            },
            {
                data: 'completed', name: 'completed',
                render: function (data, type, full, meta) {
                    var jum = full.not_planned + full.not_completed + full.completed;
                    var persen = (full.completed / jum) * 100;
                    var isi = full.completed + ' | ' + persen + '%';
                    return isi;
                }
            },
            {
                render: function (data, type, full, meta) {
                    var jum = full.not_planned + full.not_completed + full.completed;
                    return jum;
                }
            },
        ],
        columnDefs: [
            { className: 'text-center', targets: [ 1, 2, 3, 4] },
        ],
        footerCallback: function (row, data, start, end, display) {
            var api = this.api();

            // Remove the formatting to get integer data for summation
            var intVal = function (i) {
                return typeof i === 'string' ? i.replace(/[\$,]/g, '') * 1 : typeof i === 'number' ? i : 0;
            };

            // Total over all pages
            not_planned = api
                .column(1)
                .data()
                .reduce(function (a, b) {
                    return intVal(a) + intVal(b);
                }, 0);

            not_completed = api
                .column(2)
                .data()
                .reduce(function (a, b) {
                    return intVal(a) + intVal(b);
                }, 0);

            completed = api
                .column(3)
                .data()
                .reduce(function (a, b) {
                    return intVal(a) + intVal(b);
                }, 0);

            jumlah = api
                .column(4)
                .data()
                .reduce(function (a, b) {
                    return intVal(a) + intVal(b);
                }, 0);


            $(api.column(1).footer()).html(not_planned);
            $(api.column(2).footer()).html(not_completed);
            $(api.column(3).footer()).html(completed);
            $(api.column(4).footer()).html(jumlah);
        },
        buttons: [

            {
                extend: 'excel',
                title: 'Summary Material',
                className: 'btn',
                text: '<i class="far fa-file-code"></i> Excel',
                titleAttr: 'Excel',
                exportOptions: {
                    columns: ':not(:last-child)',
                }
            },
        ]
    });
    var table_jasa = $('#table_jasa').DataTable({
        dom: '<"dataTables_wrapper dt-bootstrap"<"row"<"col-xl-7 d-block d-sm-flex d-xl-block justify-content-center"<"d-block d-lg-inline-flex me-0 me-md-3"l><"d-block d-lg-inline-flex"B>><"col-xl-5 d-flex d-xl-block justify-content-center"fr>>t<"row"<"col-md-5"i><"col-md-7"p>>>',
        processing: true,
        serverSide: true,
        ajax: {
            url: '/data_jasa',
            method: 'POST',
            data: function (d) {
                d.project_filter = $('#project_filter').find(':selected').data('id');
            }
        },
        lengthMenu: [[5, 10, 25, 50, -1], [5, 10, 25, 50, "All"]],
        columns: [
            //{ data: 'DT_RowIndex', name: 'DT_RowIndex', orderable: false, searchable: false },
            { data: 'alias', name: 'alias' },
            {
                data: 'not_planned', name: 'not_planned',
                render: function (data, type, full, meta) {
                    var jum = full.not_planned + full.not_completed + full.completed;
                    var persen = (full.not_planned / jum) * 100;
                    var isi = full.not_planned + ' | ' + persen + '%';
                    return isi;
                }
            },
            {
                data: 'not_completed', name: 'not_completed',
                render: function (data, type, full, meta) {
                    var jum = full.not_planned + full.not_completed + full.completed;
                    var persen = (full.not_completed / jum) * 100;
                    var isi = full.not_completed + ' | ' + persen + '%';
                    return isi;
                }
            },
            {
                data: 'completed', name: 'completed',
                render: function (data, type, full, meta) {
                    var jum = full.not_planned + full.not_completed + full.completed;
                    var persen = (full.completed / jum) * 100;
                    var isi = full.completed + ' | ' + persen + '%';
                    return isi;
                }
            },
            {
                render: function (data, type, full, meta) {
                    var jum = full.not_planned + full.not_completed + full.completed;
                    return jum;
                }
            },
        ],
        columnDefs: [
            { className: 'text-center', targets: [1, 2, 3, 4] },
        ],
        footerCallback: function (row, data, start, end, display) {
            var api = this.api();

            // Remove the formatting to get integer data for summation
            var intVal = function (i) {
                return typeof i === 'string' ? i.replace(/[\$,]/g, '') * 1 : typeof i === 'number' ? i : 0;
            };

            // Total over all pages
            not_planned = api
                .column(1)
                .data()
                .reduce(function (a, b) {
                    return intVal(a) + intVal(b);
                }, 0);

            not_completed = api
                .column(2)
                .data()
                .reduce(function (a, b) {
                    return intVal(a) + intVal(b);
                }, 0);

            completed = api
                .column(3)
                .data()
                .reduce(function (a, b) {
                    return intVal(a) + intVal(b);
                }, 0);

            jumlah = api
                .column(4)
                .data()
                .reduce(function (a, b) {
                    return intVal(a) + intVal(b);
                }, 0);


            $(api.column(1).footer()).html(not_planned);
            $(api.column(2).footer()).html(not_completed);
            $(api.column(3).footer()).html(completed);
            $(api.column(4).footer()).html(jumlah);
        },
        buttons: [

            {
                extend: 'excel',
                title: 'Summary Jasa',
                className: 'btn',
                text: '<i class="far fa-file-code"></i> Excel',
                titleAttr: 'Excel',
                exportOptions: {
                    columns: ':not(:last-child)',
                }
            },
        ]
    });
    var project = $('#project_filter').val();
    var project_id = $('#project_filter').find(':selected').data('id');
    var project_name = $('#project_filter').find(':selected').data('desc');
    var project_rev = $('#project_filter').find(':selected').data('rev');

    var joblist = $('#job_filter').val();

    $('.judul_project').html('( ' + project_name + ' )');
    get_joblist(project_id, joblist);
    //get_proggres_jobplan(project_id);


    $(document).on('change', '#job_filter', function () {
        get_joblist(project_id, $(this).val());
    })

    $(document).on('change', '#area_filter', function () {
        var project_id = $('#project_filter').find(':selected').data('id');
        var area = $('#area_filter').val();
    })

    $(document).on('change', '#project_filter', function () {
        var project = $('#project_filter').val();
        var project_id = $('#project_filter').find(':selected').data('id');
        var project_name = $('#project_filter').find(':selected').data('desc');
        var project_rev = $('#project_filter').find(':selected').data('rev');

        var joblist = $('#job_filter').val();
        var area = $('#area_filter').val();
        $('.judul_project').html('( ' + project_name + ' )');
        get_joblist(project_id, joblist);
        table_material.ajax.reload();
        table_jasa.ajax.reload();
    })
    $(document).on('change', '#lldi_filter', function () {
        table_material.ajax.reload();
    })
})