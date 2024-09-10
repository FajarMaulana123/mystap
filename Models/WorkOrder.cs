using System.ComponentModel.DataAnnotations.Schema;

namespace mystap.Models
{
    [Table("work_order")]
    public class WorkOrder
    {
        public long id { get; set; }
        public string? plant { get; set; }
        public string? notification { get; set; }
        public DateTime? created_on { get; set; }
        public string? superior_order { get; set; }
        public string? order { get; set; }
        public string? description { get; set; }
        public string? equipment { get; set; }
        public string? func_loc { get; set; }
        public string? location { get; set; }
        public string? revision { get; set; }
        public string? system_status { get; set; }
        public int? user_status { get; set; }
        public string? wbs_ord_header { get; set; }
        public int? total_plnnd_costs { get; set; }
        public int? total_act_costs { get; set; }
        public string? planner_group { get; set; }
        public string? main_work_ctr { get; set; }
        public string? changed_by { get; set; }
        public DateTime? bas_start_date { get; set; }
        public DateTime? basic_fin_date { get; set; }
        public DateTime? actual_release { get; set; }
        public string? cost_center { get; set; }
        public string? entered_by { get; set; }
    }
}
