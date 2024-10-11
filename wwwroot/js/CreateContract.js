var target_co = '';
var target_ph = '';
var select2label;

function formatDate(date) {
    var d = new Date(date),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2) 
        month = '0' + month;
    if (day.length < 2) 
        day = '0' + day;

    return [year, month, day].join('-');
}

function getKatTender(project){
    $.ajax({
        url: 'getKatTender',
        type: 'POST',
        data: {
            project: project
        },
        success: function(res){
            $('#katTender').html(res);
        }
    })
}

function get_sow(project){
        $.ajax({
        url: 'get_sow',
        type: 'POST',
        data: {
            project: project
        },
        success: function(res){
            $('#judulPekerjaan').html(res);
        }
    })
}

function count_date() {
    var persiapan = ($('#persiapan').val() == '') ? 0 : $('#persiapan').val();
    var fabrikasi = ($('#fabrikasi').val() == '') ? 0 : $('#fabrikasi').val();
    var mdays = ($('#mdays').val() == '') ? 0 : $('#mdays').val();
    var finishing = ($('#finishing').val() == '') ? 0 : $('#finishing').val();
    var maint = ($('#maint').val() == '') ? 0 : $('#maint').val();
        
    var tot = parseInt(persiapan) + parseInt(fabrikasi);
    var jum = parseInt(mdays) + parseInt(finishing) + parseInt(maint);

    var deadLine = $('#deadline').val();
    var total_co = $('#katTender').find(':selected').data('total');
    var total_ph = $('#katTender').find(':selected').data('total_ph');

    if(tot != 0 ){
        var date_targetco = new Date(target_co);
        date_targetco.setDate(date_targetco.getDate() - tot);
        $('#target_co').val(formatDate(date_targetco));

        var date_targetph = new Date(target_ph);
        date_targetph.setDate(date_targetph.getDate() - tot);
        $('#target_buka_ph').val(formatDate(date_targetph));

        var terbit_sp = new Date(deadLine);
        terbit_sp.setDate(terbit_sp.getDate() - tot);
        $('#target_terbit_sp').val(formatDate(terbit_sp));
        $('#start_date').val(formatDate(terbit_sp));
            
    }else{
        var date1 = new Date(deadLine);
        date1.setDate(date1.getDate() - total_co);
        $('#target_co').val(formatDate(date1));

        var date2 = new Date(deadLine);
        date2.setDate(date2.getDate() - total_ph);
        $('#target_buka_ph').val(formatDate(date2));

        var date3 = new Date(deadLine);
        date3.setDate(date3.getDate());
        $('#target_terbit_sp').val(formatDate(date3));
        $('#start_date').val(formatDate(date3));
    }

    if(jum != 0){
        var end_date = new Date(deadLine);
        end_date.setDate(end_date.getDate() + jum);
        $('#end_date').val(formatDate(end_date));
    }else{
        var end_date = new Date(deadLine);
        end_date.setDate(end_date.getDate());
        $('#end_date').val(formatDate(end_date));
    }
}   

$(document).ready(function() {
    $(".select2").select2({});

    $.ajaxSetup({
        headers: {
            'X-CSRF-TOKEN': $('meta[name="csrf-token"]').attr('content')
        }
    });

    $(document).on('click', '.tipe_koordinasi', function(){
        if($(this).val() != 0){
            document.getElementById('show_koordinasi').style.display = 'block';
        }else{
            document.getElementById('show_koordinasi').style.display = 'none';
        }
    })

    $(document).on('change','#deadline',function(){
        var total_co = $('#katTender').find(':selected').data('total');
        var total_ph = $('#katTender').find(':selected').data('total_ph');
        var date_co = new Date($(this).val());
        date_co.setDate(date_co.getDate() - total_co);
        $('#target_co').val(formatDate(date_co));
        target_co = formatDate(date_co)

        var date_ph = new Date($(this).val());
        date_ph.setDate(date_ph.getDate() - total_ph);
        $('#target_buka_ph').val(formatDate(date_ph));
        target_ph = formatDate(date_ph)
        count_date();
    })

    $(document).on('change','#katTender',function(){
        var deadline = $('#deadline').val();
        if(deadline != ''){
            var total_co = $('#katTender').find(':selected').data('total');
            var total_ph = $('#katTender').find(':selected').data('total_ph');
            var date_co = new Date(deadline);
            date_co.setDate(date_co.getDate() - total_co);
            $('#target_co').val(formatDate(date_co));
            target_co = formatDate(date_co)

            var date_ph = new Date(deadline);
            date_ph.setDate(date_ph.getDate() - total_ph);
            $('#target_buka_ph').val(formatDate(date_ph));
            target_ph = formatDate(date_ph)
            count_date();
        }
    })

    $(document).on('change', '#projectID', function(){
        var id = $(this).val();
        getKatTender(id);
        get_sow(id);
    })

        $("#add-form").validate({
        errorClass: "is-invalid",
        highlight: function (element, errorClass, validClass) {
            $(element).parents('.form-control').removeClass('is-valid').addClass('is-invalid');     
        },
        unhighlight: function (element, errorClass, validClass) {
            $(element).parents('.form-control').removeClass('is-invalid').addClass('is-valid');
        },
        errorPlacement: function (error, element) {
            if(element.hasClass('select2') && element.next('.select2-container').length) {
                error.insertAfter(element.next('.select2-container'));
            } else if (element.parent('.input-group').length) {
                error.insertAfter(element.parent());
            }
            else if (element.prop('type') === 'radio' && element.parent('.radio-inline').length) {
                error.insertAfter(element.parent().parent());
            }
            else if (element.prop('type') === 'checkbox' || element.prop('type') === 'radio') {
                error.appendTo(element.parent().parent());
            }
            else {
                error.insertAfter(element);
            }
        },
            
        // validClass: "is-valid",
            
        rules: {
            projectID: {
                required: true
            },
            judulPekerjaan: {
                required: true
            },
            bulan: {
                required: true
            },
            unit: {
                required: true
            },
            pic: {
                required: true
            },
            kategoriPaket: {
                required: true
            },
            tipePaket: {
                required: true
            },
            kriteria: {
                required: true
            },
            katPaket: {
                required: true
            },
            skillGroup: {
                required: true
            },
            csms: {
                required: true
            },
            sourceGroup: {
                required: true
            },
            asalSource: {
                required: true
            },
            kota: {
                required: true
            },
            direksiPengawas: {
                required: true
            },
            katTender: {
                required: true
            },
            deadline: {
                required: true
            },
            persiapan: {
                required: ($('#persiapan') == '0') ? false : true
            },
            fabrikasi: {
                required: ($('#fabrikasi') == '0') ? false : true
            },
            mdays: {
                required: ($('#mdays') == '0') ? false : true
            },
            finishing: {
                required: ($('#finishing') == '0') ? false : true
            },
            maint: {
                required: ($('#maint') == '0') ? false : true
            }


        },

        submitHandler: function(form) {
        let url;
            
        var form_data = new FormData(document.getElementById("add-form"));
            //form_data.append('id_sow', $('#judulPekerjaan').find(':selected').attr('data-id'));

        $.ajax({
            url: 'create_contracttracking',
            type: "POST",
            data: form_data,
            dataType: "JSON",
            contentType: false,
            cache: false,
            processData: false,
            beforeSend: function(){
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
            success: function(data) {
                if (data.result != true) {
                    Swal.fire({
                        title: 'Gagal',
                        text: "Gagal Tambah / Update Contract",
                        icon: 'error',
                        // timer: 3000,
                        showCancelButton: false,
                        showConfirmButton: true,
                        // buttons: false,
                    });
                } else {
                    Swal.fire({
                        title: 'Berhasil',
                        icon: 'success',
                        // timer: 3000,
                        showCancelButton: false,
                        showConfirmButton: true,
                        // buttons: false,
                    });

                    window.location.href = 'contracttracking';
                        
                }
            },
            error: function(jqXHR, textStatus, errorThrown) {
                alert('Error adding / update data');
            }
        });
        }
    });
});

function validate(evt) {
    let theEvent = evt || window.event;
    let key;
    // Handle paste
    if (theEvent.type === 'paste') {
        key = event.clipboardData.getData('text/plain');
    } else {
        // Handle key press
        key = theEvent.keyCode || theEvent.which;
        key = String.fromCharCode(key);
    }
    let regex = /^\d*[]?\d*$/;
    if (!regex.test(key)) {
        theEvent.returnValue = false;
        if (theEvent.preventDefault) theEvent.preventDefault();
    }
}