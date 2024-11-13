function count_eksekusi() {
    $.ajax({
        url: '/count_eksekusi',
        method: 'POST',
        data: {
            project_filter: $('#project_filter').val(),
            disiplin_filter: $('#disiplin_filter').val()
        },
        success: function (res) {
            $('.di_kerjakan').html(res.result.di_kerjakan);
            $('.tidak_dikerjakan').html(res.result.tidak_dikerjakan);
        }
    })
}
var table;
// function dikerjakan(id, info){
//     if(id.length != 0){
//         $.ajax({
//             url: "/dikerjakan_",
//             method: "POST",
//             data: {
//                 id: id,
//                 info: info
//             },
//             beforeSend: function(){
//                 swal.fire({
//                     title: 'Harap Tunggu!',
//                     allowEscapeKey: false,
//                     allowOutsideClick: false,
//                     showCancelButton: false,
//                     showConfirmButton: false,
//                     buttons: false,
//                     didOpen: () => {
//                         Swal.showLoading()
//                     }
//                 })
//             },
//             success:function(data){
//                 if (data.result != true) {
//                     Swal.fire({
//                         title: 'Gagal',
//                         text: "Silahkan Hubungi Administrator !",
//                         icon: 'error',
//                         // timer: 3000,
//                         showCancelButton: false,
//                         showConfirmButton: true,
//                         // buttons: false,
//                     });
//                     table.ajax.reload();
//                 } else {
//                     Swal.fire({
//                         title: 'Berhasil',
//                         icon: 'success',
//                         // timer: 3000,
//                         showCancelButton: false,
//                         showConfirmButton: true,
//                         // buttons: false,
//                     });
//                     table.ajax.reload();
//                 }
//             }
//         })
//     }else{
//         Swal.fire({
//             title: 'Warning',
//             text: "Pilih Material !",
//             icon: 'warning',
//             // timer: 3000,
//             showCancelButton: false,
//             showConfirmButton: true,
//             // buttons: false,
//         });
//     }
// }
$(document).ready(function () {
    $.ajaxSetup({
        headers: {
            'X-CSRF-TOKEN': $('meta[name="csrf-token"]').attr('content')
        }
    });


    table = $('#table').DataTable({
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
            url: 'paket_eksekusi_',
            method: 'POST',
            data: function (d) {
                d.project = $('#project_filter').val();
                d.eqTagNo = $('#eqTagNo_filter').val();
                d.disiplin = $('#disiplin_filter').val();
                d.status_eksekusi = $('#status_eksekusi').val();
                d.table = 'planning';
            }
        },
        columnDefs: [

            { className: 'text-center', targets: [4, 6] },
            //    { className: 'text-center', targets: [5,6,12,13,15,17,18,19] },
            //    { "visible": false, "targets": [7,8,9,10,11,14,16] },
            //{
            //    targets: [2],
            //    createdCell: function (cell) {
            //        var $cell = $(cell);


            //        $(cell).contents().wrapAll("<div class='content'></div>");
            //        var $content = $cell.find(".content");

            //        $(cell).append($("<button style='border:none; color: blue; text-align: left; background: url();'>...</button>"));
            //        $btn = $(cell).find("button");

            //        $content.css({
            //            "height": "12px",
            //            "overflow": "hidden"
            //        })
            //        $cell.data("isLess", true);

            //        $btn.click(function () {
            //            var isLess = $cell.data("isLess");
            //            $content.css("height", isLess ? "auto" : "12px")
            //            $(this).text(isLess ? "Read less" : "...")
            //            $cell.data("isLess", !isLess)
            //        })
            //    }
            //},
            (role_ == 'user') ? { "visible": false, "targets": [0] } : {},
        ],
        columns: [
            {
                data: 'check', name: 'check', orderable: false,
                "render": function (data, type, full, meta) {
                    var status = '<div class="form-check"><input class="form-check-input check" name="check" type="checkbox" value="'+ full.d.id +'"/></div>';
                    return status;
                },
            },
            { data: 'eqTagNo', name: 'eqTagNo' },
            {
                data: 'd.jobDesc', name: 'd.jobDesc',
                render: function (data, type, full, meta) {
                    return full.d.jobDesc;
                }
            },
            {
                render: function (data, type, full, meta) {
                    return full.alias;
                }
            },
            // {data: 'status', name: 'status'},
            // {
            //     data: 'isjasa', 
            //     name: 'isjasa', 
            // },
            // {data: 'status_jasa', name: 'status_jasa'},
            // {
            //     data: 'ismaterial', 
            //     name: 'ismaterial', 
            // },
            // {data: 'status_material', name: 'status_material'},
            // {data: 'all_in_kontrak', name: 'all_in_kontrak'},
            // {data: 'lldi', name: 'lldi'},
            {
                data: 'status_job', name: 'status_job',
                render: function (data, type, full, meta) {
                    var s = "";
                    if (full.d.status_job == 'COMPLETED') {
                        s = '<span class="badge bg-success shadow-none">Completed</span>';
                    } else if (full.d.status_job == 'NOT_COMPLETED') {
                        s = '<span class="badge bg-warning shadow-none">Not Completed</span>';
                    } else if (full.d.status_job == 'NOT_PLANNED') {
                        s = '<span class="badge bg-dark shadow-none">Not Planned</span>';
                    } else {
                        s = '<span class="badge bg-danger shadow-none">Not Identify</span>';
                    }
                    return s;

                },
            },
            {
                render: function (data, type, full, meta) {
                    return full.d.disiplin;
                }
            },
            {
                data: 'dikerjakan', name: 'dikerjakan',
                render: function (data, type, full, meta) {
                    var s = "";
                    if (full.d.dikerjakan != null) {

                        if (full.d.dikerjakan == 'YA') {
                            s = '<span class="badge bg-success shadow-none">'+full.d.dikerjakan+'</span>';
                        } else {
                            s = '<span class="badge bg-orange shadow-none">'+full.d.dikerjakan+'</span>';
                        }
                    }
                    
                    return s;

                },
            },
            {
                render: function (data, type, full, meta) {
                    return full.d.mitigasi;
                }
            },
            {
                render: function (data, type, full, meta) {
                    return full.d.keterangan;
                }
            },
        ],
        order: [],
        buttons: (role_ == 'superadmin' || role_ == 'admin') ? [{
            text: 'Dikerjakan',
            className: 'btn btn-success',
            action: function (e, dt, node, config) {
                // var id = [];
                // $("input[name='check']:checked").each(function(){
                //     id.push(this.value);
                // });
                // dikerjakan(id, 'ya');
                $('#form_tidak')[0].reset();
                $('#tidak_dikerjakan').modal('show');
                $('#judul_modal').html('Form dikerjakan');
                $('#status_').val('dikerjakan');
                document.getElementById('mitigasi_').style.display = 'none';
            }
        },
        {
            text: 'Tidak dikerjakan',
            className: 'btn btn-warning',
            action: function (e, dt, node, config) {
                $('#form_tidak')[0].reset();
                $('#tidak_dikerjakan').modal('show');
                $('#judul_modal').html('Form Tidak dikerjakan');
                $('#status_').val('tidak_dikerjakan');
                document.getElementById('mitigasi_').style.display = 'block';
            }
         },] : []
    });

    $('body').on('click', '.check-all', function () {
        $("input[type='checkbox']").not(this).prop('checked', this.checked);
    });

    $(document).on('click', '#filter', function () {
        var project = $('#project_filter').find(':selected').data('desc');
        count_eksekusi();
        table.ajax.reload();
    })

    $("#form_tidak").validate({
        errorClass: "is-invalid",
        // validClass: "is-valid",
        rules: {
            mitigasi: {
                required: true
            },
            keterangan: {
                required: function (element) {
                    if ($('#status_').val() == 'tidak_dikerjakan') {
                        return true
                    } else {
                        return false
                    }
                }
            }
        },
        submitHandler: function (form) {
            let url;
            url = '/status_eksekusi';
            var form_data = new FormData(document.getElementById("form_tidak"));
            var id = [];
            $("input[name='check']:checked").each(function () {
                id.push(this.value);
            });
            form_data.append('id', id);
            if (id.length != 0) {
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
                            didOpen: () => {
                                Swal.showLoading()
                            }
                        })
                    },
                    success: function (data) {
                        if (data.result != true) {
                            Swal.fire({
                                title: 'Gagal',
                                text: "Silahkan Hubungi Administrator !",
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
                            $('#tidak_dikerjakan').modal('hide');
                            table.ajax.reload();
                            $(".check-all").prop("checked", false);
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        alert('Error adding / update data');
                    }
                });
            } else {
                Swal.fire({
                    title: 'Warning',
                    text: "Pilih Jobplan !",
                    icon: 'warning',
                    // timer: 3000,
                    showCancelButton: false,
                    showConfirmButton: true,
                    // buttons: false,
                });
            }
        }
    });

});