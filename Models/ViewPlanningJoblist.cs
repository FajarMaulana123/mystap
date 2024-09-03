using Microsoft.EntityFrameworkCore;

namespace mystap.Models
{
    [Keyless]
    public class ViewPlanningJoblist
    {
        public long id { get; set; }
        public long? id_project { get; set; }
        public long? joblist_id { get; set; }
        public long? id_paket { get; set; }
        public string? eqTagNo { get; set; }
        public string? jobNo { get; set; }
        public string? jobDetailNo { get; set; }
        public string? jobDesc { get; set; }
        public string? alasan { get; set; }
        public string? wo { get; set; }
        public string? no_memo { get; set; }
        public string? file_memo { get; set; }
        public string? file_bom { get; set; }
        public string? noPaket { get; set; }
        public string? wo_jasa { get; set; }
        public string? judul_paket { get; set; }
        public string? no_po { get; set; }
        public string? no_sp { get; set; }
        public string? engineer { get; set; }
        public string? revision { get; set; }
        public string? execution { get; set; }
        public string? responsibility { get; set; }
        public string? ram { get; set; }
        public int? cleaning { get; set; }
        public int? inspection { get; set; }
        public int? repair { get; set; }
        public int? replace { get; set; }
        public int? ndt { get; set; }
        public int? modif { get; set; }
        public int? tein { get; set; }
        public int? coc { get; set; }
        public int? drawing { get; set; }
        public int? measurement { get; set; }
        public int? hsse { get; set; }
        public int? reliability { get; set; }
        public int? losses { get; set; }
        public int? energi { get; set; }
        public string? disiplin { get; set; }
        public int? project { get; set; }
        public int? critical_job { get; set; }
        public int? freezing { get; set; }
        public long? pic { get; set; }
        public string? alias { get; set; }
        public int? deleted { get; set; }
        public int? jasa { get; set; }
        public int? no_jasa { get; set; }
        public string? status_jasa { get; set; }
        public int? material { get; set; }
        public int? no_order { get; set; }
        public string? status_material { get; set; }
        public int? lldi { get; set; }
        public int? all_in_kontrak { get; set; }
        public string? order_jasa { get; set; }
        public string? pekerjaan { get; set; }
        public string? ket_status_material { get; set; }
        public string? status_job { get; set; }
        public long? notif { get; set; }
        public string? dikerjakan { get; set; }
        public string? mitigasi { get; set; }
        public string? keterangan { get; set; }
        public string? status { get; set; }
        public string? no_notif { get; set; }
        public string? link_rekomendasi { get; set; }
    }
}
