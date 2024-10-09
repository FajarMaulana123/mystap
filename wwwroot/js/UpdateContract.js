var status_curr = '';
function getKatTender(project) {
    $.ajax({
        url: '../getKatTender',
        type: 'POST',
        data: {
            project: project
        },
        success: function (res) {
            $('#katTender').html(res);
            $('#katTender').val($('#hidden_kattender').val()).trigger('change')
        }
    })
}

function count_prognosa(prognosa, durasi) {
    var date = new Date(prognosa);
    return date.setDate(date.getDate() + durasi);
}

function prognosa_kak() {
    var durasi_ = $('#susun_oe').val();
    if ($('#aktual_susun_kak').val() == '') {
        var prognosa = $('#target_susun_kak').val();
    } else {
        var prognosa = $('#aktual_susun_kak').val();
        status_curr = 'PENYUSUNAN KAK';
        $('#current').val(status_curr);
    }
    $('#prognosa_susun_kak').val(prognosa);
    let date_prognosa = count_prognosa(prognosa, parseInt(durasi_));
    $('#prognosa_susun_oe').val(formatDate(date_prognosa));
    console.log(formatDate(date_prognosa), durasi_, prognosa);
}
function prognosa_oe() {
    var durasi_ = $('#persetujuan').val();
    if ($('#aktual_susun_oe').val() == '') {
        var prognosa = $('#prognosa_susun_oe').val();
    } else {
        var prognosa = $('#aktual_susun_oe').val();
        $('#prognosa_susun_oe').val(prognosa);
        status_curr = 'PENYUSUNAN OE';
        $('#current').val(status_curr);
    }
    let date_prognosa = count_prognosa(prognosa, parseInt(durasi_));
    $('#prognosa_persetujuan').val(formatDate(date_prognosa));
}

function prognosa_perizinan() {
    var durasi_ = $('#kirim_ke_co').val();
    if ($('#akt_persetujuan').val() == '') {
        var prognosa = $('#prognosa_persetujuan').val();
    } else {
        var prognosa = $('#akt_persetujuan').val();
        status_curr = 'PERSETUJUAN';
        $('#current').val(status_curr);
    }
    let date_prognosa = count_prognosa(prognosa, parseInt(durasi_));
    $('#prognosa_kirim_paket_ke_co').val(formatDate(date_prognosa));
}

function prognosa_kirim_co() {
    var durasi_ = $('#pengumuman_pendaftaran').val();
    if ($('#aktual_kirim_paket_ke_co').val() == '') {
        var prognosa = $('#prognosa_kirim_paket_ke_co').val();
    } else {
        var prognosa = $('#aktual_kirim_paket_ke_co').val();
        $('#prognosa_kirim_paket_ke_co').val(prognosa);
        status_curr = 'KIRIM PAKET KE CO';
        $('#current').val(status_curr);
    }
    let date_prognosa = count_prognosa(prognosa, parseInt(durasi_));
    $('#prognosa_pengumuman_pendaftaran').val(formatDate(date_prognosa));
}

function prognosa_pengumuman_pendaftaran() {
    var durasi_ = $('#sertifikasi').val();
    if ($('#aktual_pengumuman_pendaftaran').val() == '') {
        var prognosa = $('#prognosa_pengumuman_pendaftaran').val();
    } else {
        var prognosa = $('#aktual_pengumuman_pendaftaran').val();
        $('#prognosa_pengumuman_pendaftaran').val(prognosa);
        status_curr = 'PENGUMUMAN PENDAFTARAN';
        $('#current').val(status_curr);
    }
    let date_prognosa = count_prognosa(prognosa, parseInt(durasi_));
    $('#prognosa_sertifikasi').val(formatDate(date_prognosa));
}

function prognosa_sertifikasi() {
    var durasi_ = $('#prakualifikasi').val();
    if ($('#aktual_sertifikasi').val() == '') {
        var prognosa = $('#prognosa_sertifikasi').val();
    } else {
        var prognosa = $('#aktual_sertifikasi').val();
        $('#prognosa_sertifikasi').val(prognosa);
        status_curr = 'SERTIFIKASI';
        $('#current').val(status_curr);
    }
    let date_prognosa = count_prognosa(prognosa, parseInt(durasi_));
    $('#prognosa_prakualifikasi').val(formatDate(date_prognosa));
}

function prognosa_prakualifikasi() {
    var durasi_ = $('#undangan').val();
    if ($('#aktual_prakualifikasi').val() == '') {
        var prognosa = $('#prognosa_prakualifikasi').val();
    } else {
        var prognosa = $('#aktual_prakualifikasi').val();
        $('#prognosa_prakualifikasi').val(prognosa);
        status_curr = 'PRAKUALIFIKASI';
        $('#current').val(status_curr);
    }
    let date_prognosa = count_prognosa(prognosa, parseInt(durasi_));
    $('#prognosa_undangan').val(formatDate(date_prognosa));
}

function prognosa_undangan() {
    var durasi_ = $('#pemberian').val();
    if ($('#aktual_undangan').val() == '') {
        var prognosa = $('#prognosa_undangan').val();
    } else {
        var prognosa = $('#aktual_undangan').val();
        $('#prognosa_undangan').val(prognosa);
        status_curr = 'UNDANGAN & PENGAMBILAN DOKUMEN';
        $('#current').val(status_curr);
    }
    let date_prognosa = count_prognosa(prognosa, parseInt(durasi_));
    $('#prognosa_pemberian_penjelasan').val(formatDate(date_prognosa));
}

function prognosa_pemberian_penjelasan() {
    var durasi_ = $('#penyampaian').val();
    if ($('#aktual_pemberian_penjelasan').val() == '') {
        var prognosa = $('#prognosa_pemberian_penjelasan').val();
    } else {
        var prognosa = $('#aktual_pemberian_penjelasan').val();
        $('#prognosa_pemberian_penjelasan').val(prognosa);
        status_curr = 'PEMBERIAN PENJELASAN';
        $('#current').val(status_curr);
    }
    let date_prognosa = count_prognosa(prognosa, parseInt(durasi_));
    $('#prognosa_penyampaian_dokumen').val(formatDate(date_prognosa));
}

function prognosa_penyampaian_dokumen() {
    var durasi_ = $('#pembukaan').val();
    if ($('#aktual_penyampaian_dokumen').val() == '') {
        var prognosa = $('#prognosa_penyampaian_dokumen').val();
    } else {
        var prognosa = $('#aktual_penyampaian_dokumen').val();
        $('#prognosa_penyampaian_dokumen').val(prognosa);
        status_curr = 'PENYAMPAIAN DOKUMEN PENAWARAN';
        $('#current').val(status_curr);
    }
    let date_prognosa = count_prognosa(prognosa, parseInt(durasi_));
    $('#prognosa_pembukaan_penawaran').val(formatDate(date_prognosa));
}

function prognosa_pembukaan_penawaran() {
    var durasi_ = $('#evaluasi').val();
    if ($('#aktual_pembukaan_penawaran').val() == '') {
        var prognosa = $('#prognosa_pembukaan_penawaran').val();
    } else {
        var prognosa = $('#aktual_pembukaan_penawaran').val();
        $('#prognosa_pembukaan_penawaran').val(prognosa);
        status_curr = 'PEMBUKAAN DOKUMEN PENAWARAN';
        $('#current').val(status_curr);
    }
    let date_prognosa = count_prognosa(prognosa, parseInt(durasi_));
    $('#prognosa_penetapan_evaluasi_penawaran').val(formatDate(date_prognosa));
}

function prognosa_penetapan_evaluasi_penawaran() {
    var durasi_ = $('#negosiasi').val();
    if ($('#aktual_penetapan_evaluasi_penawaran').val() == '') {
        var prognosa = $('#prognosa_penetapan_evaluasi_penawaran').val();
    } else {
        var prognosa = $('#aktual_penetapan_evaluasi_penawaran').val();
        $('#prognosa_penetapan_evaluasi_penawaran').val(prognosa);
        status_curr = 'EVALUASI PENAWARAN';
        $('#current').val(status_curr);
    }
    let date_prognosa = count_prognosa(prognosa, parseInt(durasi_));
    $('#prognosa_negosiasi').val(formatDate(date_prognosa));
}

function prognosa_negosiasi() {
    var durasi_ = $('#usulan').val();
    if ($('#aktual_negosiasi').val() == '') {
        var prognosa = $('#prognosa_negosiasi').val();
    } else {
        var prognosa = $('#aktual_negosiasi').val();
        $('#prognosa_negosiasi').val(prognosa);
        status_curr = 'NEGOSIASI';
        $('#current').val(status_curr);
    }
    let date_prognosa = count_prognosa(prognosa, parseInt(durasi_));
    $('#prognosa_usulan_penetapan_calon_pemenang').val(formatDate(date_prognosa));
}

function prognosa_usulan_penetapan_calon_pemenang() {
    var durasi_ = $('#keputusan').val();
    if ($('#aktual_usulan_penetapan_calon_pemenang').val() == '') {
        var prognosa = $('#prognosa_usulan_penetapan_calon_pemenang').val();
    } else {
        var prognosa = $('#aktual_usulan_penetapan_calon_pemenang').val();
        $('#prognosa_usulan_penetapan_calon_pemenang').val(prognosa);
        status_curr = 'USULAN PENETAPAN CALON PEMENANG';
        $('#current').val(status_curr);
    }
    let date_prognosa = count_prognosa(prognosa, parseInt(durasi_));
    $('#prognosa_keputusan_pemenang').val(formatDate(date_prognosa));
}

function prognosa_keputusan_pemenang() {
    var durasi_ = $('#pengumuman_pemenang').val();
    if ($('#aktual_keputusan_pemenang').val() == '') {
        var prognosa = $('#prognosa_keputusan_pemenang').val();
    } else {
        var prognosa = $('#aktual_keputusan_pemenang').val();
        $('#prognosa_keputusan_pemenang').val(prognosa);
        status_curr = 'KEPUTUSAN PEMENANG';
        $('#current').val(status_curr);
    }
    let date_prognosa = count_prognosa(prognosa, parseInt(durasi_));
    $('#prognosa_pengumuman_pemenang').val(formatDate(date_prognosa));
}

function prognosa_pengumuman_pemenang() {
    var durasi_ = $('#pengajuan_sanggah').val();
    if ($('#aktual_pengumuman_pemenang').val() == '') {
        var prognosa = $('#prognosa_pengumuman_pemenang').val();
    } else {
        var prognosa = $('#aktual_pengumuman_pemenang').val();
        $('#prognosa_pengumuman_pemenang').val(prognosa);
        status_curr = 'PENGUMUMAN PEMENANG';
        $('#current').val(status_curr);
    }
    let date_prognosa = count_prognosa(prognosa, parseInt(durasi_));
    $('#prognosa_pengajuan_sanggah').val(formatDate(date_prognosa));
}

function prognosa_pengajuan_sanggah() {
    var durasi_ = $('#jawaban_sanggah').val();
    if ($('#aktual_pengajuan_sanggah').val() == '') {
        var prognosa = $('#prognosa_pengajuan_sanggah').val();
    } else {
        var prognosa = $('#aktual_pengajuan_sanggah').val();
        $('#prognosa_pengajuan_sanggah').val(prognosa);
        status_curr = 'PENGAJUAN SANGGAH';
        $('#current').val(status_curr);
    }
    let date_prognosa = count_prognosa(prognosa, parseInt(durasi_));
    $('#prognosa_jawaban_sanggah').val(formatDate(date_prognosa));
}

function prognosa_jawaban_sanggah() {
    var durasi_ = $('#tunjuk_pemenang').val();
    if ($('#aktual_jawaban_sanggah').val() == '') {
        var prognosa = $('#prognosa_jawaban_sanggah').val();
    } else {
        var prognosa = $('#aktual_jawaban_sanggah').val();
        $('#prognosa_jawaban_sanggah').val(prognosa);
        status_curr = 'JAWABAN SANGGAH';
        $('#current').val(status_curr);
    }
    let date_prognosa = count_prognosa(prognosa, parseInt(durasi_));
    $('#prognosa_penunjukan_pemenang').val(formatDate(date_prognosa));
}

function prognosa_penunjukan_pemenang() {
    var durasi_ = $('#proses_spb').val();
    if ($('#aktual_penunjukan_pemenang').val() == '') {
        var prognosa = $('#prognosa_penunjukan_pemenang').val();
    } else {
        var prognosa = $('#aktual_penunjukan_pemenang').val();
        $('#prognosa_penunjukan_pemenang').val(prognosa);
        status_curr = 'PENUNJUKAN PEMENANG';
        $('#current').val(status_curr);
    }
    let date_prognosa = count_prognosa(prognosa, parseInt(durasi_));
    $('#prognosa_proses_spb').val(formatDate(date_prognosa));
}


function prognosa_spb() {
    if ($('#aktual_proses_spb').val() == '') {
        var prognosa = $('#prognosa_penunjuk_pemenang').val();
    } else {
        var prognosa = $('#aktual_proses_spb').val();
        $('#prognosa_proses_spb').val(prognosa);
        status_curr = 'PROSES SPB';
        $('#current').val(status_curr);
    }
}

function get_sow(project) {
    $.ajax({
        url: '../get_sow',
        type: 'POST',
        data: {
            project: project
        },
        success: function (res) {
            $('#judulPekerjaan').html(res);
        }
    })
}

function formatDate(date) {
    var d = new Date(date),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2)
        month = '0' + month;
    if (day.length < 2)
        day = '0' + day;

    return [year, month, day].join('-');
}

function get_target(id_project, target_sp, kattender) {
    $.ajax({
        url: "../get_data_target",
        method: "POST",
        data: {
            id_project: id_project,
            target_sp: target_sp,
            kattender: kattender
        },
        success: function (res) {
            if (res.result == true) {
                $('#target_susun_kak').val(formatDate(res.target_kak));
                $('#target_susun_oe').val(formatDate(res.target_oe));
                $('#target_persetujuan').val(formatDate(res.target_persetujuan));
                $('#target_kirim_paket_ke_co').val(formatDate(res.target_co));
                $('#target_pengumuman_pendaftaran').val(formatDate(res.target_pengumuman));
                $('#target_sertifikasi').val(formatDate(res.target_sertifikasi));
                $('#target_prakualifikasi').val(formatDate(res.target_prakualifikasi));
                $('#target_undangan').val(formatDate(res.target_undangan));
                $('#target_pemberian_penjelasan').val(formatDate(res.target_pemberian));
                $('#target_penyampaian_dokumen').val(formatDate(res.target_penyampaian));
                $('#target_pembukaan_penawaran').val(formatDate(res.target_pembukaan));
                $('#target_penetapan_evaluasi_penawaran').val(formatDate(res.target_evaluasi));
                $('#target_negosiasi').val(formatDate(res.target_negosiasi));
                $('#target_usulan_penetapan_calon_pemenang').val(formatDate(res.target_usulan_pemenang));
                $('#target_keputusan_pemenang').val(formatDate(res.target_keputusan_pemenang));
                $('#target_pengumuman_pemenang').val(formatDate(res.target_pengumuman_pemenang));
                $('#target_pengajuan_sanggah').val(formatDate(res.target_pengajuan_sanggah));
                $('#target_jawaban_sanggah').val(formatDate(res.target_jawaban_sanggah));
                $('#target_penunjukan_pemenang').val(formatDate(res.target_penunjukan_pemenang));
                $('#target_proses_spb').val(formatDate(res.target_spb));
            }
        }
    })
}

function count_date() {
    var persiapan = ($('#preparing').val() == '') ? 0 : $('#preparing').val();
    var fabrikasi = ($('#fabrikasi').val() == '') ? 0 : $('#fabrikasi').val();
    var mdays = ($('#mechanical').val() == '') ? 0 : $('#mechanical').val();
    var finishing = ($('#finishing').val() == '') ? 0 : $('#finishing').val();
    var maint = ($('#maintenance').val() == '') ? 0 : $('#maintenance').val();

    var tot = parseInt(persiapan) + parseInt(fabrikasi);
    var jum = parseInt(mdays) + parseInt(finishing) + parseInt(maint);
    var deadLine = $('#dead_line').val();

    $('#total_days').val(tot + jum);

    if (tot != 0) {
        var terbit_sp = new Date(deadLine);
        terbit_sp.setDate(terbit_sp.getDate() - tot);
        $('#terbit_sp').val(formatDate(terbit_sp));
        $('#start').val(formatDate(terbit_sp));
        get_target($('#projectNo').val(), $('#terbit_sp').val(), $('#katTender').val());
    } else {
        var date3 = new Date(deadLine);
        date3.setDate(date3.getDate());
        $('#terbit_sp').val(formatDate(date3));
        $('#start').val(formatDate(date3));
        get_target($('#projectNo').val(), $('#terbit_sp').val(), $('#katTender').val());
    }

    if (jum != 0) {
        var end_date = new Date(deadLine);
        end_date.setDate(end_date.getDate() + jum);
        $('#end').val(formatDate(end_date));
    } else {
        var end_date = new Date(deadLine);
        end_date.setDate(end_date.getDate());
        $('#end').val(formatDate(end_date));
    }
}

$(document).ready(function () {
    $(".select2").select2({});
    $.ajaxSetup({
        headers: {
            'X-CSRF-TOKEN': $('meta[name="csrf-token"]').attr('content')
        }
    });

    $(document).on('click', '.tipe_koordinasi', function () {
        if ($(this).val() != 0) {
            document.getElementById('show_koordinasi').style.display = 'block';
        } else {
            document.getElementById('show_koordinasi').style.display = 'none';
        }
    })

    $(document).on('change', '#projectNo', function () {
        var id = $(this).val();
        getKatTender(id);
        get_sow(id);
    })

    $(document).on('change', '#katTender', function () {
        get_target($('#projectNo').val(), $('#terbit_sp').val(), $('#katTender').val());
    })

    getKatTender($('#projectNo').val());
    //prognosa_
    prognosa_kak();
    prognosa_perizinan();
    prognosa_oe();
    prognosa_kirim_co();
    prognosa_pengumuman_pendaftaran();
    prognosa_sertifikasi();
    prognosa_prakualifikasi();
    prognosa_undangan();
    prognosa_pemberian_penjelasan();
    prognosa_penyampaian_dokumen();
    prognosa_pembukaan_penawaran();
    prognosa_penetapan_evaluasi_penawaran();
    prognosa_negosiasi();
    prognosa_usulan_penetapan_calon_pemenang();
    prognosa_keputusan_pemenang();
    prognosa_pengumuman_pemenang();
    prognosa_pengajuan_sanggah();
    prognosa_jawaban_sanggah();
    prognosa_penunjukan_pemenang();
    prognosa_spb();

    $(document).on('change', '#aktual_susun_kak', function () {
        prognosa_kak();
        prognosa_perizinan();
        prognosa_oe();
        prognosa_kirim_co();
        prognosa_pengumuman_pendaftaran();
        prognosa_sertifikasi();
        prognosa_prakualifikasi();
        prognosa_undangan();
        prognosa_pemberian_penjelasan();
        prognosa_penyampaian_dokumen();
        prognosa_pembukaan_penawaran();
        prognosa_penetapan_evaluasi_penawaran();
        prognosa_negosiasi();
        prognosa_usulan_penetapan_calon_pemenang();
        prognosa_keputusan_pemenang();
        prognosa_pengumuman_pemenang();
        prognosa_pengajuan_sanggah();
        prognosa_jawaban_sanggah();
        prognosa_penunjukan_pemenang();
        prognosa_spb();
    })

    $(document).on('change', '#akt_perizinan', function () {
        prognosa_perizinan();
        prognosa_oe();
        prognosa_kirim_co();
        prognosa_pengumuman_pendaftaran();
        prognosa_sertifikasi();
        prognosa_prakualifikasi();
        prognosa_undangan();
        prognosa_pemberian_penjelasan();
        prognosa_penyampaian_dokumen();
        prognosa_pembukaan_penawaran();
        prognosa_penetapan_evaluasi_penawaran();
        prognosa_negosiasi();
        prognosa_usulan_penetapan_calon_pemenang();
        prognosa_keputusan_pemenang();
        prognosa_pengumuman_pemenang();
        prognosa_pengajuan_sanggah();
        prognosa_jawaban_sanggah();
        prognosa_penunjukan_pemenang();
        prognosa_spb();
    })

    $(document).on('change', '#aktual_susun_oe', function () {
        prognosa_oe();
        prognosa_kirim_co();
        prognosa_pengumuman_pendaftaran();
        prognosa_sertifikasi();
        prognosa_prakualifikasi();
        prognosa_undangan();
        prognosa_pemberian_penjelasan();
        prognosa_penyampaian_dokumen();
        prognosa_pembukaan_penawaran();
        prognosa_penetapan_evaluasi_penawaran();
        prognosa_negosiasi();
        prognosa_usulan_penetapan_calon_pemenang();
        prognosa_keputusan_pemenang();
        prognosa_pengumuman_pemenang();
        prognosa_pengajuan_sanggah();
        prognosa_jawaban_sanggah();
        prognosa_penunjukan_pemenang();
        prognosa_spb();
    })

    $(document).on('change', '#aktual_kirim_paket_ke_co', function () {
        prognosa_kirim_co();
        prognosa_pengumuman_pendaftaran();
        prognosa_sertifikasi();
        prognosa_prakualifikasi();
        prognosa_undangan();
        prognosa_pemberian_penjelasan();
        prognosa_penyampaian_dokumen();
        prognosa_pembukaan_penawaran();
        prognosa_penetapan_evaluasi_penawaran();
        prognosa_negosiasi();
        prognosa_usulan_penetapan_calon_pemenang();
        prognosa_keputusan_pemenang();
        prognosa_pengumuman_pemenang();
        prognosa_pengajuan_sanggah();
        prognosa_jawaban_sanggah();
        prognosa_penunjukan_pemenang();
        prognosa_spb();
    })

    $(document).on('change', '#aktual_pengumuman_pendaftaran', function () {
        prognosa_pengumuman_pendaftaran();
        prognosa_sertifikasi();
        prognosa_prakualifikasi();
        prognosa_undangan();
        prognosa_pemberian_penjelasan();
        prognosa_penyampaian_dokumen();
        prognosa_pembukaan_penawaran();
        prognosa_penetapan_evaluasi_penawaran();
        prognosa_negosiasi();
        prognosa_usulan_penetapan_calon_pemenang();
        prognosa_keputusan_pemenang();
        prognosa_pengumuman_pemenang();
        prognosa_pengajuan_sanggah();
        prognosa_jawaban_sanggah();
        prognosa_penunjukan_pemenang();
        prognosa_spb();
    })

    $(document).on('change', '#aktual_sertifikasi', function () {
        prognosa_sertifikasi();
        prognosa_prakualifikasi();
        prognosa_undangan();
        prognosa_pemberian_penjelasan();
        prognosa_penyampaian_dokumen();
        prognosa_pembukaan_penawaran();
        prognosa_penetapan_evaluasi_penawaran();
        prognosa_negosiasi();
        prognosa_usulan_penetapan_calon_pemenang();
        prognosa_keputusan_pemenang();
        prognosa_pengumuman_pemenang();
        prognosa_pengajuan_sanggah();
        prognosa_jawaban_sanggah();
        prognosa_penunjukan_pemenang();
        prognosa_spb();
    })

    $(document).on('change', '#aktual_prakualifikasi', function () {
        prognosa_prakualifikasi();
        prognosa_undangan();
        prognosa_pemberian_penjelasan();
        prognosa_penyampaian_dokumen();
        prognosa_pembukaan_penawaran();
        prognosa_penetapan_evaluasi_penawaran();
        prognosa_negosiasi();
        prognosa_usulan_penetapan_calon_pemenang();
        prognosa_keputusan_pemenang();
        prognosa_pengumuman_pemenang();
        prognosa_pengajuan_sanggah();
        prognosa_jawaban_sanggah();
        prognosa_penunjukan_pemenang();
        prognosa_spb();
    })

    $(document).on('change', '#aktual_undangan', function () {
        prognosa_undangan();
        prognosa_pemberian_penjelasan();
        prognosa_penyampaian_dokumen();
        prognosa_pembukaan_penawaran();
        prognosa_penetapan_evaluasi_penawaran();
        prognosa_negosiasi();
        prognosa_usulan_penetapan_calon_pemenang();
        prognosa_keputusan_pemenang();
        prognosa_pengumuman_pemenang();
        prognosa_pengajuan_sanggah();
        prognosa_jawaban_sanggah();
        prognosa_penunjukan_pemenang();
        prognosa_spb();
    })

    $(document).on('change', '#aktual_pemberian_penjelasan', function () {
        prognosa_pemberian_penjelasan();
        prognosa_penyampaian_dokumen();
        prognosa_pembukaan_penawaran();
        prognosa_penetapan_evaluasi_penawaran();
        prognosa_negosiasi();
        prognosa_usulan_penetapan_calon_pemenang();
        prognosa_keputusan_pemenang();
        prognosa_pengumuman_pemenang();
        prognosa_pengajuan_sanggah();
        prognosa_jawaban_sanggah();
        prognosa_penunjukan_pemenang();
        prognosa_spb();
    })

    $(document).on('change', '#aktual_penyampaian_dokumen', function () {
        prognosa_penyampaian_dokumen();
        prognosa_pembukaan_penawaran();
        prognosa_penetapan_evaluasi_penawaran();
        prognosa_negosiasi();
        prognosa_usulan_penetapan_calon_pemenang();
        prognosa_keputusan_pemenang();
        prognosa_pengumuman_pemenang();
        prognosa_pengajuan_sanggah();
        prognosa_jawaban_sanggah();
        prognosa_penunjukan_pemenang();
        prognosa_spb();
    })

    $(document).on('change', '#aktual_pembukaan_dokumen', function () {
        prognosa_pembukaan_penawaran();
        prognosa_penetapan_evaluasi_penawaran();
        prognosa_negosiasi();
        prognosa_usulan_penetapan_calon_pemenang();
        prognosa_keputusan_pemenang();
        prognosa_pengumuman_pemenang();
        prognosa_pengajuan_sanggah();
        prognosa_jawaban_sanggah();
        prognosa_penunjukan_pemenang();
        prognosa_spb();
    })

    $(document).on('change', '#aktual_penetapan_evaluasi_penawaran', function () {
        prognosa_penetapan_evaluasi_penawaran();
        prognosa_negosiasi();
        prognosa_usulan_penetapan_calon_pemenang();
        prognosa_keputusan_pemenang();
        prognosa_pengumuman_pemenang();
        prognosa_pengajuan_sanggah();
        prognosa_jawaban_sanggah();
        prognosa_penunjukan_pemenang();
        prognosa_spb();
    })

    $(document).on('change', '#aktual_negosiasi', function () {
        prognosa_negosiasi();
        prognosa_usulan_penetapan_calon_pemenang();
        prognosa_keputusan_pemenang();
        prognosa_pengumuman_pemenang();
        prognosa_pengajuan_sanggah();
        prognosa_jawaban_sanggah();
        prognosa_penunjukan_pemenang();
        prognosa_spb();
    })

    $(document).on('change', '#aktual_usulan_penetapan_calon_pemenang', function () {
        prognosa_usulan_penetapan_calon_pemenang();
        prognosa_keputusan_pemenang();
        prognosa_pengumuman_pemenang();
        prognosa_pengajuan_sanggah();
        prognosa_jawaban_sanggah();
        prognosa_penunjukan_pemenang();
        prognosa_spb();
    })

    $(document).on('change', '#aktual_keputusan_pemenang', function () {
        prognosa_keputusan_pemenang();
        prognosa_pengumuman_pemenang();
        prognosa_pengajuan_sanggah();
        prognosa_jawaban_sanggah();
        prognosa_penunjukan_pemenang();
        prognosa_spb();
    })

    $(document).on('change', '#aktual_pengumuman_pemenang', function () {
        prognosa_pengumuman_pemenang();
        prognosa_pengajuan_sanggah();
        prognosa_jawaban_sanggah();
        prognosa_penunjukan_pemenang();
        prognosa_spb();
    })

    $(document).on('change', '#aktual_pengajuan_sanggah', function () {
        prognosa_pengajuan_sanggah();
        prognosa_jawaban_sanggah();
        prognosa_penunjukan_pemenang();
        prognosa_spb();
    })

    $(document).on('change', '#aktual_jawaban_sanggah', function () {
        prognosa_jawaban_sanggah();
        prognosa_penunjukan_pemenang();
        prognosa_spb();
    })

    $(document).on('change', '#aktual_penunjukan_pemenang', function () {
        prognosa_penunjukan_pemenang();
        prognosa_spb();
    })

    $(document).on('change', '#aktual_proses_spb', function () {
        prognosa_spb();
    })

    var table = $('#table').DataTable({
        dom: '<"dataTables_wrapper dt-bootstrap"<"row"<"col-xl-7 d-block d-sm-flex d-xl-block justify-content-center"<"d-block d-lg-inline-flex me-0 me-md-3"l><"d-block d-lg-inline-flex"B>><"col-xl-5 d-flex d-xl-block justify-content-center"fr>>t<"row"<"col-md-5"i><"col-md-7"p>>>',
        processing: true,
        serverSide: true,
        ajax: {
            url: '/related_',
            method: 'POST',
            data: {
                id_contract: $("#hidden_id").val()
            }
        },
        columns: [
            { data: 'eqTagNo', name: 'eqTagNo' },
            { data: 'jobNo', name: 'jobNo' },
            { data: 'jobDesc', name: 'jobDesc' },
            {
                data: 'action', name: 'action', orderable: false, searching: false,
                "render": function (data, type, full, meta) {
                    return '<a href="javascript:void(0);" class="btn btn-xs waves-effect waves-light btn-outline-danger delete_wo_jasa " data-id="'+full.id+'" ><i class="fas fa-trash fa-xs"></i></a>';
                },
            },
            
        ],
        buttons: [
            {
                extend: 'excel',
                title: 'Job List',
                className: 'btn',
                text: '<i class="far fa-file-code"></i> Excel',
                titleAttr: 'Excel',
                exportOptions: {
                    columns: ':not(:last-child)',
                }
            },
        ]

    });

    $(document).on('click', '.delete_wo_jasa', function () {
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
                    url: "../delete_wo_jasa",
                    type: "POST",
                    data: {
                        id: id
                    },
                    dataType: "JSON",
                    success: function (data) {
                        table.ajax.reload();
                        Swal.fire({
                            title: data.title,
                            html: data.status,
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
        rules: {

        },

        submitHandler: function (form) {
            let url;

            var form_data = new FormData(document.getElementById("add-form"));
            // form_data.append('hidden_id', $('#hidden_id').val());
            // form_data.append('form_', 'info');
            $.ajax({
                url: '../update_contracttracking',
                type: "POST",
                data: form_data,
                dataType: "JSON",
                contentType: false,
                cache: false,
                processData: false,
                success: function (data) {
                    if (data.result != true) {
                        Swal.fire({
                            title: 'Gagal',
                            text: "Gagal Tambah / Update Contract",
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

                        // window.location.href = 'contracttracking';

                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert('Error adding / update data');
                }
            });
        }
    });

    // $("#form_1").validate({
    //     errorClass: "is-invalid",
    //     highlight: function (element, errorClass, validClass) {
    //         $(element).parents('.form-control').removeClass('is-valid').addClass('is-invalid');     
    //     },
    //     unhighlight: function (element, errorClass, validClass) {
    //         $(element).parents('.form-control').removeClass('is-invalid').addClass('is-valid');
    //     },
    //     errorPlacement: function (error, element) {
    //         if(element.hasClass('select2') && element.next('.select2-container').length) {
    //             error.insertAfter(element.next('.select2-container'));
    //         } else if (element.parent('.input-group').length) {
    //             error.insertAfter(element.parent());
    //         }
    //         else if (element.prop('type') === 'radio' && element.parent('.radio-inline').length) {
    //             error.insertAfter(element.parent().parent());
    //         }
    //         else if (element.prop('type') === 'checkbox' || element.prop('type') === 'radio') {
    //             error.appendTo(element.parent().parent());
    //         }
    //         else {
    //             error.insertAfter(element);
    //         }
    //     },
    //     rules: {

    //     },

    //     submitHandler: function(form) {
    //         let url;

    //         var form_data = new FormData(document.getElementById("form_1"));
    //         form_data.append('hidden_id', $('#hidden_id').val());
    //         form_data.append('form_', 'form_1');
    //         $.ajax({
    //             url: '../update_contracttracking',
    //             type: "POST",
    //             data: form_data,
    //             dataType: "JSON",
    //             contentType: false,
    //             cache: false,
    //             processData: false,
    //             beforeSend: function(){
    //                     swal.fire({
    //                         title: 'Harap Tunggu!',
    //                         allowEscapeKey: false,
    //                         allowOutsideClick: false,
    //                         showCancelButton: false,
    //                         showConfirmButton: false,
    //                         buttons: false,
    //                         timer: 2000,
    //                         didOpen: () => {
    //                             Swal.showLoading()
    //                         }
    //                     })
    //                 },
    //             success: function(data) {
    //                 if (data.result != true) {
    //                     Swal.fire({
    //                         title: 'Gagal',
    //                         text: "Gagal Tambah / Update Contract",
    //                         icon: 'error',
    //                         timer: 3000,
    //                         showCancelButton: false,
    //                         showConfirmButton: false,
    //                         buttons: false,
    //                     });
    //                 }
    //                  else {
    //                     Swal.fire({
    //                         title: 'Berhasil',
    //                         icon: 'success',
    //                         timer: 3000,
    //                         showCancelButton: false,
    //                         showConfirmButton: false
    //                     });

    //                     // window.location.href = 'contracttracking';

    //                 }
    //             },
    //             error: function(jqXHR, textStatus, errorThrown) {
    //                 alert('Error adding / update data');
    //             }
    //         });
    //     }
    // });

    // $("#form_2").validate({
    //     errorClass: "is-invalid",
    //     highlight: function (element, errorClass, validClass) {
    //         $(element).parents('.form-control').removeClass('is-valid').addClass('is-invalid');     
    //     },
    //     unhighlight: function (element, errorClass, validClass) {
    //         $(element).parents('.form-control').removeClass('is-invalid').addClass('is-valid');
    //     },
    //     errorPlacement: function (error, element) {
    //         if(element.hasClass('select2') && element.next('.select2-container').length) {
    //             error.insertAfter(element.next('.select2-container'));
    //         } else if (element.parent('.input-group').length) {
    //             error.insertAfter(element.parent());
    //         }
    //         else if (element.prop('type') === 'radio' && element.parent('.radio-inline').length) {
    //             error.insertAfter(element.parent().parent());
    //         }
    //         else if (element.prop('type') === 'checkbox' || element.prop('type') === 'radio') {
    //             error.appendTo(element.parent().parent());
    //         }
    //         else {
    //             error.insertAfter(element);
    //         }
    //     },
    //     rules: {

    //     },

    //     submitHandler: function(form) {
    //         let url;

    //         var form_data = new FormData(document.getElementById("form_2"));
    //         form_data.append('hidden_id', $('#hidden_id').val());
    //         form_data.append('prognosa_spb', $('#prognosa_proses_spb').val());
    //         form_data.append('deadline', $('#dead_line').val());
    //         form_data.append('form_', 'form_2');
    //         $.ajax({
    //             url: '../update_contracttracking',
    //             type: "POST",
    //             data: form_data,
    //             dataType: "JSON",
    //             contentType: false,
    //             cache: false,
    //             processData: false,
    //             success: function(data) {
    //                 if (data.result != true) {
    //                     Swal.fire({
    //                         title: 'Gagal',
    //                         text: "Gagal Tambah / Update Contract",
    //                         icon: 'error',
    //                         timer: 3000,
    //                         showCancelButton: false,
    //                         showConfirmButton: false,
    //                         buttons: false,
    //                     });
    //                 } else {
    //                     Swal.fire({
    //                         title: 'Berhasil',
    //                         icon: 'success',
    //                         timer: 3000,
    //                         showCancelButton: false,
    //                         showConfirmButton: false
    //                     });

    //                     // window.location.href = 'contracttracking';

    //                 }
    //             },
    //             error: function(jqXHR, textStatus, errorThrown) {
    //                 alert('Error adding / update data');
    //             }
    //         });
    //     }
    // });
});