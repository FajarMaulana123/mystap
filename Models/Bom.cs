using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace mystap.Models
{
    public class Bom
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }
        public string? no_wo { get; set; }
        public string? tag_no { get; set; }
        public long? id_project { get; set; }
        public string? disiplin { get; set; }
        public DateTime? created_date { get; set; }
        public long? created_by { get; set; }
        public DateTime? last_modify { get; set; }
        public long? modify_by { get; set; }
        public int? deleted { get; set; }

        [ForeignKey("created_by")]
        public Users? users { get; set; }

        [ForeignKey("id_project")]
        public Project? project { get; set; }
       
    }
}
