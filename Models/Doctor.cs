using System.ComponentModel;

namespace MedCenterApp.Models;

public class Doctor : BaseEntity
{
    [DisplayName("Фамилия")]
    public string LastName { get; set; } = "";
    
    [DisplayName("Имя")]
    public string FirstName { get; set; } = "";
    
    [DisplayName("Отчество")]
    public string MiddleName { get; set; } = "";
    
    [DisplayName("Специальность")]
    public string Specialty { get; set; } = "";
    
    [DisplayName("Телефон")]
    public string Phone { get; set; } = "";
    
    [Browsable(false)]
    public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();
    
    public override string ToString() => FullName;
}
