using System.ComponentModel.DataAnnotations;
namespace mystap.Models
{
    public class Catprof
    {
        public long Id { get; set; }
        public string? code { get; set; }
        public string? desc { get; set; }
        public string? disiplin { get; set; }
        public string? Lampiran { get; set; }
        public string? object_type { get; set; }
        public string? groups { get; set; }
        public string? eqClass { get; set; }
        public string? eqGroup { get; set; }
        public int? sortNum { get; set; }
        public int? deleted { get; set; }
        public long? createdBy { get; set; }
        public DateTime? createdDate { get; set; }
        public DateTime? lastModify { get; set; }
        public long? modifyBy { get; set; }

    }
}
