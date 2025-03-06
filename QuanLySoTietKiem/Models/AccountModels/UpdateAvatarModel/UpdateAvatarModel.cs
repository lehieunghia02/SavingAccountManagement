using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace QuanLySoTietKiem.Models.AccountModels.UpdateAvatarModel
{
  public class UpdateAvatarModel
  {
    [Required(ErrorMessage = "Vui lòng chọn ảnh")]
    public IFormFile AvatarImage { get; set; }
  }
}