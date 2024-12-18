﻿$(document).ready(function () {

    $.ajaxSetup({
        headers: {
            'X-CSRF-TOKEN': $('meta[name="csrf-token"]').attr('content')
        }
    });

    var table = $('#table').DataTable({
        dom: '<"dataTables_wrapper dt-bootstrap"<"row"<"col-xl-7 d-block d-sm-flex d-xl-block justify-content-center"<"d-block d-lg-inline-flex me-0 me-md-3"l><"d-block d-lg-inline-flex"B>><"col-xl-5 d-flex d-xl-block justify-content-center"fr>>t<"row"<"col-md-5"i><"col-md-7"p>>>',
        processing: true,
        serverSide: true,
        deferLoading: 0,
        language: {
            "emptyTable": "Data tidak ditemukan - Silahkan Filter data Job List terlebih dahulu !"
        },
        "lengthMenu": [
            [30, 60, 100, 200, -1],
            [30, 60, 100, 200, "All"]
        ],
        ajax: {
            "url": "joblist_",
            "method": "POST",
            "datatype": "json",
            "data": function (d) {
                d.project = $('#project_filter').val();
                d.project_rev = $('#project_filter').find(':selected').data('rev');
                d.jobNo = $('#jobNo_filter').val();
                d.eqTagNo = $('#eqTagNo_filter').val();
                d.unitCode = $('#unitCode_filter').val();
                d.user_section = $('#user_section_filter').val();
            }
        },
        columns: [
            //{ data: 'DT_RowIndex', name: 'DT_RowIndex', orderable: false, searchable: false },
            //{
            //    "data": null, orderable: false, "render": function (data, type, full, meta) {
            //        return meta.row + 1;
            //    }
            //},
            { data: 'jobNo', name: 'jobNo' },
            { data: 'projectNo', name: 'projectNo' },
            { data: 'description', name: 'project.description' },
            { data: 'nama_unit', name: 'unit.unitCode' },
            { data: 'eqTagNo', name: 'eqTagNo' },
            {
                data: 'status_tagno', name: 'status_tagno',
                render: function (data, type, full, meta) {
                    
                    if (full.status_tagno == 'ready') {
                        full.status_tagno = '<span class="badge bg-success">READY</span>';
                    } else if (full.status_tagno == 'not_ready') {
                        full.status_tagno = '<span class="badge bg-danger">NOT READY</span>';
                    } else {
                        full.status_tagno = '<span class="badge bg-secondary">UNDEFINED</span>';
                    } return full.status_tagno;

                },
            },
           
            { data: 'userSection', name: 'userSection' },
            { data: 'keterangan', name: 'keterangan' },
            { data: 'name', name: 'users.name' },
            {
                orderable: false,
                searchable: false,
                "render": function (data, type, full, meta) {
                    var val = '<div class="d-flex">';
                    if (role_ == "superadmin" || role_ == "admin") {
                        val += '<a href="javascript:void(0);" class="btn btn-xs waves-effect waves-light btn-outline-primary carry_offer mr-1" data-id="' + full.id + '" data-eqtagno="' + full.id_eqtagno + '"><i class="fas fa-dolly fa-xs"></i></a>';
                    }
                    if (role_ == "superadmin") {
                        val += '<a href="update_joblist/' + full.id + '" style = "margin-left:5px" class="btn btn-xs waves-effect waves-light btn-outline-warning edit "> <i class="fas fa-pen fa-xs"></i></a>';
                    }
                    val += '</div>';
                        
                    return val;
                },
                
            },
        ],
        columnDefs: [
            { className: 'text-center', targets: [5, 6] },
            (role_ == 'user') ? { "visible": false, "targets": [9] } : {},
        ],
        buttons: (role_ == 'superadmin' || role_ == 'admin') ? [{
            text: '<i class="far fa-edit"></i> New',
            className: 'btn btn-success',
            action: function (e, dt, node, config) {
                window.location.href = 'create_joblist';
            }
        },
        {
            extend: 'excel',
            title: 'Job List',
            className: 'btn',
            text: '<i class="far fa-file-code"></i> Excel',
            titleAttr: 'Excel',
            exportOptions: {
                columns: ':not(:last-child)',
            }
        }] : [{
            extend: 'excel',
            title: 'Job List',
            className: 'btn',
            text: '<i class="far fa-file-code"></i> Excel',
            titleAttr: 'Excel',
            exportOptions: {
                columns: ':not(:last-child)',
            }
        }]

    });

    $(document).on('click', '.carry_offer', function () {
        $('#carry-form')[0].reset();
        $('#carry').modal('show');
        $('#hidden_id').val($(this).data('id'));
        $('#eqtagno').val($(this).data('eqtagno'));
        $('#hidden_status').val('add');
    })

    $(document).on('click', '.delete', function () {
        var id = $(this).data('id');
        Swal.fire({
            title: 'Apakah Anda Yakin?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            confirmButtonText: 'Ya',
            cancelButtonText: 'Tidak',
        }).then((result) => {
            if (result.value) {
                $.ajax({
                    url: "delete_joblist",
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

    $(document).on('change', '#unitCode', function () {
        var unitCode = $(this).val();
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
    })

    $("#carry-form").validate({
        errorClass: "is-invalid",
        // validClass: "is-valid",
        rules: {
            project: {
                required: true
            },
            keterangan: {
                required: true
            }
        },
        submitHandler: function (form) {

            var form_data = new FormData(document.getElementById("carry-form"));
            $.ajax({
                url: "/carry_offer_joblist",
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
                    if (data.result == false) {
                        Swal.fire({
                            title: 'Gagal',
                            text: "Silahkan Hubungi Administrator !",
                            icon: 'error',
                            // timer: 3000,
                            showCancelButton: false,
                            showConfirmButton: true,
                            // buttons: false,
                        });
                    }
                    if (data.result == true) {
                        Swal.fire({
                            title: 'Berhasil',
                            icon: 'success',
                            // timer: 3000,
                            showCancelButton: false,
                            showConfirmButton: true,
                            // buttons: false,
                        });
                        $('#carry').modal('hide');
                        table.ajax.reload();
                    }
                    if (data.result == 'ada') {
                        Swal.fire({
                            title: 'EqTagNo Sudah Ada !',
                            icon: 'warning',
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
    });

    $(document).on('click', '#filter', function () {
        table.ajax.reload();
    })


});