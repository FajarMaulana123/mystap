using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace mystap.Models
{
    [Keyless]
    public class ViewDetailOrder
    {
        
        public string main_work_ctr {  get; set; }
        public string order { get; set; }
        public string description {  get; set; }
        public string material { get; set; }
        public string itm {  get; set; }
        public string material_description { get; set; }
        public string? del {  get; set; }
        public string? unloading_point { get; set; }
        public string bun {  get; set; }
        public string reqmt_date { get; set; }
        public string reqmt_qty {  get; set; }
        public string? qty_res { get; set; }
        public string? pr {  get; set; }
        public string? pr_item { get; set; }
        public string? pr_qty { get; set; }
        public string? status_pengadaan { get; set; }
        public string? po { get; set; }
        public string? po_qty { get; set; }
        public string? po_item { get; set; }
        public string? deliv_date {  get; set; }
        public string? dci { get;set; }
        public string lld {  get; set; }
        public string status_ { get;set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? prognosa_ { get; set; }
    }
}
