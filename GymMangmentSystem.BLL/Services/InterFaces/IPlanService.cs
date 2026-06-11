using GymMangmentSystem.BLL.ViewModels.PlanViewModels;

namespace GymMangmentSystem.BLL.Services.InterFaces
{
    public interface IPlanService
    {
        Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default);
        Task<PlanViewModel?> GetPlanDetailsAsync(int id, CancellationToken ct = default);
        Task<PlanToUpdateViewModel?> GetPlanToUpdateAsync(int id, CancellationToken ct = default);
        Task<bool> UpdatePlanAsync(PlanToUpdateViewModel model, CancellationToken ct = default);
        // null => plan not found; otherwise the new IsActive state
        Task<bool?> ToggleActiveAsync(int id, CancellationToken ct = default);
    }
}
