
    function get_data_material_planner(project_filter, lldi_filter){
        $.ajax({
            url: "/data_material_planner",
            method: "POST",
            data: {
                project_filter: project_filter,
                lldi_filter: lldi_filter
            },
            success: function (res) {
                var datas = res.data;
                if (datas != null) {
                    $('.not_planned').html(res.data.not_planned);
                    $('.not_completed').html(res.data.not_completed);
                    $('.completed').html(res.data.completed);
                } else {
                    $('.not_planned').html('0');
                    $('.not_completed').html('0');
                    $('.completed').html('0');
                }

                // $('.detail_outs_ta_all').html('<a href="/detail_outs_ta/'+project_id+'/ta_all" target="_blank">Lihat Detail <i class="fa fa-arrow-alt-circle-right"></i></a>');
                // $('.detail_outs_ta_lldi').html('<a href="/detail_outs_ta/'+project_id+'/ta_lldi" target="_blank">Lihat Detail <i class="fa fa-arrow-alt-circle-right"></i></a>');
                // $('.detail_outs_oh').html('<a href="/detail_outs_ta/'+project_id+'/oh" target="_blank">Lihat Detail <i class="fa fa-arrow-alt-circle-right"></i></a>');
                // $('.detail_outs_rutin').html('<a href="/detail_outs_ta/'+project_id+'/rutin" target="_blank">Lihat Detail <i class="fa fa-arrow-alt-circle-right"></i></a>');
            }
        })
    }

    $(document).ready(function() {

        $.ajaxSetup({
            headers: {
                'X-CSRF-TOKEN': $('meta[name="csrf-token"]').attr('content')
            }
        });
    var project = $('#project_filter').val();
    var lldi = $('#lldi_filter').val();

    get_data_material_planner(project, lldi);

    $(document).on('change', '#project_filter', function(){
        var project = $('#project_filter').val();
    var lldi = $('#lldi_filter').val();

    get_data_material_planner(project, lldi);

        // table_readiness.ajax.reload();
    })
    // $(document).on('change', '#lldi_filter', function(){
        //     var project = $('#project_filter').val();
        //     var lldi = $('#lldi_filter').val();

        //     get_data_material_planner(project, lldi);

        //     // table_readiness.ajax.reload();
        // })
    })

    var greetElem = document.querySelector("#greetings");
    var curHr = new Date().getHours();
    var greetMes = ["Wow! Masih Begadang?",
    "Good Morning!",
    "Good Afternoon!",
    "Good Evening!",
    "Good Night!",
    "Belum Tidur ya?"];
    let greetText = ""; if (curHr < 4) greetText = greetMes[0];
    else if (curHr < 10) greetText = greetMes[1];
    else if (curHr < 16) greetText = greetMes[2];
    else if (curHr < 18) greetText = greetMes[3];
    else if (curHr < 22) greetText = greetMes[4];
    else greetText = greetMes[5];
    greetElem.setAttribute('data-content', greetText);
