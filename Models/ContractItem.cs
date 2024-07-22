using System.ComponentModel.DataAnnotations;
namespace mystap.Models
{
    public class ContractItem
    {
        public long id { get; set; }
        public string? item_top_group { get; set; }
        public string? item_group {  get; set; }
        public string? item {  get; set; }
        public int? deleted { get; set; }
        public int? rec_order { get; set; }
        public string? plant { get; set; }
        public string? initial { get; set; }

    }
}
