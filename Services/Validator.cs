using System.Linq;
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

        // Allow digits and +, -, (, ), space
        if (!patient.Phone.All(c => char.IsDigit(c) || "+()- ".Contains(c)))
        {
            error = "Номер телефона содержит недопустимые символы.";
            return false;
        }

        if (!string.IsNullOrWhiteSpace(patient.PolicyNumber) && !patient.PolicyNumber.All(char.IsDigit))
        {
            error = "Номер полиса должен содержать только цифры.";
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

        if (!string.IsNullOrWhiteSpace(doctor.Phone) && !doctor.Phone.All(c => char.IsDigit(c) || "+()- ".Contains(c)))
        {
            error = "Номер телефона содержит недопустимые символы.";
            return false;
        }

        error = "";
        return true;
    }
}
