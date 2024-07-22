namespace mystap.Models
{
    public class Unit
    {
        public long id { get; set; }
        public string? unitPlan { get; set; }
        public string? codeJob { get; set; }
        public string? unitCode { get; set; }
        public string? unitProses { get; set; }
        public string? unitKilang { get; set; }
        public string? unitGroup { get; set; }
        public string? groupName { get; set; }
        public string? unitName { get; set; }
        public string? unitNameDesc { get; set; }
        public string? pengawas { get; set; }
        public int? deleted { get; set; }
        public long? deletedBy { get; set; }
        public long? createdBy { get; set; }
        public DateTime? dateCreated { get; set; }
        public string? units { get; set; }

    }
}
