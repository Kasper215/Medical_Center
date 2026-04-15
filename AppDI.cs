using MedCenterApp.Data;
using MedCenterApp.Models;
using MedCenterApp.Services;

namespace MedCenterApp;

public static class AppDI
{
    public static IRepository<User> Users { get; } = new JsonRepository<User>("users");
    public static IRepository<Patient> Patients { get; } = new JsonRepository<Patient>("patients");
    public static IRepository<Doctor> Doctors { get; } = new JsonRepository<Doctor>("doctors");
    public static IRepository<Appointment> Appointments { get; } = new JsonRepository<Appointment>("appointments");

    public static IAuthService AuthService { get; } = new AuthService(Users);
    public static IValidator Validator { get; } = new Validator();
}
