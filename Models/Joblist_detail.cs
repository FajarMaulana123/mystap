using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mystap.Models
{
    public class Joblist_Detail
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }
        public long? joblist_id { get; set; }
        public long? id_paket { get; set; } 
        public string? jobNo { get; set; }
        public string? jobDetailNo { get; set; }
        public string? jobDesc { get; set; }
        public string? alasan { get; set; }
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

        //[ForeignKey("createBy")]
        //public Users users { get; set; }

        //[ForeignKey("joblist_id")]
        //public Joblist joblist { get; set; }

        //[ForeignKey("unitCode")]
        //public Unit unit { get; set; }
        //public Equipments equipments { get; set; }
        //public ContractTracking contracttracking { get; set; }


    }
}
