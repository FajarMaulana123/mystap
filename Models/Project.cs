﻿namespace mystap.Models
{
    public class Project
    {
        public long id { get; set; }
        public string? projectNo { get; set; }
        public string? description { get; set; }
        public string? revision { get; set; }
        public string? month { get; set; }
        public string? year { get; set; }
        public string? active { get; set; }
        public int? deleted { get; set; }
        public int? updated { get; set; }
        public DateTime? tglTA { get; set; }
        public DateTime? tglSelesaiTA { get; set; }
        public int? deleteBy { get; set; }
        public int? createdBy { get; set; }
        public DateTime? createdDate { get; set; }
        public DateTime? lastModify { get; set; }
        public long? modifyBy { get; set; }
        public long? plansID { get; set; }
        public int? durasiTABrick { get; set; }
        public string? finalDate { get; set; }
        public string? additional1Date { get; set; }
        public string? additional2Date { get; set; }
        public string? taoh { get; set; }
    }
}