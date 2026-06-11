using GymMangmentSystem.BLL.ViewModels.HomeViewModels;

namespace GymMangmentSystem.BLL.Services.InterFaces
{
    public interface IAnalyticsService
    {
        Task<DashboardViewModel> GetDashboardAsync(CancellationToken ct = default);
    }
}
