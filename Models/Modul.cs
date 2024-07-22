using System.ComponentModel.DataAnnotations;
namespace mystap.Models
{
    public class Modul
    {
        public long id_modul {  get; set; }
        public string? nama {  get; set; }
        public string? alias {  get; set; }
        public string? group {  get; set; }
        public string? description {  get; set; }
        public string? status {  get; set; }

    }
}
