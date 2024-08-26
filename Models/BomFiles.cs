using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace mystap.Models
{
    public class BomFiles
    {
        public long id { get; set; }
        public long id_bom {  get; set; }
        public String ? files { get; set; }

        [ForeignKey("id_bom")]
        public Bom bom { get; set; }

    }
}
