
    function get_data(project, katpaket, pic, project_title){
        $.ajax({
            url: '/progress_',
            method: 'POST',
            data: {
                project_filter: project,
                kategori_paket_filter: katpaket,
                pic_filter: pic
            },
            success: function (res) {
                // console.log(res
                $('#isi_table').html(res);
            }
        })
    }

    function downloadFile(response) {
    var blob = new Blob([response], {type: 'application/pdf'})
    var url = URL.createObjectURL(blob);
    location.assign(url);
    }


    $(document).ready(function(){
        $.ajaxSetup({
            headers: {
                'X-CSRF-TOKEN': $('meta[name="csrf-token"]').attr('content')
            }
        });
    $('#filter').on('click', function(){
        var project = $('#project_filter').val();
        var katpaket = $('#kategori_paket_filter').val();
        var pic = $('#pic_filter').val();

        get_data(project, katpaket, pic);
    })

    $('#btnSubmit').on('click', function () {
         
        var gridHtml = $("#Grid").html();
        var project = $('#project_filter').val();
        var project_title = $('#project_filter').find(':selected').data('title');
        var katpaket = $('#kategori_paket_filter').val();
        var pic = $('#pic_filter').val();
            
        $.ajax({
            url: "/generatePDF",
            method: "POST",
            data: {
                project: project,
                katpaket: katpaket,
                pic: pic,
                project_title: project_title,
                gridHtml: gridHtml
            },
            xhrFields: {
                responseType: 'blob'
            },
            success: function (response) {
                console.log(response);
                var blob = new Blob([response]);
                var link = document.createElement('a');
                link.href = window.URL.createObjectURL(blob);
                link.download = "Summary_Progress.pdf";
                link.click();
            },
            error: function (blob) {
                console.log(blob);
            }
        })

    })
    })
