using AutoMapper;
using GymMangmentSystem.BLL.Services.InterFaces;
using GymMangmentSystem.BLL.ViewModels.PlanViewModels;
using GymMangmentSystem.DAL.Models;
using GymMangmentSystem.DAL.Repositories.Interfaces;

namespace GymMangmentSystem.BLL.Services.Classes
{
    public class PlanService : IPlanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private IGenericRepository<Plan> _planRepository => _unitOfWork.GetRepository<Plan>();

        public PlanService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default)
        {
            var plans = await _planRepository.GetAllAsync(ct: ct);
            if (!plans.Any()) return [];
            return _mapper.Map<IEnumerable<PlanViewModel>>(plans);
        }

        public async Task<PlanViewModel?> GetPlanDetailsAsync(int id, CancellationToken ct = default)
        {
            var plan = await _planRepository.GetByIdAsync(id, ct);
            if (plan == null) return null;
            return _mapper.Map<PlanViewModel>(plan);
        }

        public async Task<PlanToUpdateViewModel?> GetPlanToUpdateAsync(int id, CancellationToken ct = default)
        {
            var plan = await _planRepository.GetByIdAsync(id, ct);
            if (plan == null) return null;
            return _mapper.Map<PlanToUpdateViewModel>(plan);
        }

        public async Task<bool> UpdatePlanAsync(PlanToUpdateViewModel model, CancellationToken ct = default)
        {
            var plan = await _planRepository.GetByIdAsync(model.Id, ct);
            if (plan == null) return false;

            plan.DurationInDays = model.DurationDays;
            plan.Price = model.Price;
            plan.Description = model.Description;
            plan.UpdatedAt = DateTime.Now;

            var result = await _planRepository.UpdateAsync(plan);
            return result > 0;
        }

        public async Task<bool?> ToggleActiveAsync(int id, CancellationToken ct = default)
        {
            var plan = await _planRepository.GetByIdAsync(id, ct);
            if (plan == null) return null;

            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.Now;
            await _planRepository.UpdateAsync(plan);

            return plan.IsActive;
        }
    }
}
