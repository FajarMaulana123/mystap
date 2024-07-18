using Microsoft.EntityFrameworkCore;

namespace mystap.Models
{
    [Keyless]
    public class ViewReadinessEquipment
    {
        public string jobNo { get; set; }
        public string eqTagNo { get; set; }
        public string status_tagno { get; set; }
    }
}
