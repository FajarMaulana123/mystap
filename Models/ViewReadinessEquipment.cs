using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace mystap.Models
{
    [Keyless]
    public class ViewReadinessEquipment
    {
        public long id { get; set; }
        public long? projectID { get; set; }
        public string? revision { get; set; }
        public string? jobNo { get; set; }
        public string? eqTagNo { get; set; }
        public string? status_tagno { get; set; }
    }
}
