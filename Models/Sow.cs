using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations.Schema;

namespace mystap.Models
{
    [Table("sow")]
    public class Sow
    {
        public long id {get; set;}
        public long? projectID {get; set;}
        public string? noSOW {get; set;}
        public string? events {get; set;}
        public string? groups {get; set;}
        public string? subGroups {get; set;}
        public string? area {get; set;}
        public string? kabo {get; set;}
        public string? tahun {get; set;}
        public string? judulPekerjaan {get; set;}
        public string? planner {get; set; }
        public string? jobCode {get; set; }
        public string? file { get; set; }
        public string? createdBy { get; set; }
        public DateTime? createdDate { get; set; }
        public string? modifyBy { get; set; }
        public DateTime? lastModify { get; set; }
        public int? deleted { get; set; }

        ////public Users users { get; set; }
        //[ForeignKey("projectID")]
        //public Project project { get; set; }


    }
}
