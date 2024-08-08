using Microsoft.EntityFrameworkCore;

namespace mystap.Models
{
    [Keyless]
    public class ViewDetailMaterial
    {
        public string? order { get; set; }
        public string? itm { get; set; }
        public string? material { get; set; }
        public string? material_description { get; set; }
        public string? del { get; set; }
        public string? bun { get; set; }
        public string? reqmt_qty { get; set; }
        public string? pr { get; set; }
        public string? pr_item { get; set; }
        public string? pr_qty { get; set; }
        public string? qty_res { get; set; }
        public string? po { get; set; }
        public string? po_item { get; set; }
        public string? po_qty { get; set; }
        public string? main_work_ctr { get; set; }
        public int? dt_ { get; set; }
        public string? lld { get; set; }
        public DateTime? prognosa_ { get; set; }
        public string? status_ { get; set; }
    }
}
