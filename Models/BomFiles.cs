using System.ComponentModel.DataAnnotations;
namespace mystap.Models
{
    public class BomFiles
    {
        public long id { get; set; }
        public int id_bom {  get; set; }
        public IFormFile ? files { get; set; }

    }
}
