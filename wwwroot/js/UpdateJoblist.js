
    function get_memo(project){
        $.ajax({
            url: '/get_memo',
            type: 'POST',
            data: {
                project: project
            },
            dataType: 'html',
            success: function (res) {
                $('#memo').html(res)
            }
        })
    }

    function get_memo_selected(joblist_id) {
        $.ajax({
            url: '/get_memo_selected',
            type: 'POST',
            data: {
                joblist_id: joblist_id
            },
            dataType: 'json',
            success: function (res) {
                $("#memo").val(res).trigger('change');
            }
        })
    }

    function get_notifikasi(eq) {
        $.ajax({
            url: '/get_notifikasi',
            type: 'POST',
            data: {
                eq: eq
            },
            dataType: 'html',
            success: function (res) {
                $('#notifikasi').html(res)
            }
        })
    }



    $(document).ready(function() {
        $(".select2").select2();
        $(".select3").select2({
            dropdownParent: $("#Modal")
        });
        $(".multiple-select2").select2({placeholder: "Select a state",dropdownParent: $("#Modal") });
        $.ajaxSetup({
            headers: {
                'X-CSRF-TOKEN': $('meta[name="csrf-token"]').attr('content')
            }
        });

        get_memo($('#id_project').val());
        get_notifikasi($('#eqtagno_detail').val());

        $(document).on('change', '#projectNo', function(){
            var id_project = $(this).find(':selected').attr('data-id');
            if($(this).find(':selected').attr('data-taoh') == 'OH'){
                document.getElementById('oh').style.display = 'block';
            } else {
                document.getElementById('oh').style.display = 'none';
            }
            get_memo(id_project);
            
        })

        var table = $('#table').DataTable({
            dom: '<"dataTables_wrapper dt-bootstrap"<"row"<"col-xl-7 d-block d-sm-flex d-xl-block justify-content-center"<"d-block d-lg-inline-flex me-0 me-md-3"l><"d-block d-lg-inline-flex"B>><"col-xl-5 d-flex d-xl-block justify-content-center"fr>>t<"row"<"col-md-5"i><"col-md-7"p>>>',
            ajax: {
                url: '/joblistdetail_',
                type: 'POST',
                data: function(d) {
                    d.jobno = $('#jobno_detail').val();
                    d.joblist_id = $('#joblist_id').val();
                }
            },
            processing: true,
            serverSide: true,
            columns: [
                { data: 'jobNo', name: 'jobNo' },
                { data: 'jobDesc', name: 'jobDesc' },
                { data: 'disiplin', name: 'disiplin' },
                {
                    data: 'status', name: 'status',
                    "render": function (data, type, full, meta) {
                        var project = (full.project != 0) ? '<span class="badge bg-primary"> Project </span>' : '';
                        var freezing = (full.freezing != 0) ? '<span class="badge bg-info"> Freezing </span>' : '';
                        var critical_job = (full.critical_job != 0) ? '<span class="badge bg-danger"> Critical </span>' : '';
                        var row = project + ' ' + freezing + ' ' + critical_job;
                        return row;
                    },
                },
                { data: 'alasan', name: 'alasan' },
                {
                    data: 'cln', name: 'cln',
                    "render": function (data, type, full, meta) {
                        var cln = (full.cleaning != 0) ? '<i class="fas fa-check-square text-primary"></i>' : '';
                        return cln;
                    }
                },
                {
                    data: 'ins', name: 'ins',
                    "render": function (data, type, full, meta) {
                        var ins = (full.inspection != 0) ? '<i class="fas fa-check-square text-primary"></i>' : '';
                        return ins;
                    }
                },
                {
                    data: 'rpr', name: 'rpr',
                    "render": function (data, type, full, meta) {
                        var rpr = (full.repair != 0) ? '<i class="fas fa-check-square text-primary"></i>' : '';
                        return rpr;
                    }
                },
                {
                    data: 'rpl', name: 'rpl',
                    "render": function (data, type, full, meta) {
                        var rpl = (full.replace != 0) ? '<i class="fas fa-check-square text-primary"></i>' : '';
                        return rpl;
                    }
                },
                {
                    data: 'ndt', name: 'ndt',
                    "render": function (data, type, full, meta) {
                        var ndt = (full.ndt != 0) ? '<i class="fas fa-check-square text-primary"></i>' : '';
                        return ndt;
                    }
                },
                {
                    data: 'tein', name: 'tein',
                    "render": function (data, type, full, meta) {
                        var tein = (full.tein != 0) ? '<i class="fas fa-check-square text-primary"></i>' : '';
                        return tein;
                    }
                },
                {
                    data: 'msr', name: 'msr',
                    "render": function (data, type, full, meta) {
                        var msr = (full.measurement != 0) ? '<i class="fas fa-check-square text-primary"></i>' : '';
                        return msr;
                    }
                },
                {
                    data: 'dwg', name: 'dwg',
                    "render": function (data, type, full, meta) {
                        var dwg = (full.drawing != 0) ? '<i class="fas fa-check-square text-primary"></i>' : '';
                        return dwg;
                    }
                },
                {
                    data: 'mod', name: 'mod',
                    "render": function (data, type, full, meta) {
                        var mod = (full.modif != 0) ? '<i class="fas fa-check-square text-primary"></i>' : '';
                        return mod;
                    }
                },
                // {data: 'pnt', name: 'pnt'},
                {
                    data: 'coc', name: 'coc',
                    "render": function (data, type, full, meta) {
                        var coc = (full.coc != 0) ? '<i class="fas fa-check-square text-primary"></i>' : '';
                        return coc;
                    }
                },
                {
                    data: 'hsse', name: 'hsse',
                    "render": function (data, type, full, meta) {
                        var hsse = (full.hsse != 0) ? '<i class="fas fa-check-square text-primary"></i>' : '';
                        return hsse;
                    }
                },
                {
                    data: 'rel', name: 'rel',
                    "render": function (data, type, full, meta) {
                        var rel = (full.reliability != 0) ? '<i class="fas fa-check-square text-primary"></i>' : '';
                        return rel;
                    }
                },
                {
                    data: 'los', name: 'los',
                    "render": function (data, type, full, meta) {
                        var los = (full.losses != 0) ? '<i class="fas fa-check-square text-primary"></i>' : '';
                        return los;
                    }
                },
                {
                    data: 'eng', name: 'eng',
                    "render": function (data, type, full, meta) {
                        var eng = (full.energi != 0) ? '<i class="fas fa-check-square text-primary"></i>' : '';
                        return eng;
                    }
                },
                {
                    data: 'action',
                    name: 'action',
                    orderable: false,
                    searchable: false,
                    "render": function (data, type, full, meta) {
                        var actionBtn = '<div class="d-flex"><a href="javascript:void(0);" class="btn btn-xs waves-effect waves-light btn-outline-warning edit mr-1" data-id="' + full.id + '" data-alasan="' + full.alasan + '" data-jobdesc="' + full.jobDesc + '" data-engineer="' + full.engineer + '" data-revision="' + full.revision + '" data-execution="' + full.execution + '" data-responsibility="' + full.responsibility + '" data-ram="' + full.ram + '" data-notif="' + full.notif + '" data-cleaning="' + full.cleaning + '" data-inspection="' + full.inspection + '" data-repair="' + full.repair + '" data-replace="' + full.replace + '" data-ndt="' + full.ndt + '" data-modif="' + full.modif + '" data-tein="' + full.tein + '" data-coc="' + full.coc + '" data-drawing="' + full.drawing + '" data-measurement="' + full.measurement + '" data-hsse="' + full.hsse + '" data-reliability="' + full.reliability + '" data-losses="' + full.losses + '" data-energi="' + full.energi + '" data-disiplin="' + full.disiplin + '" data-project="' + full.project + '" data-critical_job="' + full.critical_job + '" data-freezing="' + full.freezing + '"><i class="fas fa-pen fa-xs"></i></a>' +
                            '<a href = "javascript:void(0);" style = "margin-left:5px" class="btn btn-xs waves-effect waves-light btn-outline-danger delete " data-id="' + full.id + '" > <i class="fas fa-trash fa-xs"></i></a></div>';
                        return actionBtn;
                    }
                },
            ],
            columnDefs: [
                {
                    targets: [5,6,7,8,9,10,11,12,13,14,15,16,17,18,19],
                    className: 'text-center' 

                },{
                 targets: [1],
                createdCell: function(cell) {
                        var $cell = $(cell);
                        $(cell).contents().wrapAll("<div class='content'></div>");
                        var $content = $cell.find(".content");

                        $(cell).append($("<button style='border:none; color: blue; text-align: left; background: url();'>... Readmore</button>"));
                        $btn = $(cell).find("button");

                        $content.css({
                            "height": "50px",
                            "overflow": "hidden"
                        })
                        $cell.data("isLess", true);

                        $btn.click(function() {
                            var isLess = $cell.data("isLess");
                            $content.css("height", isLess ? "auto" : "50px")
                            $(this).text(isLess ? "Read less" : "... Readmore")
                            $cell.data("isLess", !isLess)
                        })
                    }
                }
            ],
            buttons: [{
                text: '<i class="far fa-edit"></i> New',
                className: 'btn btn-outline-primary ',
                action: function(e, dt, node, config) {
                    $('#detail-form')[0].reset();
                    // $().not("#jobno_detail,#eqtagno_detail,#joblist_id,#hidden_job").val();
                    $('#detail-form','input:checkbox').prop('checked', false);
                    $('#detail-form','input:radio').prop('checked', false);

                    $('#memo').val([]).trigger('change');
                    $("#notifikasi").val('').trigger('change');

                    $('#Modal').modal('show');
                    $('#btn-sb').text('Tambah');
                    $('.judul-modal').text('Tambah Joblist Detail');
                    $('#hidden_status').val('add');
                }
            },{
                extend: 'excel',
                title: 'Joblist Detail',
                className: 'btn btn-outline-primary',
                text: '<i class="far fa-file-code"></i> Excel',
                titleAttr: 'Excel',
                exportOptions: {
                    columns: ':not(:last-child)',
                }
            }]
        });

        $(document).on('click', '.edit', function() {
            $('#detail-form')[0].reset();

            get_memo_selected($(this).data('id'));

            $('#hidden_id').val($(this).data('id'));
            $('#jobDesc').val($(this).data('jobdesc'));
            $('#alasan').val($(this).data('alasan'));
            $('#revision').val($(this).data('revision'));
            $('#insepector').val($(this).data('engineer'));
            $('#eksekusi').val($(this).data('execution'));
            $('#responbility').val($(this).data('responsibility'));
            $('#ram').val($(this).data('ram'));

            $("#notifikasi option[data-notifikasi='"+$(this).data('notif')+"']").attr("selected","selected");

            ($(this).data('cleaning') != 0) ? $("input[name='cleaning']").prop('checked', true): $("input[name='cleaning']").prop('checked', false);
            ($(this).data('inspection') != 0) ? $("input[name='inspection']").prop('checked', true): $("input[name='inspection']").prop('checked', false);
            ($(this).data('repair') != 0) ? $("input[name='repair']").prop('checked', true): $("input[name='repair']").prop('checked', false);
            ($(this).data('replace') != 0) ? $("input[name='replace']").prop('checked', true): $("input[name='replace']").prop('checked', false);
            ($(this).data('ndt') != 0) ? $("input[name='ndt']").prop('checked', true): $("input[name='ndt']").prop('checked', false);
            ($(this).data('modif') != 0) ? $("input[name='modif']").prop('checked', true): $("input[name='modif']").prop('checked', false);
            ($(this).data('tein') != 0) ? $("input[name='tiein']").prop('checked', true): $("input[name='tiein']").prop('checked', false);
            ($(this).data('coc') != 0) ? $("input[name='coc']").prop('checked', true): $("input[name='coc']").prop('checked', false);
            ($(this).data('drawing') != 0) ? $("input[name='drawing']").prop('checked', true): $("input[name='drawing']").prop('checked', false);
            ($(this).data('measurement') != 0) ? $("input[name='measurement']").prop('checked', true): $("input[name='drawing']").prop('checked', false);
            ($(this).data('hsse') != 0) ? $("input[name='hsse']").prop('checked', true): $("input[name='drawing']").prop('checked', false);
            ($(this).data('reliability') != 0) ? $("input[name='reliability']").prop('checked', true): $("input[name='drawing']").prop('checked', false);
            ($(this).data('losses') != 0) ? $("input[name='losses']").prop('checked', true): $("input[name='drawing']").prop('checked', false);
            ($(this).data('energi') != 0) ? $("input[name='energi']").prop('checked', true): $("input[name='drawing']").prop('checked', false);
            ($(this).data('energi') != 0) ? $("input[name='energi']").prop('checked', true): $("input[name='drawing']").prop('checked', false);


            ($(this).data('project') != 0) ? $("input[name='project']").prop('checked', true): $("input[name='project']").prop('checked', false);
            ($(this).data('freezing') != 0) ? $("input[name='freezing']").prop('checked', true): $("input[name='freezing']").prop('checked', false);
            ($(this).data('critical_job') != 0) ? $("input[name='critical_job']").prop('checked', true): $("input[name='critical_job']").prop('checked', false);


            var disiplin = $(this).data('disiplin');

            $("input[name=dicipline][value=" + disiplin + "]").prop('checked', true);

            $('#Modal').modal('show');
            $('#btn-sb').text('Edit');
            $('.judul-modal').text('Update Joblist Detail');
            $('#hidden_status').val('edit');
        })

        $(document).on('click', '.delete', function() {
            var id = $(this).data('id');
            Swal.fire({
                title: 'Apakah Anda Yakin?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            confirmButtonText: 'Ya, Hapus!',
            cancelButtonText: 'Tidak',
            }).then((result) => {
                if (result.value) {
                $.ajax({
                    url: "/delete_joblist_detail",
                    type: "POST",
                    data: {
                        id: id
                    },
                    dataType: "JSON",
                    success: function (data) {
                        table.ajax.reload();
                        Swal.fire({
                            title: data.title,
                            text: data.status,
                            icon: data.icon,
                            // timer: 3000,
                            showCancelButton: false,
                            showConfirmButton: true,
                            // buttons: false,
                        });
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        alert('Error');
                    }
                });
                }
            })
        });

        $("#joblist-form").validate({
            errorClass: "is-invalid",
            // validClass: "is-valid",
            rules: {
                projectNo: {
                required: true
                },
                unitCode: {
                    required: true
                },
                unitKilang: {
                    required: true
                },
                eqTagNo: {
                    required: true
                },
            },
            submitHandler: function(form) {
                let url;
                url = '../update_joblist_';
                var form_data = new FormData(document.getElementById("joblist-form"));
                // form_data.append('disiplin',$('#disiplin').find(':selected').attr('data-disiplin'))
                form_data.append('id_joblist', $('#id_joblist').val());
                // form_data.append('unitId', $('#unitKilang').find(':selected').attr('data-id'));
                // form_data.append('codeJob', $('#unitKilang').find(':selected').attr('data-codeJob'));
                // form_data.append('catprof', $('#eqTagNo').find(':selected').attr('data-catprof'));
                form_data.append('projectID', $('#projectNo').find(':selected').attr('data-id'));
                form_data.append('month', $('#projectNo').find(':selected').attr('data-month'));
                form_data.append('year', $('#projectNo').find(':selected').attr('data-year'));
                form_data.append('taoh', $('#projectNo').find(':selected').attr('data-taoh'));
                form_data.append('revision', $('#projectNo').find(':selected').attr('data-revision'));

                checked = $("#joblist_form, input[type=checkbox]:checked").length;
                if(!checked) {
                    Swal.fire({
                        title: 'Peringatan!',
                        text: "Harap Isi data Job Criteria",
                        icon: 'warning',
                        // timer: 3000,
                        showCancelButton: false,
                        showConfirmButton: true,
                        // buttons: false,
                    });
                }else{
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
                                    text: "Silahkan Hubungi Administrator",
                                    icon: 'error',
                                    // timer: 3000,
                                    showCancelButton: false,
                                    showConfirmButton: true,
                                    // buttons: false,
                                });
                                // table.ajax.reload();
                            } else {

                                Swal.fire({
                                    title: 'Berhasil',
                                    text: 'Joblist Berhasil Di update',
                                    icon: 'success',
                                    // timer: 3000,
                                    showCancelButton: false,
                                    showConfirmButton: true,
                                    // buttons: false,
                                });
                            }
                        },
                        error: function (jqXHR, textStatus, errorThrown) {
                            alert('Error adding / update data');
                        }
                    });
                }
            }
        });

        $("#detail-form").validate({
            errorClass: "is-invalid",
            // validClass: "is-valid",
            rules: {
                jobDesc: {
                    required: true
                },
                // alasan: {
                //     required: true
                // },
                revision: {
                    required: true
                },
                inspector: {
                    required: true
                },
                eksekusi: {
                    required: true
                },
                responbility: {
                    required: true
                },
                // ram: {
                //     required: true
                // },
                // memo: {
                //     required: true
                // }

            },
            submitHandler: function(form) {
                let url;
                if ($('#hidden_status').val() == 'add') {
                    url = '../create_joblistdetail_';
                            } else {
                    url = '../update_joblistdetail_';

                }
                var form_data = new FormData(document.getElementById("detail-form"));
                form_data.append('id_joblist', $('#joblist_id').val());
                form_data.append('hidden_job', $('#hidden_job').val());
                //form_data.append('jobno_detail', $('#jobno_detail').val());
                form_data.append('eqtagno_detail', $('#eqtagno_detail').val());
                //form_data.append('no_notif', $('#notifikasi').find(':selected').attr('data-notifikasi'));

                $.ajax({
                    url: url,
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
                                text: "Gagal Tambah / Update Joblist Detail",
                                icon: 'error',
                                // timer: 3000,
                                showCancelButton: false,
                                showConfirmButton: true,
                                // buttons: false,
                            });
                            // table.ajax.reload();
                        } else {

                            Swal.fire({
                                title: 'Berhasil',
                                text: 'Berhasil Menambahkan Detail Joblist',
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
                    error: function(jqXHR, textStatus, errorThrown) {
                        alert('Error adding / update data');
                    }
                });
            }
        });

    });