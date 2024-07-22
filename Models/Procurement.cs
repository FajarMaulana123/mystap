using System.ComponentModel.DataAnnotations;
namespace mystap.Models
{
    public class Procurement
    {
        public long id { get; set; }
        public int? id_project { get; set; }
        public string? no_order { get; set; }
        public string? desc { get; set; }
        public string? equipment { get; set; }
        public DateTime? realease_order { get; set; }
        public DateTime? change_order { get; set; }
        public int? no_item { get; set; }
        public string? no_material { get; set; }
        public string? material_desc { get; set; }
        public string? po_text { get; set; }
        public string? manufacture { get; set; }
        public int? qty { get; set; }
        public string? unit { get; set; }
        public string? diff { get; set; }
        public string? revision { get; set; }
        public string? pg { get; set; }
        public string? material { get; set; }
        public string? analis { get; set; }
        public string? ast_analis { get; set; }
        public string? progress { get; set; }
        public string? pr { get; set; }
        public int? item_pr { get; set; }
        public int? qty_pr { get; set; }
        public DateTime? tgl_pr { get; set; }
        public string? tkdn { get; set; }
        public DateTime? tgl_tkdn { get; set; }
        public string? status_pengadaan { get; set; }
        public string? po { get; set; }
        public int? item_po { get; set; }
        public int? qty_po { get; set; }
        public DateTime? tgl_po { get; set; }
        public DateTime? delive_date { get; set; }
        public string? lldi { get; set; }
    }
}
