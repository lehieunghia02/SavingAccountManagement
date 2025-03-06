using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuanLySoTietKiem.Entity;
using QuanLySoTietKiem.Models;

namespace QuanLySoTietKiem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<LoaiSoTietKiem> LoaiSoTietKiems { get; set; }
        public DbSet<SoTietKiem> SoTietKiems { get; set; }
        public DbSet<BaoCaoNgay> BaoCaoNgays { get; set; }
        public DbSet<BaoCaoThang> BaoCaoThangs { get; set; }
        public DbSet<HinhThucDenHan> HinhThucDenHans { get; set; }
        public DbSet<LoaiGiaoDich> LoaiGiaoDichs { get; set; }
        public DbSet<GiaoDich> GiaoDichs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define primary key for IdentityUserLogin<string>
            modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(l => new { l.LoginProvider, l.ProviderKey });

            modelBuilder.Entity<BaoCaoThang>().Property(b => b.ChenhLech).HasPrecision(18, 2);
            modelBuilder.Entity<BaoCaoNgay>().Property(b => b.TongTienGui).HasPrecision(18, 2);
            modelBuilder.Entity<BaoCaoNgay>().Property(b => b.TongTienRut).HasPrecision(18, 2);
            modelBuilder.Entity<LoaiSoTietKiem>().Property(l => l.SoTienGuiToiThieu).HasPrecision(18, 2);
            modelBuilder.Entity<SoTietKiem>().Property(s => s.SoDuSoTietKiem).HasPrecision(18, 2);
            modelBuilder.Entity<SoTietKiem>().Property(s => s.SoTienGui).HasPrecision(18, 2);
            modelBuilder.Entity<SoTietKiem>().Property(s => s.LaiSuatKyHan).HasPrecision(18, 3);
            modelBuilder.Entity<BaoCaoThang>().Property(b => b.TongTienGui).HasPrecision(18, 2);
            modelBuilder.Entity<BaoCaoThang>().Property(b => b.TongTienRut).HasPrecision(18, 2);
            modelBuilder.Entity<SoTietKiem>().Property(s => s.LaiSuatApDung).HasPrecision(18, 3);
            modelBuilder.Entity<GiaoDich>().Property(g => g.SoTien).HasPrecision(18, 2);



            //  Seed data for HinhThucDenHan
            modelBuilder.Entity<HinhThucDenHan>().HasData(
                new HinhThucDenHan() { MaHinhThucDenHan = 1, TenHinhThucDenHan = "Rút hết" },
                new HinhThucDenHan() { MaHinhThucDenHan = 2, TenHinhThucDenHan = "Quay vòng gốc" },
                new HinhThucDenHan() { MaHinhThucDenHan = 3, TenHinhThucDenHan = "Quay vòng cả gốc và lãi" }
            );
            //Seed data for LoaiGiaoDich
            modelBuilder.Entity<LoaiGiaoDich>().HasData(new LoaiGiaoDich()
            {
                MaLoaiGiaoDich = 1,
                TenLoaiGiaoDich = "Rút tiền"
            }, new LoaiGiaoDich()
            {
                MaLoaiGiaoDich = 2,
                TenLoaiGiaoDich = "Gửi tiền"
            });
        }
    }
}
