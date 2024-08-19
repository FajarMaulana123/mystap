$(document).ready(function () {
    $.ajaxSetup({
        headers: {
            'X-CSRF-TOKEN': $('meta[name="csrf-token"]').attr('content')
        }
    });

    var url = window.location.pathname;
    var id_url = url.substring(url.lastIndexOf('/') + 1);

    var table = $('#table').DataTable({
        // dom: '<"dataTables_wrapper dt-bootstrap"<"row"<"col-xl-7 d-block d-sm-flex d-xl-block justify-content-center"<"d-block d-lg-inline-flex me-0 me-md-3"l><"d-block d-lg-inline-flex"B>><"col-xl-5 d-flex d-xl-block justify-content-center"fr>>t<"row"<"col-md-5"i><"col-md-7"p>>>',
        scrollY: '400px',
        scrollCollapse: true,
        paging: false,
        searching: false,
        info: false,
        processing: true,
        serverSide: true,
        "lengthMenu": [
            [10, 25, 50, 100, -1],
            [10, 25, 50, 100, "All"]
        ],
        ajax: {
            url: '../list_selected',
            method: 'POST',
            data: function (d) {
                d.project = $('#project_filter').val();
                d.eqTagNo = $('#eqTagNo_filter').val();
                d.disiplin = $('#disiplin_filter').val();
                d.id = id_url;
            }
        },
        columnDefs: [

            //{ className: 'text-center', targets: [7,11,15,16] },
            //    { className: 'text-center', targets: [5,6,12,13,15,17,18,19] },
            //    { "visible": false, "targets": [7,8,9,10,11,14,16] },
            //    {
            //         "targets": 2,
            //         "data":"description",
            //         render: function(data, type, row, meta) {
            //         if (type === 'display') {
            //             data = typeof data === 'string' && data.length > 50 ? data.substring(0,25) + '...' : data;
            //         }
            //             return data;
            //         }
            //     },
            {
                targets: [2],
                createdCell: function (cell) {
                    var $cell = $(cell);


                    $(cell).contents().wrapAll("<div class='content'></div>");
                    var $content = $cell.find(".content");

                    $(cell).append($("<button style='border:none; color: blue; text-align: left; background: url();'>...</button>"));
                    $btn = $(cell).find("button");

                    $content.css({
                        "height": "30px",
                        "overflow": "hidden"
                    })
                    $cell.data("isLess", true);

                    $btn.click(function () {
                        var isLess = $cell.data("isLess");
                        $content.css("height", isLess ? "auto" : "30px")
                        $(this).text(isLess ? "Read less" : "...")
                        $cell.data("isLess", !isLess)
                    })
                }
            },
        ],
        columns: [
            { data: 'check', name: 'check', orderable: false, },
            // {
            //     class: 'dt-control',
            //     orderable: false,
            //     data: null,
            //     defaultContent: '',
            // },
            // { data: 'DT_RowIndex', name: 'DT_RowIndex', orderable: false, searchable: false },
            { data: 'eqTagNo', name: 'joblist.eqTagNo' },
            { data: 'jobDesc', name: 'jobDesc' },
            { data: 'alias', name: 'users.alias' },
            { data: 'status', name: 'status' },
            // {data: 'no_memo', name: 'no_memo', orderable: false, searchable: false},
            {
                data: 'isjasa',
                name: 'isjasa',
            },
            // {data: 'noPaket', name: 'noPaket', orderable: false, searchable: false},
            // {data: 'judul_paket', name: 'judul_paket', orderable: false, searchable: false},
            // {data: 'wo_jasa', name: 'wo_jasa', orderable: false, searchable: false},
            // {data: 'no_po', name: 'no_po', orderable: false, searchable: false},
            // {data: 'no_sp', name: 'no_sp', orderable: false, searchable: false},
            { data: 'status_jasa', name: 'status_jasa' },
            {
                data: 'ismaterial',
                name: 'ismaterial',
            },
            // {data: 'order', name: 'order', orderable: false, searchable: false},
            { data: 'status_material', name: 'status_material' },
            // {data: 'ket_status_material', name: 'ket_status_material'},
            { data: 'all_in_kontrak', name: 'all_in_kontrak' },
            { data: 'lldi', name: 'lldi' },
            // {data: 'alasan', name: 'alasan'},
            { data: 'status_job', name: 'status_job' },
            { data: 'disiplin', name: 'disiplin' },
            // {
            //     data: 'action', 
            //     name: 'action', 
            //     orderable: false, 
            //     searchable: false
            // },
        ],
        order: [[
            1, 'asc'
        ]],

        // buttons: [{
        //         text: 'Create Paket Joblist',
        //         className: 'btn btn-info',
        //         action: function(e, dt, node, config) {
        //             $('#paket-form')[0].reset();
        //             $('#paket').modal('show');
        //         }
        //     },
        //     {   
        //         extend: 'excel',
        //         title: 'Job List',
        //         className: 'btn',
        //         text: '<i class="far fa-file-code"></i> Excel',
        //         titleAttr: 'Excel',
        //         exportOptions: {
        //             columns: ':not(:last-child)',
        //             orthogonal: 'fullNotes'
        //         }
        //     },
        // ]

    });

    $('body').on('click', '.check-all', function () {
        $("input[type='checkbox']").not(this).prop('checked', this.checked);
    });

    $(document).on('click', '#filter', function () {
        table.ajax.reload();
    })

    $("#paket-form").validate({
        errorClass: "is-invalid",
        // validClass: "is-valid",
        rules: {
            no_paket: {
                required: true
            },
            tag_no: {
                required: true
            },
            no_memo: {
                required: true
            },
            disiplin_paket: {
                required: true
            }
        },
        submitHandler: function (form) {
            let url;
            url = '/edit_paket_joblist';
            var form_data = new FormData(document.getElementById("paket-form"));
            var id = [];
            $("input[name='check']:checked").each(function () {
                id.push(this.value);
            });
            form_data.append('id', id);
            form_data.append('id_paket', id_url);
            form_data.append('id_project', $('#project_filter').val());
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
                            $('#paket-form')[0].reset();
                            table.ajax.reload();
                            $("input[type='checkbox']").prop("checked", false);
                            $(".check-all").prop("checked", false);
                            window.location.href = '../paket_joblist';
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        alert('Error adding / update data');
                    }
                });
            } else {
                Swal.fire({
                    title: 'Warning',
                    text: "Pilih Joblist !",
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