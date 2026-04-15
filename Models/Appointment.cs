using System;
using System.ComponentModel;

namespace MedCenterApp.Models;

public class Appointment : BaseEntity
{
    [Browsable(false)]
    public int PatientId { get; set; }
    
    [Browsable(false)]
    public int DoctorId { get; set; }
    
    [DisplayName("Дата и время")]
    public DateTime DateTime { get; set; }
    
    [DisplayName("Услуга")]
    public string Service { get; set; } = "";
    
    [DisplayName("Диагноз")]
    public string Diagnosis { get; set; } = "";
    
    [DisplayName("Стоимость")]
    public decimal Cost { get; set; }
}
