using Microsoft.EntityFrameworkCore;

namespace mystap.Models
{
    [Keyless]
    public class ViewListPr
    {
        public string? pr { get; set; }
        public string? pr_item { get; set; }
        public string? qty_pr { get; set; }
        public string? pg { get; set; }
        public string? reqmt_date { get; set; }
        public string? order { get; set; }
        public string? material { get; set; }
        public string? material_description { get; set; }
    }
}
