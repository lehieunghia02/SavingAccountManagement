﻿namespace QuanLySoTietKiem.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<(bool succeeded, string message, string returnUrl)> LoginAsync(string userName, string password, bool isPersistent = false);
        Task LogoutAsync();
    }
}
