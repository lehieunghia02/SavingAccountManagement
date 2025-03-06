using Microsoft.EntityFrameworkCore;
using QuanLySoTietKiem.Data;
using QuanLySoTietKiem.Entity;
using QuanLySoTietKiem.Models.GiaoDichModels;
using QuanLySoTietKiem.Services.Interfaces;

namespace QuanLySoTietKiem.Services;

public class GiaoDichService : IGiaoDichService
{
    private readonly ApplicationDbContext _context;
    public GiaoDichService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<GiaoDich> CreateGiaoDich(GiaoDichModel model)
    {
        var giaoDich = new GiaoDich
        {
            MaSoTietKiem = model.MaSoTietKiem,
            MaLoaiGiaoDich = model.MaLoaiGiaoDich,
            NgayGiaoDich = model.NgayGiaoDich,
            SoTien = (decimal)model.SoTien,
        };
        _context.GiaoDichs.Add(giaoDich);
        await _context.SaveChangesAsync();

        //Map back to model 
        model.MaGiaoDich = giaoDich.MaGiaoDich;
        return giaoDich;
    }
    public async Task<List<GiaoDich>> GetAllGiaoDichAsync()
    {
        return await _context.GiaoDichs.Include(g => g.MaSoTietKiem).ToListAsync();
    }
}