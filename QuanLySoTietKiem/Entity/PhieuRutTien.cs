﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QuanLySoTietKiem.Models
{
    public class PhieuRutTien
    {
        [Key]
        public int MaPhieuRut { get; set; }

        [Required]
        public int MaSoTietKiem { get; set; }
        public DateTime NgayRut { get; set; }
        [Required]
        public decimal SoTienRut { get; set; }

        [ForeignKey("MaSoTietKiem")]
        public virtual SoTietKiem SoTietKiem { get; set; }
    }
}
