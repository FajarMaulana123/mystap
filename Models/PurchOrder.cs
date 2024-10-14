using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
namespace mystap.Models
{
    [PrimaryKey(nameof(pr), nameof(item_pr))]
    public class PurchOrder
    {
       
        public string material { get; set; }
        public string pr { get; set; }
        public string item_pr { get; set; }
        public string? gl_acct { get; set; }
        public string? short_text { get; set; }
        public string? po { get; set; }
        public string? item_po { get; set; }
        public string? d { get; set; }
        public string? dci { get; set; }
        public string? pgr { get; set; }
        public string? doc_date { get; set; }
        public string? po_quantity { get; set; }
        public string? qty_delivered { get; set; }
        public string? deliv_date { get; set; }
        public string? oun { get; set; }
        public string? net_price { get; set; }
        public string? crcy { get; set; }
        public string? per { get; set; }

    }
}
