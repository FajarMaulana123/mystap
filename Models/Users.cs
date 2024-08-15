using System.ComponentModel.DataAnnotations.Schema;

namespace mystap.Models
{
    public class Users
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }
        public string? name { get; set; }
        public string? email { get; set; }
        public DateTime? email_verified_at { get; set; }
        public string? password { get; set; }
        public string? role { get; set; }
        public string? noPekerja { get; set; }
        public string? jabatan { get; set; }
        public string? pu { get; set; }
        public string? engineerCat { get; set; }
        public string? asal { get; set; }
        public string? alias { get; set; }
        public string? uSection { get; set; }
        public string? subSection { get; set; }
        public string? status { get; set; }
        public DateTime? lastLogin { get; set; }
        public string? remember_token { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public string? username { get; set; }
        public string? statPekerja { get; set; }
        public int? deleted { get; set; }
        public int? updated { get; set; }
        public int? deletedBy {  get; set; }
        public int? updatedBy { get; set; }
        public int? createBy { get; set; }
        public string? manPower { get; set; }
        public int? projectID { get; set; }
        public string? noSPKontrak { get; set; }
        public string? kodeBAU { get; set; }
        public string? plant {  get; set; }
        public int? locked { get; set; }
        public string? foto { get; set; }


    }
}
