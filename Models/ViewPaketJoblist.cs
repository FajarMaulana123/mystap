using Microsoft.EntityFrameworkCore;

namespace mystap.Models
{
    [Keyless]
    public class ViewPaketJoblist
    {
        public long id_paket { get; set; }
        public long? id_project { get; set; }
        public long? no_paket { get; set; }
        public string? no_add { get; set; }
        public string? tag_no { get; set; }
        public string? no_memo { get; set; }
        public string? disiplin { get; set; }
        public string? project_rev { get; set; }
        public int? total { get; set; }
        public string? status_tagno { get; set; }
    }
}
