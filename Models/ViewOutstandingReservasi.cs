﻿using Microsoft.EntityFrameworkCore;

namespace mystap.Models
{
    [Keyless]
    public class ViewOutstandingReservasi
    {
        public long? id { get; set; }
        public string? order { get; set; }
        public string? wo_description { get; set; }
        public string? main_work_ctr { get; set; }
        public string? revision { get; set; }
        public string? reserv_no { get; set; }
        public string? material { get; set; }
        public string? material_desc { get; set; }
        public string? itm { get; set; }
        public string? sloc { get; set; }
        public string? pg { get; set; }
        public string? ict { get; set; }
        public string? del { get; set; }
        public string? recipient { get; set; }
        public string? unloading_point { get; set; }
        public string? fls { get; set; }
        public string? cost_ctrs { get; set; }
        public string? reqmt_date { get; set; }
        public string? bun { get; set; }
        public string? reqmt_qty { get; set; }
        public string? qty_f_avail_check { get; set; }
        public string? qty_withdrawn { get; set; }
        public string? price { get; set; }
        public string? per { get; set; }
        public string? crcy { get; set; }
        public string? pr { get; set; }
        public string? pr_item { get; set; }
        public string? pr_qty { get; set; }
        public string? qty_res { get; set; }
        public int? prognosa_matl { get; set; }
    }
}