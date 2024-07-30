function count_progress_jasa(project_id) {
    $.ajax({
        url: '/count_progress_jasa',
        method: 'POST',
        data: {
            project_filter: project_id,
        },
        success: function (res) {
            $('.on_track').html(res.data.on_track);
            $('.potensi_delay').html(res.data.potensi_delay);
            $('.delay').html(res.data.delay);
            $('.sp').html(res.data.sp);
        }
    })
}

function get_kontrak_unit(project, kontrak) {
    $.ajax({
        url: '/kontrak_by_unit',
        method: 'POST',
        data: {
            project: project,
            kategori: kontrak
        },
        success: function (res) {
            grafik_kontrak_unit("chart_kontrak_unit", res.data);
        }
    })
}

function get_kontrak_status(project) {
    $.ajax({
        url: '/kontrak_by_status',
        method: 'POST',
        data: {
            project: project,
        },
        success: function (res) {
            $('.tot_kontrak').html(res.total);
            $('#list_contract_status').html(res.data);
        }
    })
}

function get_chart_kontrak_status(project) {
    $.ajax({
        url: '/chart_kontrak_status',
        method: 'POST',
        data: {
            project: project,
        },
        success: function (res) {
            grafik_kontrak_status("chart_kontrak_status", res.data);
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

function grafik_kontrak_unit(div, datas) {
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
        legendLabelText: "[fontSize: 10px]{unit}[/]",
        legendValueText: " : [fontSize: 10px]{total} paket[/]"
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

function grafik_kontrak_status(div, datas) {
    maybeDisposeRoot(div);
    var root = am5.Root.new(div);


    // Set themes
    // https://www.amcharts.com/docs/v5/concepts/themes/
    root.setThemes([
        am5themes_Animated.new(root)
    ]);


    // Create chart
    // https://www.amcharts.com/docs/v5/charts/xy-chart/
    var chart = root.container.children.push(am5xy.XYChart.new(root, {
        panX: true,
        panY: true,
        wheelX: "panX",
        wheelY: "zoomX",
        pinchZoomX: true
    }));

    // Add cursor
    // https://www.amcharts.com/docs/v5/charts/xy-chart/cursor/
    var cursor = chart.set("cursor", am5xy.XYCursor.new(root, {}));
    cursor.lineY.set("visible", false);


    // Create axes
    // https://www.amcharts.com/docs/v5/charts/xy-chart/axes/
    var xRenderer = am5xy.AxisRendererX.new(root, { minGridDistance: 30 });
    xRenderer.labels.template.setAll({
        rotation: -90,
        centerY: am5.p50,
        centerX: am5.p100,
        paddingRight: 15
    });

    xRenderer.grid.template.setAll({
        location: 1
    })

    var xAxis = chart.xAxes.push(am5xy.CategoryAxis.new(root, {
        maxDeviation: 0.3,
        categoryField: "currStat",
        renderer: xRenderer,
        tooltip: am5.Tooltip.new(root, {})
    }));

    var yAxis = chart.yAxes.push(am5xy.ValueAxis.new(root, {
        maxDeviation: 0.3,
        renderer: am5xy.AxisRendererY.new(root, {
            strokeOpacity: 0.1
        })
    }));


    // Create series
    // https://www.amcharts.com/docs/v5/charts/xy-chart/series/
    var series = chart.series.push(am5xy.ColumnSeries.new(root, {
        name: "Series 1",
        xAxis: xAxis,
        yAxis: yAxis,
        valueYField: "total",
        sequencedInterpolation: true,
        categoryXField: "currStat",
        tooltip: am5.Tooltip.new(root, {
            labelText: "{valueY}"
        })
    }));

    series.columns.template.setAll({ cornerRadiusTL: 5, cornerRadiusTR: 5, strokeOpacity: 0 });
    series.columns.template.adapters.add("fill", function (fill, target) {
        return chart.get("colors").getIndex(series.columns.indexOf(target));
    });

    series.columns.template.adapters.add("stroke", function (stroke, target) {
        return chart.get("colors").getIndex(series.columns.indexOf(target));
    });


    // Set data
    var data = datas;

    xAxis.data.setAll(data);
    series.data.setAll(data);


    // Make stuff animate on load
    // https://www.amcharts.com/docs/v5/concepts/animations/
    series.appear(1000);
    chart.appear(1000, 100);

}

function detail_jasa(currStat, project_id) {
    $.ajax({
        url: '/detail_jasa',
        method: 'POST',
        data: {
            currStat: currStat,
            project_id: project_id,
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


    var project = $('#project_filter').val();
    var project_id = $('#project_filter').find(':selected').data('id');
    var project_name = $('#project_filter').find(':selected').data('desc');
    var kontrak = $('#kontrak_filter').val();
    var joblist = $('#job_filter').val();
    var kontrak_status = $('#kontrak_status_filter').val();
    $('.judul_project').html('( ' + project_name + ' )');

    get_kontrak_unit(project_id, kontrak);
    get_kontrak_status(project_id);
    get_chart_kontrak_status(project_id);
    count_progress_jasa(project_id);

    $(document).on('change', '#kontrak_filter', function () {
        get_kontrak_unit(project_id, $(this).val());
    })

    $(document).on('change', '#project_filter', function () {
        var project = $('#project_filter').val();
        var project_id = $('#project_filter').find(':selected').data('id');
        var project_name = $('#project_filter').find(':selected').data('desc');
        var kontrak = $('#kontrak_filter').val();
        var kontrak_status = $('#kontrak_status_filter').val();
        var joblist = $('#job_filter').val();
        var area = $('#area_filter').val();
        $('.judul_project').html('( ' + project_name + ' )');
        get_kontrak_unit(project_id, kontrak);
        get_kontrak_status(project_id);
        get_chart_kontrak_status(project_id);
        count_progress_jasa(project_id);
    })

    $(document).on('change', '#lldi_filter', function () {
        table.ajax.reload();
    })

    $(document).on('click', '.detail_kontrak', function () {
        var project_id = $('#project_filter').find(':selected').data('id');
        detail_jasa($(this).data('status'), project_id);
        $('#Modal_kontrak').modal('show');
    })
})