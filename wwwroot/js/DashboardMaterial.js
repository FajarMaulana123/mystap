var table_wo;



function get_summary_pengadaan_material(project_id, project_name, lldi) {
    $.ajax({
        url: '/summary_material_',
        method: 'POST',
        data: {
            project_id: project_id,
            project_name: project_name,
            lldi: lldi,
        },
        beforeSend: function () {
            document.getElementById('load_summary_material_p').style.display = 'block';
        },
        success: function (res) {
            document.getElementById('load_summary_material_p').style.display = 'none';
            $('#list_summary_pengadaan').html(res.datas);

            $('.tot_del').html('<a href="/detail_summary_material_pengadaan/' + project_id + '/deleted/all" target="_blank" style="color:red;">* Data Reservasi yang Dihapus : ' + res.count_del + '</a>');
            // console.log(res.count_del);
        }
    })
}

function get_status_summary(project_id, lldi) {
    $.ajax({
        url: '/grafik_status_summary',
        method: 'POST',
        data: {
            project_id: project_id,
            lldi: lldi,
        },
        success: function (res) {
            grafik_summary_material_("summary_material", res.datas);
            // console.log(res.datas);
            var arr = res.datas;
            for (var i = 0; i < 3; i++) {
                if (arr.find(x => x.status_ready === 'Delay')) {
                    if (res.datas[i].status_ready == 'Delay') {
                        $('.text_delay').html(res.datas[i].total);
                    }
                } else {
                    $('.text_delay').html('0');
                }

                if (arr.find(x => x.status_ready === 'On Track')) {
                    if (res.datas[i].status_ready == 'On Track') {
                        $('.text_ontrack').html(res.datas[i].total);
                    }
                } else {
                    $('.text_ontrack').html('0');
                }

                if (arr.find(x => x.status_ready === 'Ready')) {
                    if (res.datas[i].status_ready == 'Ready') {
                        $('.text_ready').html(res.datas[i].total);
                    }
                } else {
                    $('.text_ready').html('0');
                }
                var lld_filter_ = $('#lldi_filter').val();
                $('.detail_delay').html('<a href="/detail_summary_material/' + project_id + '/delay/' + lld_filter_ + '" target="_blank">Lihat Detail <i class="fa fa-arrow-alt-circle-right"></i></a>');
                $('.detail_on_track').html('<a href="/detail_summary_material/' + project_id + '/on_track/' + lld_filter_ + '" target="_blank">Lihat Detail <i class="fa fa-arrow-alt-circle-right"></i></a>');
                $('.detail_ready').html('<a href="/detail_summary_material/' + project_id + '/ready/' + lld_filter_ + '" target="_blank">Lihat Detail <i class="fa fa-arrow-alt-circle-right"></i></a>');
            }
        }
    })
}

function grafik_material(project_id, project_name, lldi) {
    $.ajax({
        url: '/grafik_material',
        method: 'POST',
        data: {
            project_id: project_id,
            project_name: project_name,
            lldi: lldi,
        },
        success: function (res) {
            grafik_material_("chart_material", res.datas);
        }
    })
}

function grafik_progress_material(project_rev, lldi) {
    $.ajax({
        url: '/grafik_progress_material',
        method: 'POST',
        data: {
            revision: project_rev,
            lldi: lldi
        },
        success: function (res) {
            grafik_progress_material_("chart_progress_material", res.datas);
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

function grafik_material_(div, datas) {
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
        categoryField: "status_",
        legendLabelText: "{status_}",
        legendValueText: ":{total}"

    }));
    series.get("colors").set("colors", [
        am5.color(0xFF0000),
        am5.color(0xFFC000),
        am5.color(0xED7D31),

        am5.color(0x00B050),
        am5.color(0x92D050),

        am5.color(0x0070C0),
        am5.color(0x003760),
    ]);
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

function grafik_summary_material_(div, datas) {
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
        categoryField: "status_ready",
        legendLabelText: "{status_ready}",
        legendValueText: ":{total}"

    }));
    series.get("colors").set("colors", [
        am5.color(0xf59c1a),
        am5.color(0x32a932),
        am5.color(0x348fe2),
    ]);
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

function grafik_progress_material_(div, datas) {
    maybeDisposeRoot(div);
    var root = am5.Root.new(div);

    root.setThemes([
        am5themes_Animated.new(root)
    ]);

    var chart = root.container.children.push(
        am5xy.XYChart.new(root, {
            panY: "none",
            wheelY: "zoomX",
            layout: root.verticalLayout
        })
    );

    chart.get("colors").set("colors", [


        am5.color(0xffffff), //putih 1
        am5.color(0x5B9BD5), //hijau 2
        am5.color(0xFF0000), //kuning 9
        am5.color(0xED7D31), //mas 8
        am5.color(0xFFC000), //oren 7
        am5.color(0xFFFF00), //biru 6
        am5.color(0x92D050), //biru muda 5
        am5.color(0x00B050), //biru langit 4
        am5.color(0x9BC2E6), //merah 3
        am5.color(0x0070C0), //ijo muda 10
    ]);

    // Create Y-axis
    var yAxis = chart.yAxes.push(
        am5xy.ValueAxis.new(root, {
            renderer: am5xy.AxisRendererY.new(root, {})
        })
    );

    // Create X-Axis
    var xAxis = chart.xAxes.push(
        am5xy.DateAxis.new(root, {
            groupData: true,
            baseInterval: { timeUnit: "day", count: 1 },
            renderer: am5xy.AxisRendererX.new(root, {
                minGridDistance: 30
            })
        })
    );

    xAxis.get("dateFormats")["day"] = "MM/dd";
    xAxis.get("periodChangeDateFormats")["day"] = "MMMM";

    // Generate random data

    var data = datas;

    // Create series
    function createSeries(name, field) {
        var series = chart.series.push(
            am5xy.LineSeries.new(root, {
                name: name,
                xAxis: xAxis,
                yAxis: yAxis,
                valueYField: field,
                valueXField: "date",
                tooltip: am5.Tooltip.new(root, {})
            })
        );

        series.bullets.push(function () {
            return am5.Bullet.new(root, {
                sprite: am5.Circle.new(root, {
                    radius: 5,
                    fill: series.get("fill")
                })
            });
        });


        series.strokes.template.set("strokeWidth", 2);

        series.get("tooltip").label.set("text", "[bold]{name}[/]\n{valueX.formatDate()}: {valueY}")
        series.data.processor = am5.DataProcessor.new(root, {
            dateFormat: "yyyy-MM-dd",
            dateFields: ["date"]
        });
        series.data.setAll(data);



        // Pre-zoom X axis
        var tgl = new Date();
        tgl.setDate(tgl.getDate() - 6);

        var tgl_now = new Date();
        tgl_now.setDate(tgl_now.getDate() + 1);

        series.events.once("datavalidated", function (ev, target) {
            xAxis.zoomToDates(tgl, tgl_now)
        })
    }



    createSeries("Total", "total_item");
    createSeries("Stock", "stock");
    createSeries("Create PR", "create_pr");
    createSeries("Outstanding PR", "outstanding_pr");
    createSeries("Inquiry Harga", "inquiry_harga");
    createSeries("HPS/OE", "hps_oe");
    createSeries("Proses Tender", "proses_tender");
    createSeries("Penentapan Pemenang", "penetapan_pemenang");
    createSeries("Tunggu Onsite", "tunggu_onsite");
    createSeries("Onsite", "onsite");


    // Add cursor
    chart.set("cursor", am5xy.XYCursor.new(root, {
        behavior: "zoomXY",
        xAxis: xAxis
    }));

    xAxis.set("tooltip", am5.Tooltip.new(root, {
        themeTags: ["axis"]
    }));

    yAxis.set("tooltip", am5.Tooltip.new(root, {
        themeTags: ["axis"]
    }));

    chart.set("scrollbarX", am5.Scrollbar.new(root, {
        orientation: "horizontal"
    }));
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
    var lldi = $('#lldi_filter').val();
    $('.judul_project').html('( ' + project_name + ' )');
    get_summary_pengadaan_material(project_id, project_name, lldi);
    get_status_summary(project_id, lldi);
    grafik_material(project_id, project_name, lldi);
    grafik_progress_material(project_id, lldi);


    $(document).on('change', '#project_filter', function () {
        var project = $('#project_filter').val();
        var project_id = $('#project_filter').find(':selected').data('rev');
        var project_name = $('#project_filter').find(':selected').data('desc');
        var lldi = $('#lldi_filter').val();

        $('.judul_project').html('( ' + project_name + ' )');

        get_summary_pengadaan_material(project_id, project_name, lldi);
        grafik_material(project_id, project_name, lldi);
        get_status_summary(project_id, lldi);
        grafik_progress_material(project_id, lldi);

        // table_readiness.ajax.reload();
    })

    $(document).on('change', '#lldi_filter', function () {
        var project = $('#project_filter').val();
        var project_id = $('#project_filter').find(':selected').data('rev');
        var project_name = $('#project_filter').find(':selected').data('desc');
        var lldi = $('#lldi_filter').val();
        get_summary_pengadaan_material(project_id, project_name, lldi);
        grafik_material(project_id, project_name, lldi);
        get_status_summary(project_id, lldi);
        grafik_progress_material(project_id, lldi);
    })

})