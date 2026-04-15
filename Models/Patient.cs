using System;
using System.ComponentModel;

namespace MedCenterApp.Models;

public class Patient : BaseEntity
{
    [DisplayName("Фамилия")]
    public string LastName { get; set; } = "";
    
    [DisplayName("Имя")]
    public string FirstName { get; set; } = "";
    
    [DisplayName("Отчество")]
    public string MiddleName { get; set; } = "";
    
    [DisplayName("Дата рождения")]
    public DateTime BirthDate { get; set; } = DateTime.Today;
    
    [DisplayName("Телефон")]
    public string Phone { get; set; } = "";
    
    [DisplayName("Адрес")]
    public string Address { get; set; } = "";
    
    [DisplayName("Номер полиса")]
    public string PolicyNumber { get; set; } = "";
    
    [Browsable(false)]
    public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();
    
    public override string ToString() => FullName;
}
