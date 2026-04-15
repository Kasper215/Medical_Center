using MedCenterApp.Models;

namespace MedCenterApp.Services;

public class Validator : IValidator
{
    public bool ValidatePatient(Patient patient, out string error)
    {
        if (string.IsNullOrWhiteSpace(patient.LastName) ||
            string.IsNullOrWhiteSpace(patient.FirstName) ||
            string.IsNullOrWhiteSpace(patient.Phone))
        {
            error = "Обязательные поля (Фамилия, Имя, Телефон) не могут быть пустыми.";
            return false;
        }
        error = "";
        return true;
    }

    public bool ValidateDoctor(Doctor doctor, out string error)
    {
        if (string.IsNullOrWhiteSpace(doctor.LastName) ||
            string.IsNullOrWhiteSpace(doctor.FirstName) ||
            string.IsNullOrWhiteSpace(doctor.Specialty))
        {
            error = "Обязательные поля (Фамилия, Имя, Специальность) не могут быть пустыми.";
            return false;
        }
        error = "";
        return true;
    }
}
