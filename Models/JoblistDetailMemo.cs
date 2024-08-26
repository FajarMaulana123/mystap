using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace mystap.Models
{
    [Table("joblist_detail_memo")]
    public class JoblistDetailMemo
    {
        public long Id { get; set; }
        public long jobListDetailID { get; set; }
        public long ? id_memo { get; set; }

    }
}
