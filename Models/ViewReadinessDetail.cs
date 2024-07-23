using Microsoft.EntityFrameworkCore;

namespace mystap.Models
{
    [Keyless]
    public class ViewReadinessDetail
    {
        public long id { get; set; }
        public long joblist_id { get; set; }
        public string? eqTagNo { get; set; }
        public long projectID { get; set; }
        public string? wo { get; set; }
        public int? no_jasa { get; set; }
        public string? jobDesc { get; set; }
        public int? jasa { get; set; }
        public int? material {  get; set; }
        public string sts_kontrak { get; set; }
        public string sts_material { get; set; }

    }
}
