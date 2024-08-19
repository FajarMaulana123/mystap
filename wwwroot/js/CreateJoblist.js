function get_memo(project) {
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

function get_kilang(unitCode) {
    $.ajax({
        url: 'getUnitKilang',
        method: 'POST',
        data: {
            unitCode: unitCode,
        },
        success: function (res) {
            $('#unitKilang').html(res);
        }
    })
}

function get_eqtagno(unitCode, unitKilang) {
    $.ajax({
        url: 'get_eqtagno',
        method: 'POST',
        data: {
            unitCode: unitCode,
            unitKilang: unitKilang,
        },
        success: function (res) {
            $('#eqTagNo').html(res);
        }
    })
}

$(document).ready(function () {

    $(".select2").select2();
    $(".select3").select2({
        dropdownParent: $("#Modal")
    });
    $(".multiple-select2").select2({ placeholder: "Select Memo", dropdownParent: $("#Modal") });
    $.ajaxSetup({
        headers: {
            'X-CSRF-TOKEN': $('meta[name="csrf-token"]').attr('content')
        }
    });

    $(document).on('change', '#projectNo', function () {
        var id_project = $(this).find(':selected').attr('data-id');
        if ($(this).find(':selected').attr('data-taoh') == 'OH') {
            document.getElementById('oh').style.display = 'block';
        } else {
            document.getElementById('oh').style.display = 'none';
        }
        get_memo(id_project);

    })

    $(document).on('change', '#unitCode', function () {
        var unitCode = $(this).val();
        get_kilang(unitCode);
    })

    $(document).on('change', '#unitKilang', function () {
        var unitCode = $('#unitCode').val();
        var unitKilang = $(this).val();
        get_eqtagno(unitCode, unitKilang);
    })

    $(document).on('change', '#eqTagNo', function () {
        var eq = $(this).val();
        get_notifikasi(eq);
    })

    var table = $('#table').DataTable({
        dom: '<"dataTables_wrapper dt-bootstrap"<"row"<"col-xl-7 d-block d-sm-flex d-xl-block justify-content-center"<"d-block d-lg-inline-flex me-0 me-md-3"l><"d-block d-lg-inline-flex"B>><"col-xl-5 d-flex d-xl-block justify-content-center"fr>>t<"row"<"col-md-5"i><"col-md-7"p>>>',
        ajax: {
            url: '/joblistdetail_',
            type: 'POST',
            data: function (d) {
                d.jobno = $('#jobno_detail').val();
                d.joblist_id = $('#joblist_id').val();
            }
        },
        processing: true,
        serverSide: true,
        deferLoading: 0,
        language: {
            "emptyTable": "Data tidak ditemukan - Silahkan Mengisi data Job List terlebih dahulu !"
        },
        columnDefs: [

            { className: 'text-center', targets: [5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19] },
        ],
        columns: [
            { data: 'jobNo', name: 'jobNo' },
            { data: 'jobDesc', name: 'jobDesc' },
            { data: 'disiplin', name: 'disiplin' },
            { data: 'status', name: 'status' },
            { data: 'alasan', name: 'alasan' },
            { data: 'cln', name: 'cln' },
            { data: 'ins', name: 'ins' },
            { data: 'rpr', name: 'rpr' },
            { data: 'rpl', name: 'rpl' },
            { data: 'ndt', name: 'ndt' },
            { data: 'tein', name: 'tein' },
            { data: 'msr', name: 'msr' },
            { data: 'dwg', name: 'dwg' },
            { data: 'mod', name: 'mod' },
            // {data: 'pnt', name: 'pnt'},
            { data: 'coc', name: 'coc' },
            { data: 'hsse', name: 'hsse' },
            { data: 'rel', name: 'rel' },
            { data: 'los', name: 'los' },
            { data: 'eng', name: 'eng' },
            {
                data: 'action',
                name: 'action',
                orderable: false,
                searchable: false
            },
        ],
        columnDefs: [
            {
                targets: [1],
                className: 'text-wrap width-200'


            }
        ],
        buttons: [
            {
                text: '<i class="far fa-edit"></i> New',
                className: 'btn btn-outline-primary ',
                enabled: ($('#hidden_job').val() != 'no') ? true : false,
                action: function (e, dt, node, config) {
                    $('#detail-form')[0].reset();
                    // $().not("#jobno_detail,#eqtagno_detail,#joblist_id,#hidden_job").val();
                    $('#detail-form', 'input:checkbox').prop('checked', false);
                    $('#detail-form', 'input:radio').prop('checked', false);

                    $('#memo').val([]).trigger('change');
                    $("#notifikasi").val('').trigger('change');

                    $('#Modal').modal('show');
                    $('#btn-sb').text('Tambah');
                    $('.judul-modal').text('Tambah Joblist Detail');
                    $('#hidden_status').val('add');
                }
            },

            {
                extend: 'excel',
                title: 'Joblist Detail',
                className: 'btn btn-outline-primary',
                text: '<i class="far fa-file-code"></i> Excel',
                titleAttr: 'Excel',
                exportOptions: {
                    columns: ':not(:last-child)',
                }
            },

        ]

    });

    $(document).on('click', '.edit', function () {
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

        $("#notifikasi option[data-notifikasi='" + $(this).data('notif') + "']").attr("selected", "selected");

        ($(this).data('cleaning') != 0) ? $("input[name='cleaning']").prop('checked', true) : $("input[name='cleaning']").prop('checked', false);
        ($(this).data('inspection') != 0) ? $("input[name='inspection']").prop('checked', true) : $("input[name='inspection']").prop('checked', false);
        ($(this).data('repair') != 0) ? $("input[name='repair']").prop('checked', true) : $("input[name='repair']").prop('checked', false);
        ($(this).data('replace') != 0) ? $("input[name='replace']").prop('checked', true) : $("input[name='replace']").prop('checked', false);
        ($(this).data('ndt') != 0) ? $("input[name='ndt']").prop('checked', true) : $("input[name='ndt']").prop('checked', false);
        ($(this).data('modif') != 0) ? $("input[name='modif']").prop('checked', true) : $("input[name='modif']").prop('checked', false);
        ($(this).data('tein') != 0) ? $("input[name='tiein']").prop('checked', true) : $("input[name='tiein']").prop('checked', false);
        ($(this).data('coc') != 0) ? $("input[name='coc']").prop('checked', true) : $("input[name='coc']").prop('checked', false);
        ($(this).data('drawing') != 0) ? $("input[name='drawing']").prop('checked', true) : $("input[name='drawing']").prop('checked', false);
        ($(this).data('measurement') != 0) ? $("input[name='measurement']").prop('checked', true) : $("input[name='drawing']").prop('checked', false);
        ($(this).data('hsse') != 0) ? $("input[name='hsse']").prop('checked', true) : $("input[name='drawing']").prop('checked', false);
        ($(this).data('reliability') != 0) ? $("input[name='reliability']").prop('checked', true) : $("input[name='drawing']").prop('checked', false);
        ($(this).data('losses') != 0) ? $("input[name='losses']").prop('checked', true) : $("input[name='drawing']").prop('checked', false);
        ($(this).data('energi') != 0) ? $("input[name='energi']").prop('checked', true) : $("input[name='drawing']").prop('checked', false);
        ($(this).data('energi') != 0) ? $("input[name='energi']").prop('checked', true) : $("input[name='drawing']").prop('checked', false);

        ($(this).data('project') != 0) ? $("input[name='project']").prop('checked', true) : $("input[name='project']").prop('checked', false);
        ($(this).data('freezing') != 0) ? $("input[name='freezing']").prop('checked', true) : $("input[name='freezing']").prop('checked', false);
        ($(this).data('critical_job') != 0) ? $("input[name='critical_job']").prop('checked', true) : $("input[name='critical_job']").prop('checked', false);

        var disiplin = $(this).data('disiplin');

        $("input[name=dicipline][value=" + disiplin + "]").prop('checked', true);

        $('#Modal').modal('show');
        $('#btn-sb').text('Edit');
        $('.judul-modal').text('Update Joblist Detail');
        $('#hidden_status').val('edit');
    })

    $(document).on('click', '.delete', function () {
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

    $(document).on('change', '#eqTagNo', function () {
        $.ajax({
            url: "/cek_eqtagno",
            type: "POST",
            data: {
                eqtagno: $(this).val(),
                project: $('#projectNo').find(':selected').attr('data-id'),
            },
            success: function (data) {
                if (data.result == true) {
                    Swal.fire({
                        title: 'Apakah Anda ingin Melanjutkan?',
                        icon: 'warning',
                        text: 'Data Joblist Sudah Ada',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        confirmButtonText: 'Ya, Lanjutkan!',
                        cancelButtonText: 'Tidak',
                    }).then((result) => {
                        if (result.isConfirmed) {
                            console.log(result.value)
                            $('#joblist_id').val(data.joblist_id);
                            $('#jobno_detail').val(data.jobNo);
                            $('#eqtagno_detail').val(data.eqTagNo);
                            $('#remarks').val(data.remarks);
                            $('#hidden_job').val('yes');
                            $('#projectNo').prop('disabled', 'disabled');
                            $('#unitCode').prop('disabled', 'disabled');
                            $('#unitKilang').prop('disabled', 'disabled');

                            (data.criteriaMI != 0) ? $("input[name='criteriaMI']").prop('checked', true) : $("input[name='criteriaMI']").prop('checked', false);
                            (data.criteriaPI != 0) ? $("input[name='criteriaPI']").prop('checked', true) : $("input[name='criteriaPI']").prop('checked', false);
                            (data.criteriaOPT != 0) ? $("input[name='criteriaOPT']").prop('checked', true) : $("input[name='criteriaOPT']").prop('checked', false);
                            // $('#eqTagNo').prop('disabled', 'disabled');
                            Swal.fire({
                                title: 'Berhasil',
                                icon: 'success',
                                showCancelButton: false,
                                showConfirmButton: true
                            });

                            table.buttons().enable(true);
                            table.ajax.reload();
                        } else {
                            window.location.href = 'joblist';
                        }
                        // 
                    })
                }
            }
        })
    })

    $("#joblist-form").validate({
        errorClass: "is-invalid",
        highlight: function (element, errorClass, validClass) {
            $(element).parents('.form-control').removeClass('is-valid').addClass('is-invalid');
        },
        unhighlight: function (element, errorClass, validClass) {
            $(element).parents('.form-control').removeClass('is-invalid').addClass('is-valid');
        },
        errorPlacement: function (error, element) {
            if (element.hasClass('select2') && element.next('.select2-container').length) {
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
            start_date: {
                required: function (element) {
                    if ($('#projectNo').find(':selected').attr('data-taoh') == 'OH') {
                        return true
                    } else {
                        return false
                    }
                }
            }
            // remarks: {
            //     required: true
            // }

        },
        submitHandler: function (form) {
            let url;
            if ($('#hidden_job').val() == 'yes') {
                url = 'update_joblist_';
            } else {
                url = 'create_joblist_';
            }
            var form_data = new FormData(document.getElementById("joblist-form"));
            // form_data.append('disiplin',$('#disiplin').find(':selected').attr('data-disiplin'))
            form_data.append('unitId', $('#unitKilang').find(':selected').attr('data-id'));
            form_data.append('codeJob', $('#unitKilang').find(':selected').attr('data-codeJob'));
            form_data.append('catprof', $('#eqTagNo').find(':selected').attr('data-catprof'));
            form_data.append('projectID', $('#projectNo').find(':selected').attr('data-id'));
            form_data.append('month', $('#projectNo').find(':selected').attr('data-month'));
            form_data.append('year', $('#projectNo').find(':selected').attr('data-year'));
            form_data.append('taoh', $('#projectNo').find(':selected').attr('data-taoh'));
            form_data.append('revision', $('#projectNo').find(':selected').attr('data-revision'));

            checked = $("#joblist_form, input[type=checkbox]:checked").length;
            if (!checked) {
                Swal.fire({
                    title: 'Peringatan!',
                    text: "Harap Isi data Job Criteria",
                    icon: 'warning',
                    showCancelButton: false,
                    showConfirmButton: true,
                    // buttons: false,
                });
            } else {
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
                                text: "Silahkan Menghubungi Administrator",
                                icon: 'error',
                                // timer: 3000,
                                showCancelButton: false,
                                showConfirmButton: true,
                                // buttons: false,
                            });
                            // table.ajax.reload();
                        } else {
                            // console.log(, data.eqTagNo);
                            $('#joblist_id').val(data.joblist_id);
                            $('#jobno_detail').val(data.jobNo);
                            $('#eqtagno_detail').val(data.eqTagNo);
                            $('#hidden_job').val('yes');
                            $('#projectNo').prop('disabled', 'disabled');
                            $('#unitCode').prop('disabled', 'disabled');
                            $('#unitKilang').prop('disabled', 'disabled');
                            $('#eqTagNo').prop('disabled', 'disabled');
                            Swal.fire({
                                title: 'Berhasil',
                                text: 'Joblist Berhasil Ditambahkan - Silahkan Mengisi Detail Joblist',
                                icon: 'success',
                                // timer: 3000,
                                showCancelButton: false,
                                showConfirmButton: true
                            });
                            table.buttons().enable(true);
                            table.ajax.reload();
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
        highlight: function (element, errorClass, validClass) {
            $(element).parents('.form-control').removeClass('is-valid').addClass('is-invalid');
        },
        unhighlight: function (element, errorClass, validClass) {
            $(element).parents('.form-control').removeClass('is-invalid').addClass('is-valid');
        },
        errorPlacement: function (error, element) {
            if (element.hasClass('select2') && element.next('.select2-container').length) {
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
            jobDesc: {
                required: true
            },
            // alasan: {
            //     required: false
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
            //     required: false
            // },
            memo: {
                required: true
            }

        },
        submitHandler: function (form) {
            let url;
            if ($('#hidden_status').val() == 'add') {
                url = 'create_joblistdetail_';
            } else {
                url = 'update_joblistdetail_';

            }
            var form_data = new FormData(document.getElementById("detail-form"));
            form_data.append('joblist_id', $('#joblist_id').val());
            form_data.append('hidden_job', $('#hidden_job').val());
            form_data.append('jobno_detail', $('#jobno_detail').val());
            form_data.append('eqtagno_detail', $('#eqtagno_detail').val());
            form_data.append('no_notif', $('#notifikasi').find(':selected').attr('data-notifikasi'));

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
                            icon: 'success',
                            // timer: 3000,
                            showCancelButton: false,
                            showConfirmButton: true
                        });
                        $('#Modal').modal('hide');
                        table.ajax.reload();
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert('Error adding / update data');
                }
            });
        }
    });

});