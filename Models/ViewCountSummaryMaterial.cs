using Microsoft.EntityFrameworkCore;

namespace mystap.Models
{
    [Keyless]
    public class ViewCountSummaryMaterial
    {
        public string? status_ready { get; set; }
        public int? total {  get; set; }
    }
}
