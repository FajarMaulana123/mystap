using Microsoft.EntityFrameworkCore;

namespace mystap.Models
{
    [Keyless]
    public class ViewReservasi
    {
        public long? id { get; set; }
        public string? order { get; set; }
        public string? wo_description { get; set; }
        public string? main_work_ctr { get; set; }
        public string? revision { get; set; }
        public string? reserv_no { get; set; }
        public string? material { get; set; }
        public string? description { get; set; }
        public string? itm { get; set; }
        public string? bun { get; set; }
        public string? reqmt_qty { get; set; }
        public string? pr { get; set; }
        public string? pr_item { get; set; }
        public string? pr_qty { get; set; }
        public string? qty_res { get; set; }
        public string? po { get; set; }
        public string? po_item { get; set; }
        public string? po_qty { get; set; }
        public string? dci { get; set; }
        public int? tot_ { get; set; }
        public string? fls { get; set; }
        public int? diff_qty { get; set; }
        public int? dt_ { get; set; }
        public DateTime? finish_date { get; set; }
        public DateTime? md { get; set; }
        public DateTime? prognosa_ { get; set; }
        public string? status_ { get; set; }
        public string? status_ready { get; set; }
        public string? status_qty { get; set; }

    }
}
