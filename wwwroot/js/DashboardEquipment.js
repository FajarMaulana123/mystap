
    let table_wo;
    let table_jasa;
    //function get_equipments(unit, project){
    //    $.ajax({
    //        url: '/dashboard/equipments',
    //        method: 'POST',
    //        data: {
    //            unit: unit,
    //            project: project
    //        },
    //        success:function(res){
    //            // console.log(res);
    //            grafik_equipment("chart_equipment", JSON.parse(res));
    //        }
    //    })
    //}


    //function grafik_readiness_equipment(project, project_rev){
    //    $.ajax({
    //        url: '/grafik_readiness_equipment',
    //        method: 'POST',
    //        data: {
    //            project_filter: project,
    //            project_rev: project_rev
    //        },
    //        success:function(res){
    //            grafik_readiness("chart_readiness", res);
    //        }
    //    })
    //}


    //function maybeDisposeRoot(div) {
    //    var data = am5.registry.rootElements;
    //    for (var i = 0; i < data.length; i++) {
    //        if (data[i].dom.id == div) {
    //            data[i].dispose();
    //        }
    //    }
    //}

    //function grafik_equipment(div,datas){
    //    maybeDisposeRoot(div);
    //    var root = am5.Root.new(div);


    //    // Set themes
    //    // https://www.amcharts.com/docs/v5/concepts/themes/
    //    root.setThemes([
    //    am5themes_Animated.new(root)
    //    ]);


    //    // Create chart
    //    // https://www.amcharts.com/docs/v5/charts/xy-chart/
    //    var chart = root.container.children.push(am5xy.XYChart.new(root, {
    //    panX: false,
    //    panY: false,
    //    wheelX: "panX",
    //    wheelY: "zoomX",
    //    layout: root.verticalLayout
    //    }));

    //    // Add scrollbar
    //    // https://www.amcharts.com/docs/v5/charts/xy-chart/scrollbars/
    //    chart.set("scrollbarX", am5.Scrollbar.new(root, {
    //    orientation: "horizontal"
    //    }));

    //    var data = datas.datas


    //    // Create axes
    //    // https://www.amcharts.com/docs/v5/charts/xy-chart/axes/
    //    var xRenderer = am5xy.AxisRendererX.new(root, {});
    //    var xAxis = chart.xAxes.push(am5xy.CategoryAxis.new(root, {
    //    categoryField: "unitKilang",
    //    renderer: xRenderer,
    //    tooltip: am5.Tooltip.new(root, {})
    //    }));

    //    xRenderer.grid.template.setAll({
    //    location: 1
    //    })

    //    xAxis.data.setAll(data);

    //    var yAxis = chart.yAxes.push(am5xy.ValueAxis.new(root, {
    //    min: 0,
    //    renderer: am5xy.AxisRendererY.new(root, {
    //        strokeOpacity: 0.1
    //    })
    //    }));


    //    // Add legend
    //    // https://www.amcharts.com/docs/v5/charts/xy-chart/legend-xy-series/
    //    var legend = chart.children.push(am5.Legend.new(root, {
    //    centerX: am5.p50,
    //    x: am5.p50
    //    }));


    //    // Add series
    //    // https://www.amcharts.com/docs/v5/charts/xy-chart/series/
    //    function makeSeries(name, fieldName) {
    //        var series = chart.series.push(am5xy.ColumnSeries.new(root, {
    //            name: name,
    //            stacked: true,
    //            xAxis: xAxis,
    //            yAxis: yAxis,
    //            valueYField: fieldName,
    //            categoryXField: "unitKilang"
    //        }));
            
    //        series.columns.template.setAll({
    //            tooltipText: "{name}, {categoryX} - {valueY}",
    //            tooltipY: am5.percent(10)
    //        });
    //        series.data.setAll(data);

    //        // Make stuff animate on load
    //        // https://www.amcharts.com/docs/v5/concepts/animations/
    //        series.appear();

    //        series.bullets.push(function() {
    //            return am5.Bullet.new(root, {
    //            sprite: am5.Label.new(root, {
    //                text: "{valueY}",
    //                fill: root.interfaceColors.get("alternativeText"),
    //                centerY: am5.p50,
    //                centerX: am5.p50,
    //                populateText: true
    //            })
    //            });
    //        });

    //        legend.data.push(series);
    //    }


    //    makeSeries("HEAT EXCHANGER", "HEAT_EXCHANGER", false);
    //    makeSeries("HEATER", "HEATER", true);
    //    makeSeries("VESSEL", "VESSEL", true);
    //    makeSeries("INSTRUMENT", "INSTRUMENT", true);
    //    makeSeries("LISTRIK", "LISTRIK", true);
    //    makeSeries("PSV", "PSV", true);
    //    makeSeries("PIPING_SYSTEM", "PIPING_SYSTEM", true);
    //    makeSeries("ROTATING", "ROTATING", true);
    //    makeSeries("OTHER EQUIPMENT", "other", true);


    //    // Make stuff animate on load
    //    // https://www.amcharts.com/docs/v5/concepts/animations/
    //    chart.appear(1000, 100);

    //}

    //function grafik_readiness(div, datas){
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
    //        categoryField: "status_tagno",
    //        legendLabelText: "{status_tagno}",
    //        legendValueText: ":{total} TagNo"
    //    }));

    //    series.get("colors").set("colors", [
    //        am5.color(0xff5b57),
    //        am5.color(0x00acac),
    //    ]);
        
    //    // Set data
    //    // https://www.amcharts.com/docs/v5/charts/percent-charts/pie-chart/#Setting_data
    //    series.data.setAll(datas);

        
        
    //    // series.set("fill", am5.color("#00ff00"));
    //    // Create legend
    //    // https://www.amcharts.com/docs/v5/charts/percent-charts/legend-percent-series/
    //    var legend = chart.children.push(am5.Legend.new(root, {
    //    centerX: am5.p50,
    //    x : am5.p50,
    //    marginTop: 15,
    //    marginBottom: 15
    //    }));

    //    legend.data.setAll(series.dataItems);


    //    // Play initial series animation
    //    // https://www.amcharts.com/docs/v5/concepts/animations/#Animation_of_series
    //    series.appear(1000, 100);
    //}

    function get_data_order(id){
        
        table_wo = $('#table_order').DataTable({
            dom: '<"dataTables_wrapper dt-bootstrap"<"row"<"col-xl-7 d-block d-sm-flex d-xl-block justify-content-center"<"d-block d-lg-inline-flex me-0 me-md-3"l><"d-block d-lg-inline-flex"B>><"col-xl-5 d-flex d-xl-block justify-content-center"fr>>t<"row"<"col-md-5"i><"col-md-7"p>>>',
            processing: true,
            serverSide: true,
            ajax: {
                url: 'order_',
                method: 'POST',
                data: function(d){
                    d.id = id;
                }
            },
            columns: [
                
                { data: 'DT_RowIndex', name: 'DT_RowIndex', orderable: false, searchable: false },
                {data: 'order', name: 'order'},
                {data: 'itm', name: 'itm'},
                {data: 'material', name: 'material'},
                {data: 'material_description', name: 'material_description'},
                {data: 'pr', name: 'pr'},
                {data: 'pr_item', name: 'pr_item'},
                {data: 'po', name: 'po'},
                {data: 'po_item', name: 'po_item'},
                {data: 'reqmt_qty', name: 'reqmt_qty'},
                {data: 'pr_qty', name: 'pr_qty'},
                {data: 'po_qty', name: 'po_qty'},
                {data: 'bun', name: 'bun'},
                {data: 'lld', name: 'lld'},
                {data: 'prognosa_', name: 'prognosa_'},
                {data: 'status_', name: 'status_'},
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

   function detail_jasa(wo){
        $.ajax({
            url: '/detail_jasa',
            method: 'POST',
            data: {
                order_ : wo
            },
            success: function(res){
                // console.log(res
                $('#isi_table').html(res);
            }
        })
    }

$(document).ready(function() {
    var table_readiness = $('#table_tagno').DataTable({
        //dom: '<"dataTables_wrapper dt-bootstrap"<"row"<"col-xl-4 d-block d-sm-flex d-xl-block justify-content-center"<"d-block d-lg-inline-flex me-0 me-md-3"l><"d-block d-lg-inline-flex"B>><"col-xl-8 d-flex d-xl-block justify-content-center"fr>>t<"row"<"col-md-5"i><"col-md-7"p>>>',
        
        processing: true,
        serverSide: true,
        ajax: {
            url: '/get_readiness_equipment',
            method: 'POST',
            data: function(d){
                d.project_filter =  $('#project_filter').find(':selected').data('id');
                d.project_rev =  $('#project_filter').find(':selected').data('rev');
            }
        },
        columns: [
            {data: 'jobNo', name: 'jobNo'},
            // {data: 'projectNo', name: 'projectNo'},
            // {data: 'description', name: 'description'},
            {
                data: 'eqTagNo', name: 'eqTagNo',
                render: function (data, type, full, meta) {
                    return full.eqTagNo + '<span class="popover_ text-primary" style="margin-left:10px"><i class="fas fa-info-circle fa-sm"></i></span>';
                },
},
            {data: 'status_tagno', name: 'status_tagno'},
            //{data: 'hidden', name: 'hidden'},
        ],
        buttons: [
        {
            extend: 'excel',
            title: 'Readiness Equipment',
            className: 'btn',
            text: '<i class="far fa-file-code"></i> Excel',
            titleAttr: 'Excel',
        },
        ],
        //columnDefs: [
        //    {className: 'text-center', targets: [2] },
        //    {
        //        targets: [3],
        //        visible: false
        //    }
        //],
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
                url: '/readiness_detail/' + data.projectID + '/' + data.revision + '/' + data.id,
                content: function (data) {
                    return data
                }
            });
        }
    });


    //var project = $('#project_filter').val();
    //var project_id = $('#project_filter').find(':selected').data('id');
    //var project_name = $('#project_filter').find(':selected').data('desc');
    //var project_rev = $('#project_filter').find(':selected').data('rev');

    //var joblist = $('#job_filter').val();

    //$('.judul_project').html('( '+project_name+' )');
    //get_equipments('', project_id);

    //grafik_readiness_equipment(project_id, project_rev);

    //$(document).on('change', '#status_', function(){
    //    var val = $(this).val();
    //    //   $('#status_').val(status)
    //    table_readiness.column(3).search(val ? '^' + val + '$' : '', true, false).draw();
    //})

    //$(document).on('change', '#area_filter', function(){
    //    var project_id = $('#project_filter').find(':selected').data('id');
    //    var area = $('#area_filter').val();
    //    get_equipments(area, project_id);
    //})

    //$(document).on('change', '#project_filter', function(){
    //    var project = $('#project_filter').val();
    //    var project_id = $('#project_filter').find(':selected').data('id');
    //    var project_name = $('#project_filter').find(':selected').data('desc');
    //    var project_rev = $('#project_filter').find(':selected').data('rev');

    //    var joblist = $('#job_filter').val();
    //    var area = $('#area_filter').val();
    //    $('.judul_project').html('( '+project_name+' )');
    //    get_equipments(area, project_id);

    //    grafik_readiness_equipment(project_id, project_rev);


    //    table_readiness.ajax.reload();
    //})

    //$(document).on('click', '.detail_material', function(){
    //    $('#table_order').DataTable().destroy();
    //    get_data_order($(this).data('id'));
    //    $('#MySecondmodal').modal('show');
    //})
    //$(document).on('click', '.detail_kontrak', function(){
    //    detail_jasa($(this).data('jasa'));
    //    $('#Modal_kontrak').modal('show');
    //})  
})