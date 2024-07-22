using System.ComponentModel.DataAnnotations;
namespace mystap.Models
{
    public class Disiplin
    {
        public long id { get; set; }
        public string? disiplin {  get; set; }
        public int? deleted { get; set; }
        public string? inisial { get; set; }

    }
}
