using System.ComponentModel.DataAnnotations;

namespace mystap.Models
{
    public class Plans
    {
        public long id { get; set; }
        public string? plans { get; set; }
        public string? planDesc { get; set; }

        public string? created_by { get; set; }
        //public long? createdBy { get; set; }
        public DateTime? createdDate { get; set; }
        public DateTime? lastModify { get; set; }
        public long? modifyBy { get; set; }
        public int? deleted { get; set; }
    }
}
