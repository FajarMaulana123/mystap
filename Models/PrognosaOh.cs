using System.ComponentModel.DataAnnotations;
namespace mystap.Models
{
    public class PrognosaOh
    {
        public long id { get; set; }
        public int? id_joblist { get; set; }
        public string? eqTagNo { get; set; }
        public string? revision { get; set; }
        public DateTime? start_date { get; set; }

    }
}
