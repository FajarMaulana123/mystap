$(document).ready(function () {

    $.ajaxSetup({
        headers: {
            'X-CSRF-TOKEN': $('meta[name="csrf-token"]').attr('content')
        }
    });

    var table = $('#table').DataTable({
        dom: '<"dataTables_wrapper dt-bootstrap"<"row"<"col-xl-7 d-block d-sm-flex d-xl-block justify-content-center"<"d-block d-lg-inline-flex me-0 me-md-3"l><"d-block d-lg-inline-flex"B>><"col-xl-5 d-flex d-xl-block justify-content-center"fr>>t<"row"<"col-md-5"i><"col-md-7"p>>>',
        processing: true,
        serverSide: true,
        ajax: {
            url: '/unit_',
            method: 'POST',
        },
        columns: [
            //{ data: 'DT_RowIndex', name: 'DT_RowIndex', orderable: false, searchable: false },
            { data: 'unitPlan', name: 'unitPlan' },
            { data: 'codeJob', name: 'codeJob' },
            { data: 'unitCode', name: 'unitCode' },
            { data: 'unitProses', name: 'unitProses' },
            { data: 'unitKilang', name: 'unitKilang' },
            { data: 'unitGroup', name: 'unitGroup' },
            { data: 'groupName', name: 'groupName' },
            { data: 'unitName', name: 'unitName' },
            {
                "render": function (data, type, full, meta) {
                    return '<div class="d-flex"><a href="javascript:void(0);" class="btn btn-warning  btn-xs edit mr-1" data-id="' + full.id + '"  data-unitPlan="' + full.unitPlan + '" data-codeJob="'+ full.codeJob + '" data-unitCode="' + full.unitCode + '" data-unitProses="' + full.unitProses + '" data-unitKilang="' + full.unitKilang + '" data-unitGroup="' + full.unitGroup + '" data-groupName="' + full.groupName + '" data-unitName="' + full.unitName + '"><i class="fas fa-pen fa-xs"></i></a><a href = "javascript:void(0);" style = "margin-left:5px" class="btn btn-danger btn-xs delete " data-id="' + full.id + '" > <i class="fas fa-trash fa-xs"></i></a ></div > ';
                },
                orderable: false,
                searchable: false
            },
        ],
        //columnDefs: [
        //    (user_auth == 'user') ? { "visible": false, "targets": [9] } : {},
        //],
        buttons:/* (user_auth == 'superadmin' || user_auth == 'admin') ? */[{
            text: '<i class="far fa-edit"></i> New',
            className: 'btn btn-success',
            action: function (e, dt, node, config) {
                $('#add-form')[0].reset();
                $('#Modal').modal('show');
                $('#btn-sb').text('Tambah');
                $('.judul-modal').text('Tambah Unit');
                $('#hidden_status').val('add');
            }
        },

        {
            extend: 'excel',
            title: 'Unit',
            className: 'btn',
            text: '<i class="far fa-file-code"></i> Excel',
            titleAttr: 'Excel',
            exportOptions: {
                columns: ':not(:last-child)',
            }
        },
        ] /*: [{
            extend: 'excel',
            title: 'Unit',
            className: 'btn',
            text: '<i class="far fa-file-code"></i> Excel',
            titleAttr: 'Excel',
            exportOptions: {
                columns: ':not(:last-child)',
            }
        }]*/

    });

    $(document).on('click', '.edit', function () {
        $('#add-form')[0].reset();
        $('#Modal').modal('show');
        $('#btn-sb').text('Update');
        $('.judul-modal').text('Edit Unit');
        $('#hidden_status').val('edit');
        $('#hidden_id').val($(this).data('id'));
        $('#unitPlan').val($(this).data('unitplan'));
        $('#codeJob').val($(this).data('codejob'));
        $('#unitCode').val($(this).data('unitcode'));
        $('#unitProses').val($(this).data('unitproses'));
        $('#unitKilang').val($(this).data('unitkilang'));
        $('#unitGroup').val($(this).data('unitgroup'));
        $('#groupName').val($(this).data('groupname'));
        $('#unitName').val($(this).data('unitname'));
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
                    url: "/delete_unit",
                    type: "POST",
                    data: {
                        id: id
                    },
                    dataType: "JSON",
                    success: function (data) {
                        table.ajax.reload();
                        Swal.fire({
                            title: data.title,
                            html: '<b>' + data.status + "</b>",
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

    $("#add-form").validate({
        errorClass: "is-invalid",
        // validClass: "is-valid",
        rules: {
            unitPlan: {
                required: true
            },
            codeJob: {
                required: true
            },
            unitCode: {
                required: true
            },
            unitProses: {
                required: true
            },
            unitKilang: {
                required: true
            },
            unitGroup: {
                required: true
            },
            groupName: {
                required: true
            },
            unitName: {
                required: true
            }

        },
        submitHandler: function (form) {
            let url;
            if ($('#hidden_status').val() == 'add') {
                url = '/create_unit';
            } else {
                url = '/update_unit';
            }
            var form_data = new FormData(document.getElementById("add-form"));
            form_data.append('unitProses', $('#unitCode').find(':selected').attr('data-unit'))
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
                            text: "Gagal Tambah / Update User",
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