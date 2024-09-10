using Microsoft.EntityFrameworkCore;

namespace mystap.Models
{
    [Keyless]
    public class ViewUpdatePr
    {
        public long? id { get; set; }
        public string? order { get; set; }
        public string? material { get; set; }
        public string? description { get; set; }
        public string? itm { get; set; }
        public string? bun { get; set; }
        public string? reqmt_qty { get; set; }
        public string? reqmt_date { get; set; }
        public string? pr { get; set; }
        public string? pr_item { get; set; }
        public string? pr_qty { get; set; }
        public string? qty_res { get; set; }
        public string? pg { get; set; }
        public string? status_qty { get; set; }
    }
}
