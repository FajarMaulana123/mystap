﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mystap.Models
{
    public class Rapat
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }
        public string? id_project { get; set; }
        public DateTime? tanggal { get; set; }
        public string? judul {  get; set; }
        public string? materi {  get; set; }
        public string? notulen {  get; set; }
        public long? created_by {  get; set; }
        public DateTime? created_date {  get; set; }
        public int? deleted {  get; set; }

        [ForeignKey("created_by")]
        public Users? users { get; set; }

    }
}
