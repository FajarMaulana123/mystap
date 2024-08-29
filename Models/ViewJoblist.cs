using Microsoft.EntityFrameworkCore;

namespace mystap.Models
{
    [Keyless]
    public class ViewJoblist
    {
        public long id { get; set; }
        public long? id_eqtagno { get; set; }
        public string? jobNo { get; set; }
        public string? projectNo { get; set; }
        public string? description { get; set; }
        public string? nama_unit { get; set; }
        public string? name { get; set; }
        public string? revision { get; set; }
        public string? userSection { get; set; }
        public string? eqTagNo { get; set; }
        public string? keterangan {  get; set; }
        public string? status_tagno { get; set; }
    }
}
