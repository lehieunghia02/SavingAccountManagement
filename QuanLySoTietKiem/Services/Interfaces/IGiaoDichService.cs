using QuanLySoTietKiem.Entity;
using QuanLySoTietKiem.Models.GiaoDichModels;

namespace QuanLySoTietKiem.Services.Interfaces
{
    public interface IGiaoDichService
    {
      public Task<GiaoDich> CreateGiaoDich(GiaoDichModel model);
      public Task<List<GiaoDich>> GetAllGiaoDichAsync(); 
    }
}