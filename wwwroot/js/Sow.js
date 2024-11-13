$(document).ready(function () {

    $('.date-own').datepicker({
        minViewMode: 2,
        format: 'yyyy'
    });
    function get_subGroup(groups_, selected) {
        // console.log(groups_)
        $.ajax({
            url: 'get_sow_group',
            method: 'POST',
            data: {
                groups_: groups_,
            },
            success: function (res) {
                $('#subGroup').html(res);
                if (selected != '') {
                    $('#subGroup').val(selected);
                }
            }
        })
    }

    function get_subGroup_filter(groups_) {
        // console.log(groups_)
        $.ajax({
            url: 'get_sow_group',
            method: 'POST',
            data: {
                groups_: groups_,
            },
            success: function (res) {
                $('#subGroup_filter').html(res);
            }
        })
    }

    $.ajaxSetup({
        headers: {
            'X-CSRF-TOKEN': $('meta[name="csrf-token"]').attr('content')
        }
    });

    var table = $('#table').DataTable({
        dom: '<"dataTables_wrapper dt-bootstrap"<"row"<"col-xl-7 d-block d-sm-flex d-xl-block justify-content-center"<"d-block d-lg-inline-flex me-0 me-md-3"l><"d-block d-lg-inline-flex"B>><"col-xl-5 d-flex d-xl-block justify-content-center"fr>>t<"row"<"col-md-5"i><"col-md-7"p>>>',
        processing: true,
        serverSide: true,
        //deferLoading: 0,
        //language: {
        //    "emptyTable": "Data tidak ditemukan - Silahkan Filter data Scope Of Work terlebih dahulu !"
        //},
        "lengthMenu": [
            [30, 60, 100, 200, -1],
            [30, 60, 100, 200, "All"]
        ],
        ajax: {
            url: '/sow_',
            method: 'POST',
            data: function (d) {
                d.project = $('#project_filter').val();
                d.events = $('#events_filter').val();
                d.groups = $('#groups_filter').val();
                d.subGroups = $('#subGroups_filter').val();
                d.area = $('#area_filter').val();
                d.kabo = $('#kabo_filter').val();
                d.tahun = $('#tahun_filter').val();
                d.no = $('#no_filter').val();
            }
        },
        columns: [

            {
                data: 'noSOW',
                name: 'noSOW'
            },
            {
                data: 'jobCode',
                name: 'jobCode'
            },
            {
                data: 'judulPekerjaan',
                name: 'judulPekerjaan'
            },
            {
                data: 'planner',
                name: 'planner'
            },
            {
                data: 'kabo',
                name: 'kabo'
            },
            {
                data: 'events',
                name: 'events'
            },
            {
                data: 'groups',
                name: 'groups'
            },
            {
                data: 'subGroups',
                name: 'subGroups'
            },
            {
                data: 'area',
                name: 'area'
            },
            {
                data: 'tahun',
                name: 'tahun'
            },
            {
                data: 'description',
                name: 'description'
            },
            {
                data: 'createdBy',
                name: 'createdBy'
            },
            {
                data: 'modifyBy',
                name: 'modifyBy'
            },
            {
                "render": function (data, type, full, meta) {
                    var val = "";
                    if (role_ == "superadmin" || role_ == "admin") {
                        val += '<div class="d-flex"><a href="javascript:void(0);" class="btn btn-xs waves-effect waves-light btn-outline-warning edit mr-1" data-id="' + full.id + '" data-events="' + full.events + '" data-groups="' + full.groups + '" data-subGroups="' + full.subGroups + '" data-area="' + full.area + '" data-tahun="' + full.tahun + '" data-projectid="' + full.projectId + '" data-planner="' + full.planner + '" data-judulPekerjaan="' + full.judulPekerjaan + '" data-file="' + full.file + '" data-kabo="' + full.kabo + '"><i class="fas fa-pen fa-xs"></i></a>';
                    }
                    if (role_ == "superadmin" ) {
                        val += '<a href="javascript:void(0);" style="margin-left:5px" class="btn btn-danger btn-xs delete " data-id="' + full.id + '"><i class="fas fa-trash fa-xs"></i></a></div>';
                    }
                    return val;
                        
                },
                orderable: false,
                searchable: false
            },
        
        ],
        columnDefs: [
            (role_ == 'user') ? { "visible": false, "targets": [13] } : {},
        ],
        order: [],
        buttons: (role_ == 'superadmin' || role_ == 'admin') ? [{
            text: '<i class="far fa-edit"></i> New',
            className: 'btn btn-success',
            action: function (e, dt, node, config) {
                $('#add-form')[0].reset();
                $('#Modal').modal('show');
                $('#btn-sb').text('Tambah');
                $('.judul-modal').text('Tambah Scope Of Work');
                $('#hidden_status').val('add');
            }
        },

        {
            extend: 'excel',
            title: 'Scope Of Work',
            className: 'btn',
            text: '<i class="far fa-file-code"></i> Excel',
            titleAttr: 'Excel',
            exportOptions: {
                columns: ':not(:last-child)',
            }
        },


        ] : [{
            extend: 'excel',
            title: 'Scope Of Work',
            className: 'btn',
            text: '<i class="far fa-file-code"></i> Excel',
            titleAttr: 'Excel',
            exportOptions: {
                columns: ':not(:last-child)',
            }
        }]
    });

    $(document).on('click', '.edit', function () {
        $('#add-form')[0].reset();
        $('#groups').val($(this).data('groups'));
        get_subGroup($(this).data('groups'), $(this).data('subgroups'));
        $('#Modal').modal('show');
        $('#btn-sb').text('Update');
        $('.judul-modal').text('Edit Scope Of Work');
        $('#hidden_status').val('edit');
        $('#hidden_id').val($(this).data('id'));
        $('#events').val($(this).data('events'));
        $('#subGroup').val($(this).data('subgroups')).trigger('change');
        $('#area').val($(this).data('area'));
        $('#kabo').val($(this).data('kabo'));
        $('#tahun').val($(this).data('tahun'));
        $('#projectID').val($(this).data('projectid'));
        $('#planner').val($(this).data('planner'));
        $('#file_').val($(this).data('file'));
        $('#judulPekerjaan').val($(this).data('judulpekerjaan'));
    });


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
                    url: "delete_sow",
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

    $(document).on('change', '#groups', function () {
        var groups_ = $(this).val();
        var selected = '';
        get_subGroup(groups_, selected);
    })

    $(document).on('change', '#groups_filter', function () {
        var groups_ = $(this).val();
        get_subGroup_filter(groups_);
    })

    $(document).on('click', '#filter', function () {
        var project = $('#project_filter').find(':selected').data('desc');
        $('#title-sow').html('Scope Of Work ( ' + project + ' )');
        table.ajax.reload();
    })

    $("#add-form").validate({
        errorClass: "is-invalid",
        // validClass: "is-valid",
        rules: {
            events: {
                required: true
            },
            groups_: {
                required: true
            },
            subGroup: {
                required: true
            },
            area: {
                required: true
            },
            kabo: {
                required: true
            },
            tahun: {
                required: true
            },
            projectID: {
                required: true
            },
            planner: {
                required: true
            },
            judulPekerjaan: {
                required: true
            }


        },
        submitHandler: function (form) {
            let url;
            if ($('#hidden_status').val() == 'add') {
                url = 'create_sow';
            } else {
                url = 'update_sow';
            }
            var form_data = new FormData(document.getElementById("add-form"));
            form_data.append('urut', $('#subGroup').find(':selected').attr('data-urut'));
            form_data.append('inisial', $('#groups').find(':selected').attr('data-inisial'));
            form_data.append('event', $('#events').find(':selected').attr('data-event'));
            form_data.append('codeKabo', $('#kabo').find(':selected').attr('data-kabo'));
            form_data.append('project', $('#projectID').find(':selected').attr('data-project'));

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
                            text: "Gagal Tambah / Update Plans",
                            icon: 'error',
                            // timer: 3000,
                            showCancelButton: false,
                            showConfirmButton: true,
                            // buttons: false,
                        });
                        table.ajax.reload();
                    } else {
                        Swal.fire({
                            title: 'Berhasil',
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
                error: function (jqXHR, textStatus, errorThrown) {
                    alert('Error adding / update data');
                }
            });
        }
    });

});