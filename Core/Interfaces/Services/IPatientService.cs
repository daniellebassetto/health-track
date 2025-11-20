using HealthTrack.Areas.Patient.ViewModels;

namespace HealthTrack.Core.Interfaces.Services
{
    public interface IPatientService
    {
        Task<DashboardViewModel> GetDashboardDataAsync(string userId);
        Task<EditPatientViewModel> GetForEditAsync(string userId);
        Task CreateAsync(CreatePatientViewModel model, string userId);
        Task UpdateAsync(EditPatientViewModel model, string userId);
    }
}