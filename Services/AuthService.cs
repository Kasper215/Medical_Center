using System.Linq;
using MedCenterApp.Models;

namespace MedCenterApp.Services;

public class AuthService : IAuthService
{
    private readonly IRepository<User> _userRepository;
    
    public User? CurrentUser { get; private set; }

    public AuthService(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public User? Login(string login, string password)
    {
        // В рамках курса пароли храним как есть или простейший хеш
        // Для демонстрации находим пользователя с совпадающим логином и паролем
        var users = _userRepository.GetAll();
        
        // Если база пустая добавляем админа по умолчанию
        if (!users.Any())
        {
            var admin = new User { Login = "admin", PasswordHash = "admin", Role = "Admin" };
            _userRepository.Add(admin);
            users.Add(admin);
        }
        
        var user = users.FirstOrDefault(u => u.Login == login && u.PasswordHash == password);
        if (user != null)
        {
            CurrentUser = user;
            return user;
        }

        return null;
    }

    public void Logout()
    {
        CurrentUser = null;
    }
}
