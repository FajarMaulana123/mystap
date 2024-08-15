using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace mystap.Models
{
    [Table("catalog_profile")]
    public class CatalogProfile
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }
        public int? code { get; set; }
        public string? disiplin {  get; set; }
        public string? equipment_class { get; set; }
        public string? equipment_group { get; set; }
        public string? long_description { get; set; }
        public long? createdBy { get; set; }
        public DateTime? created_date { get; set; }
        public int? deleted {  get; set; }
        public long? deletedBy { get; set; }
        public DateTime? lastmodify { get; set; }
        public long? modifyBy { get; set; }
        public string? groups { get; set; }
        public int? object_type { get; set; }
        public int? sort_num { get; set; }


    }
}
