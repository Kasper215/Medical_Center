using MedCenterApp.Models;

namespace MedCenterApp.Services;

public interface IAuthService
{
    User? Login(string login, string password);
    void Logout();
    User? CurrentUser { get; }
}
