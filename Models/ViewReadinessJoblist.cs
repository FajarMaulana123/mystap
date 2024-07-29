using Microsoft.EntityFrameworkCore;

namespace mystap.Models
{
    [Keyless]
    public class ViewReadinessJoblist
    {
        public long id_paket {  get; set; }
        public string tag_no { get; set; }
        public DateTime created_date { get; set; }
        public string disiplin { get; set; }
        public string no_memo {  get; set; }
        public string description {  get; set; }
        public string revision {get; set; }
        public long projectID {  get; set; }
        public string no_add {  get; set; }
        public string status_tagno { get; set; }
    }
}
