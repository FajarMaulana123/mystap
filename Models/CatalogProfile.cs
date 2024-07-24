using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace mystap.Models
{
    [Table("catalog_profile")]
    public class CatalogProfile
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }
        public string? code { get; set; }
        public string? disiplin {  get; set; }
        public string? equipment_class { get; set; }
        public string? equipment_group { get; set; }
        public string? long_description { get; set; }
        public string? createdBy { get; set; }
        public string? created_date { get; set; }
        public string? deleted {  get; set; }
        public string? deletedBy { get; set; }
        public string? lastmodify { get; set; }
        public string? modifyBy { get; set; }
        public string? groups { get; set; }
        public string? object_type { get; set; }
        public string? sort_num { get; set; }

        public Users users { get; set; }
    }
}
