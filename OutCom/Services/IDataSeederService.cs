using Microsoft.AspNetCore.Identity;
using OutCom.Data;
using OutCom.Models;

namespace OutCom.Services
{
    public interface IDataSeederService
    {
        Task SeedDefaultDataAsync();
        Task<ApplicationUser> CreateDefaultAdminAsync(string email, string password);
        Task EnsureRolesExistAsync();
    }
}