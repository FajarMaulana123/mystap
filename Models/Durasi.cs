using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace mystap.Models
{
    public class Durasi
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }
        public long? id_project { get; set; }
        public string? kat_tender { get; set; }
        public int? susun_kak { get; set; }
        public int? susun_oe { get; set; }
        public int? persetujuan { get; set; }
        public int? kirim_ke_co { get; set; }
        public int? pengumuman_pendaftaran { get; set; }
        public int? sertifikasi { get; set; }
        public int? prakualifikasi { get; set; }
        public int? undangan { get; set; }
        public int? pemberian { get; set; }
        public int? penyampaian { get; set; }
        public int? pembukaan { get; set; }
        public int? evaluasi { get; set; }
        public int? negosiasi { get; set; }
        public int? usulan { get; set; }
        public int? keputusan { get; set; }
        public int? pengumuman_pemenang { get; set; }
        public int? pengajuan_sanggah { get; set; }
        public int? jawaban_sanggah { get; set; }
        public int? tunjuk_pemenang { get; set; }
        public int? proses_spb { get; set; }
        private int? total { get; set; }

        [ForeignKey("id_project")]
        public Project project { get; set; }

    }
}
