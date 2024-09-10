using Microsoft.EntityFrameworkCore;

namespace mystap.Models
{
    [Keyless]
    public class ViewCountEksekusi
    {
        public int? di_kerjakan { get; set; }
        public int? tidak_dikerjakan { get; set; }
    }
}
