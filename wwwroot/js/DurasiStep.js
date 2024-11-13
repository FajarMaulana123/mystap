$(document).ready(function (project, project_rev ,kat_tender) {

    $.ajaxSetup({
        headers: {
            'X-CSRF-TOKEN': $('meta[name="csrf-token"]').attr('content')
        }
    });

    var table = $('#table').DataTable({
        dom: '<"dataTables_wrapper dt-bootstrap"<"row"<"col-xl-7 d-block d-sm-flex d-xl-block justify-content-center"<"d-block d-lg-inline-flex me-0 me-md-3"l><"d-block d-lg-inline-flex"B>><"col-xl-5 d-flex d-xl-block justify-content-center"fr>>t<"row"<"col-md-5"i><"col-md-7"p>>>',
        // scrollY:        300,
        scrollX: true,
        scrollCollapse: true,
        // paging:         false,
        // fixedColumns:   true,
        processing: true,
        serverSide: true,
        ajax: {
            url: '/durasi_step_',
            method: 'POST',
            data: function (d) {
                d.project = $('#project_filter').val();
                d.kat_tender = $('#kat_tender_filter').val();
            }

        },

        columns: [
            /* { data: 'DT_RowIndex', name: 'DT_RowIndex', orderable: false, searchable: false },*/
            {
                "data": null, orderable: false, "render": function (data, type, full, meta) {
                    return meta.row + 1;
                }
            },
            { data: 'description', name: 'description' },
            { data: 'kat_tender', name: 'kat_tender' },
            { data: 'susun_kak', name: 'susun_kak' },
            { data: 'susun_oe', name: 'susun_oe' },
            { data: 'persetujuan', name: 'persetujuan' },
            { data: 'kirim_ke_co', name: 'kirim_ke_co' },
            { data: 'pengumuman_pendaftaran', name: 'pengumuman_pendaftaran' },
            { data: 'sertifikasi', name: 'sertifikasi' },
            { data: 'prakualifikasi', name: 'prakualifikasi' },
            { data: 'undangan', name: 'undangan' },
            { data: 'pemberian', name: 'pemberian' },
            { data: 'penyampaian', name: 'penyampaian' },
            { data: 'pembukaan', name: 'pembukaan' },
            { data: 'evaluasi', name: 'evaluasi' },
            { data: 'negosiasi', name: 'negosiasi' },
            { data: 'usulan', name: 'usulan' },
            { data: 'keputusan', name: 'keputusan' },
            { data: 'pengumuman_pemenang', name: 'pengumuman_pemenang' },
            { data: 'pengajuan_sanggah', name: 'pengajuan_sanggah' },
            { data: 'jawaban_sanggah', name: 'jawaban_sanggah' },
            { data: 'tunjuk_pemenang', name: 'tunjuk_pemenang' },
            { data: 'proses_spb', name: 'proses_spb' },

            {
                "render": function (data, type, full, meta) {
                    var val = '<div class="d-flex">';
                    if (role_ == "superadmin" || role_ == "admin") {
                        val += '<a href="javascript:void(0);" class="btn btn-warning  btn-xs edit mr-1" data-id="' + full.id + '" data-id_project="' + full.id_project + '" data-kat_tender="' + full.kat_tender + '" data-susun_kak="' + full.susun_kak + '" data-susun_oe="' + full.susun_oe + '" data-persetujuan="' + full.persetujuan + '" data-kirim_ke_co="' + full.kirim_ke_co + '" data-pengumuman_pendaftaran="' + full.pengumuman_pendaftaran + '" data-sertifikasi="' + full.sertifikasi + '" data-prakualifikasi="' + full.prakualifikasi + '" data-undangan="' + full.undangan + '" data-pemberian="' + full.pemberian + '" data-penyampaian="' + full.penyampaian + '" data-pembukaan="' + full.pembukaan + '" data-evaluasi="' + full.evaluasi + '" data-negosiasi="' + full.negosiasi + '" data-usulan="' + full.usulan + '" data-keputusan="' + full.keputusan + '" data-pengumuman_pemenang="' + full.pengumuman_pemenang + '" data-pengajuan_sanggah="' + full.pengajuan_sanggah + '" data-jawaban_sanggah="' + full.jawaban_sanggah + '" data-tunjuk_pemenang="' + full.tunjuk_pemenang + '" data-proses_spb="' + full.proses_spb + '"><i class="fas fa-pen fa-xs"></i></a>';
                    }
                    if (role_ == "superadmin") {
                        val += '<a href = "javascript:void(0);" style = "margin-left:5px" class="btn btn-danger btn-xs delete " data-id="' + full.id + '" > <i class="fas fa-trash fa-xs"></i></a>';
                    }
                    val += '</div>';
                    return val;
                },
                orderable: false,
                searchable: false
            },
        ],
        // fixedColumns:   {
        //     left: 3,
        //     right: 1
        // },
        columnDefs: [
            {
                targets: [2, 6, 10],
                className: 'text-wrap width-200'


            },
            (role_ == 'user') ? { "visible": false, "targets": [23] } : {},
        ],
        buttons: (role_ == 'superadmin' || role_ == 'admin') ? [{
            text: '<i class="far fa-edit"></i> New',
            className: 'btn btn-success',
            action: function (e, dt, node, config) {
                $('#add-form')[0].reset();
                $('#Modal').modal('show');
                $('#btn-sb').text('Tambah');
                $("#id_project").prop("disabled", false);
                $("#kat_tender").prop("disabled", false);
                $('.judul-modal').text('Tambah Durasi Step');
                $('#hidden_status').val('add');
            }
        },

        {
            extend: 'excel',
            title: 'SLA / Durasi Step',
            className: 'btn btn-outline-primary',
            text: '<i class="far fa-file-code"></i> Excel',
            titleAttr: 'Excel',
            exportOptions: {
                columns: ':not(:last-child)',
            }
        }] : [{
            extend: 'excel',
            title: 'Project',
            className: 'btn btn-outline-primary',
            text: '<i class="far fa-file-code"></i> Excel',
            titleAttr: 'Excel',
            exportOptions: {
                columns: ':not(:last-child)',
            }
        }]
    });
    // table.button( 0 ).nodes().css('height', '35px')

    //$(document).on('click', '#tambah', function () {
    //    $('#add-form')[0].reset();
    //    $('.judul-modal').html('Tambah');
    //    $('#hidden_status').val('add');
    //})

    $(document).on('click', '.edit', function () {
        $('#add-form')[0].reset();
        $('#Modal').modal('show');
        $('.judul-modal').text('Update');
        $('#hidden_status').val('edit');
        $('#id_project').prop('disabled', 'disabled');
        $('#kat_tender').prop('disabled', 'disabled');
        $('#hidden_id').val($(this).data('id'));
        $('#id_project').val($(this).data('id_project'));
        $('#kat_tender').val($(this).data('kat_tender'));
        $('#susun_kak').val($(this).data('susun_kak'));
        $('#susun_oe').val($(this).data('susun_oe'));
        $('#persetujuan').val($(this).data('persetujuan'));
        $('#kirim_ke_co').val($(this).data('kirim_ke_co'));
        $('#pengumuman_pendaftaran').val($(this).data('pengumuman_pendaftaran'));
        $('#sertifikasi').val($(this).data('sertifikasi'));
        $('#prakualifikasi').val($(this).data('prakualifikasi'));
        $('#undangan').val($(this).data('undangan'));
        $('#pemberian').val($(this).data('pemberian'));
        $('#penyampaian').val($(this).data('penyampaian'));
        $('#pembukaan').val($(this).data('pembukaan'));
        $('#evaluasi').val($(this).data('evaluasi'));
        $('#negosiasi').val($(this).data('negosiasi'));
        $('#usulan').val($(this).data('usulan'));
        $('#keputusan').val($(this).data('keputusan'));
        $('#pengumuman_pemenang').val($(this).data('pengumuman_pemenang'));
        $('#pengajuan_sanggah').val($(this).data('pengajuan_sanggah'));
        $('#jawaban_sanggah').val($(this).data('jawaban_sanggah'));
        $('#tunjuk_pemenang').val($(this).data('tunjuk_pemenang'));
        $('#proses_spb').val($(this).data('proses_spb'));

    });


    $(document).on('click', '.delete', function () {
        var id = $(this).data('id');
        swal({
            title: 'Are you sure?',
            text: 'Apakah Anda Yakin!',
            icon: 'warning',
            buttons: {
                cancel: {
                    text: 'Cancel',
                    value: null,
                    visible: true,
                    className: 'btn btn-default',
                    closeModal: true,
                },
                confirm: {
                    text: 'Primary',
                    value: true,
                    visible: true,
                    className: 'btn btn-primary',
                    closeModal: true
                }
            }
        })
            .then((result) => {
                if (result.value) {
                    $.ajax({
                        url: "/delete_durasi_step",
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

    $("#add-form").validate({
        errorClass: "is-invalid",
        // validClass: "is-valid",
        rules: {
            //plant: {
            //    required: true
            //},
            //year: {
            //    required: true
            //},
            //execution_date: {
            //    required: true
            //},
            //finish_date: {
            //    required: true
            //},
            //revision: {
            //    required: true
            //},
            //description: {
            //    required: true
            //},
            //durasiTABrick: {
            //    required: true
            //},
            //section: {
            //    required: true
            //}


        },
        submitHandler: function (form) {
            let url;
            if ($('#hidden_status').val() == 'add') {
                url = '/create_durasi_step';
            } else {
                url = '/update_durasi_step';
            }

            var form_data = new FormData(document.getElementById("add-form"));


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
                    if (data.result == false) {
                        Swal.fire({
                            title: 'Gagal',
                            text: "Gagal Tambah / Update SLA",
                            icon: 'error',
                            // timer: 3000,
                            showCancelButton: false,
                            showConfirmButton: true,
                            // buttons: false,
                        });
                    } else if (data.result == true) {
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
                    } else if (data.result == 'ada') {
                        Swal.fire({
                            title: 'Gagal!',
                            icon: 'error',
                            text: "Maaf Data Sudah Ada !!",
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