using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
namespace mystap.Models
{
	[PrimaryKey(nameof(id_modul))]
	public class Modul
    {
        public string id_modul {  get; set; }
        public string? nama {  get; set; }
        public string? alias {  get; set; }
        public string? group {  get; set; }
        public string? description {  get; set; }
        public string? status {  get; set; }

    }
}
