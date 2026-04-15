using System.ComponentModel;

namespace MedCenterApp.Models;

public class User : BaseEntity
{
    [DisplayName("Логин")]
    public string Login { get; set; } = "";
    
    [Browsable(false)]
    public string PasswordHash { get; set; } = "";
    
    [DisplayName("Роль")]
    public string Role { get; set; } = "Admin";
}
