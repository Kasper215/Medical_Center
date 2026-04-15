using MedCenterApp.Models;

namespace MedCenterApp.Services;

public interface IValidator
{
    bool ValidatePatient(Patient patient, out string error);
    bool ValidateDoctor(Doctor doctor, out string error);
}
