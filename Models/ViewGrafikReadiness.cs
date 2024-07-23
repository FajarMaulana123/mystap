using Microsoft.EntityFrameworkCore;

namespace mystap.Models
{
    [Keyless]
    public class ViewGrafikReadiness
    {
        public string status_tagno { get; set; }
        public int total { get; set; }
    }
}
