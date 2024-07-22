using System.ComponentModel.DataAnnotations;
namespace mystap.Models
{
    public class Notifikasi
    {
        public long id { get; set; }
        public string? notification_type { get; set; }
        public string? notifikasi {  get; set; }
        public string? order {  get; set; }
        public DateTime? notification_date {  get; set; }
        public string? created_by {  get; set; }
        public DateTime? created_on {  get; set; }
        public string? change_by {  get; set; }
        public DateTime? change_on {  get; set; }
        public string? planner_group {  get; set; }
        public string? description {  get; set; }
        public int? user_status {  get; set; }
        public string? system_status {  get; set; }
        public string? maintenance_plant {  get; set; }
        public string? functional_location {  get; set; }
        public string? equipment {  get; set; }
        public DateTime? required_start {  get; set; }
        public DateTime? required_end {  get; set; }
        public string? location {  get; set; }
        public string? main_work_center {  get; set; }
        public string? maintenance_item {  get; set; }
        public string? maintenance_plan {  get; set; }
        public string? rekomendasi {  get; set; }

    }
}
