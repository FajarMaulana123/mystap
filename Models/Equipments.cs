using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace mystap.Models
{
    public class Equipments
    {
        public long id { get; set; }
        public string? eqTagNo { get; set; }
        public string? eqDesc { get; set; }
        public string? funcLocID { get; set; }
        public string? weight { get; set; }
        public string? weight_unit { get; set; }
        public string? size { get; set; }
        public string? start_up_date { get; set; }
        public string? acquisition_value { get; set; }
        public string? currency_key { get; set; }
        public string? acquisition_date { get; set; }
        public string? planning_plant { get; set; }
        public string? planner_group { get; set; }
        public string? main_work_center { get; set; }
        public int? catalog_profile { get; set; }
        public string? maint_plant { get; set; }
        public string? location { get; set; }
        public string? plant_section { get; set; }
        public string? main_asset_no { get; set; }
        public string? asset_sub_no { get; set; }
        public string? cost_center { get; set; }
        public string? WBS_element { get; set; }
        public string? Position { get; set; }
        public string? tin { get; set; }
        public string? manufacture { get; set; }
        public string? model { get; set; }
        public string? part_no { get; set; }
        public string? serial_no { get; set; }
        public string? eqp_cat { get; set; }
        public string? date_valid { get; set; }
        public string? object_type { get; set; }
        public string? country_of_manuf { get; set; }
        public string? year_of_const { get; set; }
        public string? month_of_const { get; set; }
        public string? plant_main_work_center { get; set; }
        public string? const_type { get; set; }
        public string? permit_assign { get; set; }
        public string? Criticallity { get; set; }
        public string? Remark { get; set; }
        public string? unitProses { get; set; }
        public string? createdBy { get; set; }
        public string? dateCreated { get; set; }
        public string? update { get; set; }
        public string? updateBy { get; set; }
        public string? deleted { get; set; }
        public string? deletedBy { get; set; }
        public string? responbility { get; set; }
        public string? craft { get; set; }
        public string? eqGroupID { get; set; }
        public string? unitKilang { get; set; }
        public string? catProf { get; set; }


    }
}
