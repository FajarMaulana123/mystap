﻿using System.ComponentModel.DataAnnotations;
using mystap.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace mystap.Models
{
    public class ContractTracking
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long idPaket { get; set; }
        public string? noPaket { get; set; }
        public string? judulPekerjaan { get; set; }
        public string? kategoriPaket { get; set; }
        public string? tipePaket { get; set; }
        public int? tipe_koordinasi { get; set; }
        public string? koordinasi { get; set; }
        public string? unit { get; set; }
        public string? pic { get; set; }
        public DateTime? deadLine { get; set; }
        public string? descSP { get; set; }
        public int? kak { get; set; }
        public int? estimasi { get; set; }
        public int? aanwijz { get; set; }
        public int? tender { get; set; }
        public int? pemenang { get; set; }
        public int? spb { get; set; }
        public int? t_light { get; set; }
        public string? prevStat { get; set; }
        public string? currStat { get; set; }
        public string? currStatDesc { get; set; }
        public string? kriteria { get; set; }
        public string? dirPWS { get; set; }
        public string? sico { get; set; }
        public long? oe { get; set; }
        public long? nilaiKontrak { get; set; }
        public string? noSP { get; set; }
        public DateTime? tglSP { get; set; }
        public string? file_sp { get; set; }
        public int? persiapan { get; set; }
        public int? fabrikasi { get; set; }
        public int? mech { get; set; }
        public int? finishing { get; set; }
        public int? pemeliharaan { get; set; }
        public DateTime? mulai { get; set; }
        public DateTime? selesai { get; set; }
        public int? waktuAdendum { get; set; }
        public int? angkaWaktuAdendum { get; set; }
        public string? tglAkhirAddendum { get; set; }
        public int? nilaiAddendum { get; set; }
        public int? angkaAddendum { get; set; }
        public int? angkaAddendum1 { get; set; }
        public int? angkaAddendum2 { get; set; }
        public string? kontenSPAddendum { get; set; }
        public long? idVendor { get; set; }
        public string? contactPerson { get; set; }
        public string? polaShift { get; set; }
        public int? jamKerjaPerHari { get; set; }
        public int? perShiftMP { get; set; }
        public string? poiKOM { get; set; }
        public string? poiSOS { get; set; }
        public string? katTender { get; set; }
        public string? denda { get; set; }
        public long? projectID { get; set; }
        public long? createdBy { get; set; }
        public DateTime? dateCreated { get; set; }
        public int? deleted { get; set; }
        public int? updated { get; set; }
        public DateTime? targetCO { get; set; }
        public DateTime? aktualCO { get; set; }
        public DateTime? targetSP { get; set; }
        public DateTime? aktualSP { get; set; }
        public DateTime? aktualBukaPH { get; set; }
        public string? lastStep { get; set; }
        public string? WO { get; set; }
        public int? PR { get; set; }
        public int? note { get; set; }
        public long? updatedBy { get; set; }
        public DateTime? lastUpdate { get; set; }
        public int? statusTagihan { get; set; }
        public string? maInpro { get; set; }
        public string? maTMT { get; set; }
        public string? budgetInpro { get; set; }
        public string? budgetTMT { get; set; }
        public string? keuInpro { get; set; }
        public string? keuTMT { get; set; }
        public string? ketLainVendor { get; set; }
        public string? konfirmasiVendor { get; set; }
        public string? actionPlan { get; set; }
        public string? sourceVendor { get; set; }
        public string? asalSource { get; set; }
        public string? kota { get; set; }
        public string? katPaket { get; set; }
        public string? skillGroup { get; set; }
        public string? csms { get; set; }
        public int? perluAanwijzing { get; set; }
        public int? statusJasa { get; set; }
        public int? KontrakManPower { get; set; }
        public int? needWelder { get; set; }
        public string? vendorTender { get; set; }
        public string? katCriticalWork { get; set; }
        public string? permitWO { get; set; }
        public string? tglRelWO { get; set; }
        public string? nilaiHasilKerja { get; set; }
        public string? rencanaAktualPIC { get; set; }
        public int? retender { get; set; }
        public string? retenderReason { get; set; }
        public string? keterangan { get; set; }
        public string? po { get; set; }
        public string? disiplin { get; set; }
        public int? pengajuanBadge { get; set; }
        public int? jmlMP1 { get; set; }
        public int? jmlMP2 { get; set; }
        public int? jmlMP3 { get; set; }
        public int? jmlMP4 { get; set; }
        public int? jmlMP5 { get; set; }
        public string? notPengajuanBadge { get; set; }
        public string? klasifikasiKontrak { get; set; }
        public string? organik { get; set; }
        public DateTime? targetBukaPH { get; set; }
        public DateTime? target_kak { get; set; }
        public DateTime? akt_kak { get; set; }
        public DateTime? target_oe { get; set; }
        public DateTime? akt_oe { get; set; }
        public DateTime? target_co { get; set; }
        public DateTime? akt_co { get; set; }
        public DateTime? target_pengumuman { get; set; }
        public DateTime? akt_pengumuman { get; set; }
        public DateTime? target_sertifikasi { get; set; }
        public DateTime? akt_sertifikasi { get; set; }
        public DateTime? target_prakualifikasi { get; set; }
        public DateTime? akt_prakualifikasi { get; set; }
        public DateTime? target_undangan { get; set; }
        public DateTime? akt_undangan { get; set; }
        public DateTime? target_pemberian { get; set; }
        public DateTime? akt_pemberian { get; set; }
        public DateTime? target_penyampaian { get; set; }
        public DateTime? akt_penyampaian { get; set; }
        public DateTime? target_pembukaan { get; set; }
        public DateTime? akt_pembukaan { get; set; }
        public DateTime? target_evaluasi { get; set; }
        public DateTime? akt_evaluasi { get; set; }
        public DateTime? target_negosiasi { get; set; }
        public DateTime? akt_negosiasi { get; set; }
        public DateTime? target_usulan_pemenang { get; set; }
        public DateTime? akt_usulan_pemenang { get; set; }
        public DateTime? target_keputusan_pemenang { get; set; }
        public DateTime? akt_keputusan_pemenang { get; set; }
        public DateTime? target_pengumuman_pemenang { get; set; }
        public DateTime? akt_pengumuman_pemenang { get; set; }
        public DateTime? target_pengajuan_sanggah { get; set; }
        public DateTime? akt_pengajuan_sanggah { get; set; }
        public DateTime? target_jawaban_sanggah { get; set; }
        public DateTime? akt_jawaban_sanggah { get; set; }
        public DateTime? target_penunjukan_pemenang { get; set; }
        public DateTime? akt_penunjukan_pemenang { get; set; }
        public DateTime? target_spb { get; set; }
        public DateTime? akt_spb { get; set; }
        public DateTime? bulan { get; set; }
        public DateTime? tahun { get; set; }
    }
}
