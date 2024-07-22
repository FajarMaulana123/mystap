using System.ComponentModel.DataAnnotations;
namespace mystap.Models
{
    public class PrInv
    {
        public string? no_order { get; set; }
        public string? itm { get; set; }
        public string? material { get; set; }
        public string? material_description { get; set; }
        public string? reqmts_qty { get; set; }
        public string? uom { get; set; }
        public string? qty_reserved { get; set; }
        public string? purchreq { get; set; }
        public string? item { get; set; }
        public string? qty_purch { get; set; }
    }
}
