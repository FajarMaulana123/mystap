using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace mystap.Models
{
    public class RapatViewModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }
        public long? id_project { get; set; }
        public DateTime? tanggal { get; set; }
        public string? judul { get; set; }
        public IFormFile materi { get; set; }
        public IFormFile notulen { get; set; }
        public long? created_by { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date")]
        public DateTime? created_date { get; set; }
        public int? deleted { get; set; }


        [ForeignKey("created_by")]
        public Users? users { get; set; }

        [ForeignKey("id_project")]
        public Project? project { get; set; }

    }
}
