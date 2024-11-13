using Microsoft.EntityFrameworkCore;

namespace mystap.Models
{
    [Keyless]
    public class ViewBom
    {
        public long? id { get; set; }
        public string? no_wo { get; set; }
        public string? tag_no { get; set; }
        public long? id_project { get; set; }
        public string? disiplin { get; set; }
        public DateTime? created_date { get; set; }
        public long? created_by { get; set; }
        public DateTime? last_modify { get; set; }
        public long? modify_by { get; set; }
        public int? deleted { get; set; }
        public string? file_bom { get; set; }
        public string? alias { get; set; }

    }
}
