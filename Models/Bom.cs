using System.ComponentModel.DataAnnotations;
namespace mystap.Models
{
    public class Bom
    {
        public long Id { get; set; }
        public string? tag_no { get; set; }
        public string? id_project { get; set; }
        public string? disiplin { get; set; }
        public DateTime? created_date { get; set; }
        public string? created_by { get; set; }
        public DateTime? last_modify { get; set; }
        public string? modify_by { get; set; }
        public string? deleted { get; set; }
    }
}
