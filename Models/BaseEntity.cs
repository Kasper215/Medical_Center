using System;
using System.ComponentModel;

namespace MedCenterApp.Models;

public abstract class BaseEntity
{
    [DisplayName("ID")]
    public int Id { get; set; }
    
    [Browsable(false)]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
