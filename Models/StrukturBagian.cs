using System.ComponentModel.DataAnnotations;
namespace mystap.Models
{
    public class StrukturBagian
    {
        public long id { get; set; }
        public string? fungsi { get; set; }
        public string? bagian { get; set; }
        public int? createdBy { get; set; }
        public DateTime? createdDate { get; set; }
        public long? modifiedBy { get; set; }
        public DateTime? lastModifiy { get; set; }
        public int? deleted { get; set; }

    }
}
