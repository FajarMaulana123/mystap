using System.ComponentModel.DataAnnotations.Schema;

namespace mystap.Models
{
    [Table("fungsi_bagian")]
    public class FungsiBagian
    {
        public long id { get; set; }
        public string? fungsi { get; set; }
        public string? bagian { get; set; }
        public DateTime? createdDate { get; set; }
        public long? createdBy { get; set; }
        public DateTime? lastModify { get; set; }
        public long? modifiedBy { get; set; }
        public int? deleted { get; set; }
    }
}
