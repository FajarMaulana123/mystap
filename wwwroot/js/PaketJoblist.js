
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
        deferLoading: 0,
        language: {
            "emptyTable": "Data tidak ditemukan - Silahkan Filter data Planning Joblist terlebih dahulu !"
        },
        "lengthMenu": [
            [30, 60, 100, 200, -1],
            [30, 60, 100, 200, "All"]
        ],
        ajax: {
            url: 'paket_joblist_',
            method: 'POST',
            data: function (d) {
                d.project_filter = $('#project_filter').val();
                d.project_rev = $('#project_filter').find(':selected').data('rev');
                d.disiplin = $('#disiplin_').val();
                d.tag_no = $('#tag_no_').val();

            }
        },

        columns: [
            {
                class: 'dt-control',
                orderable: false,
                data: null,
                defaultContent: '',
            },
            { data: 'no_add', name: 'no_add' },
            { data: 'tag_no', name: 'tag_no' },
            { data: 'no_memo', name: 'no_memo' },
            { data: 'disiplin', name: 'disiplin' },
            { data: 'total', name: 'total' },
            {
                data: 'status_tagno', name: 'status_tagno',
                render: function (data, type, full, meta) {
                    var s = "";
                    if (full.status_tagno == 'ready') {
                        s = '<span class="badge bg-success">Ready</span>';
                    } else if (full.status_tagno == 'not_ready') {
                        s = '<span class="badge bg-danger">Not Ready</span>';
                    } else if (full.status_tagno == 'not_identify') {
                        s = 'N/R';
                    } else if (full.status_tagno == 'not_execute') {
                        s = '<span class="badge bg-black">Not Execute</span>';
                    } else {
                        s = '<span class="badge bg-secondary">Undefined</span>';
                    }

                    return s;

                },
            },
            {
                orderable: false, searchable: false,
                render: function (data, type, full, meta) {
                    var val = '<div class="d-flex">';
                    if (role_ == "superadmin" || role_ == "admin") {
                        val += '<a href="edit_paket_joblist_/' + full.id_paket + '" class="btn btn-xs waves-effect waves-light btn-outline-warning edit mr-1" ><i class="fas fa-pen fa-xs"></i></a>';
                    }
                    if (role_ == "superadmin") {
                        val += '<a href="javascript:void(0);" style="margin-left:5px" class="btn btn-xs waves-effect waves-light btn-outline-danger delete " data-id="' + full.id_paket + '"><i class="fas fa-trash fa-xs"></i></a>';
                    }
                    val += '</div>';
                        
                    return val;
                }
            },
        ],
        columnDefs: [
            { className: 'text-center', targets: [5, 6] },
            (role_ == 'user') ? { "visible": false, "targets": [7] } : {},
        ],
        order: [],
        buttons:
            (role_ == 'superadmin' || role_ == 'admin') ? 
        [{
            text: '<i class="far fa-edit"></i> New',
            className: 'btn btn-success',
            action: function (e, dt, node, config) {
                window.location.href = 'create_paket_joblist_';
            }
        },
        {
            extend: 'excel',
            title: 'Data Paket Joblist',
            className: 'btn',
            text: '<i class="far fa-file-code"></i> Excel',
            titleAttr: 'Excel',
            exportOptions: {
                columns: ':not(:last-child)',
                orthogonal: 'fullNotes'
            }
        },
        ] 
        : [{
            extend: 'excel',
            title: 'Data Paket Joblist',
            className: 'btn',
            text: '<i class="far fa-file-code"></i> Excel',
            titleAttr: 'Excel',
            exportOptions: {
                columns: ':not(:last-child)',
                orthogonal: 'fullNotes'
            }
        }]
    });

    function format(d) {
        
        var div = $('<div/>')
            .addClass('loading')
            .text('Loading...');

        $.ajax({
            url: '/detail_paket_joblist',
            type: 'POST',
            data: {
                id_project: d.id_project,
                project_rev: d.project_rev,
                id_paket: d.id_paket,
            },
            dataType: 'json',
            success: function (json) {
                div
                    .html(json.data)
                    .removeClass('loading');
            }
        });

        return div;
    }

    $('#table tbody').on('click', 'td.dt-control', function () {
        var tr = $(this).closest('tr');
        var row = table.row(tr);

        if (row.child.isShown()) {
            row.child.hide();
            tr.removeClass('shown');
        } else {
            row.child(format(row.data())).show();
            tr.addClass('shown');
        }
    });


    $(document).on('click', '#filter', function () {
        var project = $('#project_filter').find(':selected').data('desc');
        table.ajax.reload();
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
                    url: "delete_paket_joblist",
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

});