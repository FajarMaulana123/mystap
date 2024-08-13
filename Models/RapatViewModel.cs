
using System.ComponentModel.DataAnnotations;

namespace mystap.Models
{
    public class RapatViewModel
    {

        public long id { get; set; }
        public long? id_project { get; set; }
        public DateTime? tanggal { get; set; }
        public string? judul { get; set; }
        public IFormFile? materi { get; set; }
        public IFormFile? notulen { get; set; }

    }
}
