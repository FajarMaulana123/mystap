using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mystap.Models
{
    [PrimaryKey(nameof(purch_req), nameof(item))]
    public class Zpm02
    {
        public string? plant { get; set; }
        public string? material { get; set; }
        public string? material_description { get; set; }
        public string? purch_req { get; set; }
        public int? item { get; set; }
        public string? d { get; set; }
        public string? rel { get; set; }
        public string? s { get; set; }
        public string? pgr { get; set; }
        public string? tracking_no { get; set; }
        public int? qty_req { get; set; }
        public string? un { get; set; }
        public DateTime? req_date { get; set; }
        public string? valn_price { get; set; }
        public string? crcy { get; set; }
        public int? per { get; set; }
        public DateTime? release_dt { get; set; }
    }
}
