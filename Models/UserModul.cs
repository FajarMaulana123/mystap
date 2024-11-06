using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace mystap.Models
{
	[Table("user_modul")]
	[PrimaryKey(nameof(id_modul), nameof(id_user))]
	public class UserModul
	{
		public string? id_modul { get; set; }
		public int id_user { get; set; }
		public DateTime? created_at { get; set; }
		public DateTime? updated_at { get; set; }
	}
}
