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
            url: '/usermanagemant_',
            method: 'POST',
        },
        columns: [
            //{ data: 'DT_RowIndex', name: 'DT_RowIndex', orderable: false, searchable: false },
            { data: 'name', name: 'name' },
            { data: 'username', name: 'username' },
            { data: 'email', name: 'email' },
            { data: 'role', name: 'role' },
            { data: 'lastLogin', name: 'lastLogin' },
            {
                data: 'islocked', name: 'islocked',
                render: function (data, type, full, meta) {

                    d = (full.locked == 1) ? "<span class='badge bg-danger'>Locked</span>" : "<span class='badge bg-warning'>Unlocked</span>";
                    full.status = d;
                    return "<a href='#' class='status' data-id='" + full.id + "'>" + d + "</a>";
                },
            },
            {
                "render": function (data, type, full, meta) {
                    return '<div class="d-flex"><a href="javascript:void(0);" class="btn btn-warning  btn-xs edit mr-1" data-id="' + full.id + '" data-nama="' + full.name + '" data-email="' + full.email + '" data-role="' + full.role + '" data-noPekerja="' + full.noPekerja + '" data-username="' + full.username + '" data-alias="' + full.alias + '" data-plant="' + full.plant + '" data-asal="' + full.asal + '" data-uSection="' + full.uSection + '" data-subSection="' + full.subSection + '" data-status="' + full.status + '" data-statPekerja="' + full.statPekerja + '" ><i class="fas fa-pen fa-xs"></i></a><a href = "javascript:void(0);" style = "margin-left:5px" class="btn btn-danger btn-xs delete " data-id="' + full.id + '" > <i class="fas fa-trash fa-xs"></i></a ></div > ';
                },
                orderable: false,
                searchable: false
            },
        ],
        buttons: [{
            text: '<i class="far fa-edit"></i> New',
            className: 'btn btn-success',
            action: function (e, dt, node, config) {
                $('#add-form')[0].reset();
                $('#Modal').modal('show');
                $('#btn-sb').text('Tambah');
                $('.judul-modal').text('Tambah User Management');
                $('#hidden_status').val('add');
            }
        },

        //{
        //    extend: 'excel',
        //    title: 'User management',
        //    className: 'dt-buttons btn-group flex-wrap',
        //    text: '<i class="far fa-file-code"></i> Excel',
        //    titleAttr: 'Excel',
        //    exportOptions: {
        //        columns: ':not(:last-child)',
        //    }
        //},


        ]
    });

    $('body').on('click', '.check-all-submodul', function () {
        var no = $(this).data('no');
        var submodul = '.submodul-' + no;
        $(submodul + " input[type='checkbox']").not(this).prop('checked', this.checked);
    });

    $('body').on('click', '.checkbox-submodul', function () {
        var no = $(this).data('no_all');
        if ($('.sub-' + no + ':checked').length == $('.sub-' + no).length) {
            $('.all-' + no).prop('checked', true);
        } else {
            $('.all-' + no).prop('checked', false);
        }
    });

    $(document).on('click', '.edit', function () {
        // $('#modul-form')[0].reset();
        $("#add-form").trigger("reset");
        var id = $(this).data('id');
        $('#add-form')[0].reset();
        $('#Modal').modal('show');
        $('#btn-sb').text('Edit');
        $('.judul-modal').text('Edit User Management');
        $("#hidden_id").val(id);
        $("#nama").val($(this).data('nama'));
        $("#username").val($(this).data('username'));
        $("#email").val($(this).data('email'));
        $("#noPekerja").val($(this).data('nopekerja'));
        $("#role").val($(this).data('role'));
        $("#alias").val($(this).data('alias'));
        $("#plant").val($(this).data('plant'));
        $("#asal").val($(this).data('asal'));
        $("#status").val($(this).data('status'));
        $("#statPekerja").val($(this).data('statpekerja'));
        $("#fungsi").val($(this).data('usection'));
        $("#bagian").val($(this).data('subsection'));
        $("#hidden_status").val("edit");
        $.ajax({
            url: '/get_modul',
            type: "POST",
            data: {
                'id': $(this).data('id'),
            },
            success: function (res) {
                var data = JSON.parse(res)
                var data_modul = data.data_modul;
                for (var i = 0; i < data_modul.length; i++) {
                    $("input[name='permission[]']").each(function () {
                        if ($(this).val() === data_modul[i].id_modul) {
                            $(this).prop("checked", true);
                        }
                    });
                }
                $('.checkbox-submodul').each(function (index, obj) {
                    var no = $(this).data('no_all');
                    if ($('.sub-' + no + ':checked').length == $('.sub-' + no).length) {
                        $('.all-' + no).prop('checked', true);
                    } else {
                        $('.all-' + no).prop('checked', false);
                    }
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Error adding / update data');
            }
        });
    });

    $("#add-form").validate({
        errorClass: "is-invalid",
        // validClass: "is-valid",
        rules: {
            nama: {
                required: true
            },
            username: {
                required: true
            },
            email: {
                required: true
            },
            noPekerja: {
                required: true
            },
            password: {
                required: function () {
                    if ($('#hidden_status').val() == 'edit') {
                        return false;
                    } else {
                        return true;
                    }
                },
                minlength: 6
            },
            repassword: {
                required: function () {
                    if ($('#hidden_status').val() == 'edit') {
                        return false;
                    } else {
                        return true;
                    }
                },
                minlength: 6,
                equalTo: "#password",
            },
            alias: {
                required: true,
            },
            plant: {
                required: true,
            },
            role: {
                required: true,
            },
            asal: {
                required: true,
            },
            fungsi: {
                required: true,
            },
            bagian: {
                required: true,
            },
            status: {
                required: true,
            },
            statPekerja: {
                required: true,
            },

        },
        submitHandler: function (form) {
            let url;
            if ($('#hidden_status').val() == 'add') {
                url = '/create_usermanagement';
            } else {
                url = '/update_usermanagement';
            }
            $.ajax({
                url: url,
                type: "POST",
                data: new FormData(document.getElementById("add-form")),
                dataType: "JSON",
                contentType: false,
                cache: false,
                processData: false,
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
                    url: "/delete_user",
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

    $(document).on('click', '.locked', function () {
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
                    url: "/locked_user",
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
    })



});