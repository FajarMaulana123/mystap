using Microsoft.EntityFrameworkCore;

namespace mystap.Models
{
    [Keyless]
    public class ViewGrafikMaterial
    {
        public string? status_ {  get; set; }
        public int? total {  get; set; }

    }
}
