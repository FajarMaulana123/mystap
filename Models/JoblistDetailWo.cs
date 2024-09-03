using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace mystap.Models
{
    [Table("joblist_detail_wo")]
    public class JoblistDetailWo
    {
        public long Id { get; set; }
        public long? JobListDetailID { get; set; }
        public string? order { get; set; }

    }
}
