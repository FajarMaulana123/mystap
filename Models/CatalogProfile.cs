using System.ComponentModel.DataAnnotations;
namespace mystap.Models
{
    public class CatalogProfile
    {
        public long id { get; set; }
        public int? code { get; set; }
        public string? disiplin {  get; set; }
        public string? equipment_class { get; set; }
        public string? equipment_group { get; set; }
        public string? long_description { get; set; }
        public string? createdBy { get; set; }
        public DateTime? created_date { get; set; }
        public int? deleted {  get; set; }
        public string? deletedBy { get; set; }
        public DateTime? lastmodify { get; set; }
        public string? modifyBy { get; set; }
        public string? groups { get; set; }
        public int? object_type { get; set; }
        public int? sort_num { get; set; }
    }
}
