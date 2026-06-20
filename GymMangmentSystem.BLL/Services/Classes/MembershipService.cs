using AutoMapper;
using GymMangmentSystem.BLL.Common;
using GymMangmentSystem.BLL.Services.InterFaces;
using GymMangmentSystem.BLL.ViewModels.MembershipViewModels;
using GymMangmentSystem.DAL.Models;
using GymMangmentSystem.DAL.Repositories.Interfaces;

namespace GymMangmentSystem.BLL.Services.Classes
{
    public class MembershipService : IMembershipService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MembershipService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MembershipViewModel>?> GetAllMembershipsAsync(CancellationToken ct = default)
        {
            var memberships = await _unitOfWork.MembershipRepository.GetAllMembershipsWithMemberAndPlanAsync(ct: ct);
            if (memberships?.Any() != true) return null;
            memberships = memberships.OrderByDescending(m => m.EndDate);
            return _mapper.Map<IEnumerable<MembershipViewModel>>(memberships);
        }

        public async Task<MembershipViewModel?> GetMembershipDetailsAsync(int memberId, int planId, CancellationToken ct = default)
        {
            var membership = await _unitOfWork.MembershipRepository.GetMembershipWithMemberAndPlanAsync(memberId, planId, ct);
            if (membership == null) return null;
            return _mapper.Map<MembershipViewModel>(membership);
        }

        public async Task<EditMembershipViewModel?> GetMembershipToEditAsync(int memberId, int planId, CancellationToken ct = default)
        {
            var membership = await _unitOfWork.MembershipRepository.GetMembershipWithMemberAndPlanAsync(memberId, planId, ct);
            if (membership == null) return null;
            return _mapper.Map<EditMembershipViewModel>(membership);
        }

        public async Task<Result> AssignMembershipAsync(CreateMembershipViewModel model, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(model.MemberId, ct);
            if (member == null) return Result.NotFound("Selected member was not found.");

            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(model.PlanId, ct);
            if (plan == null) return Result.NotFound("Selected plan was not found.");
            if (!plan.IsActive) return Result.ValidationError("Cannot assign an inactive plan.");

            var exists = await _unitOfWork.MembershipRepository.AnyAsync(m => m.MemberId == model.MemberId && m.PlanId == model.PlanId, ct);
            if (exists) return Result.Failure("This member already has a membership for this plan. Use renew instead.");

            var membership = new Membership
            {
                MemberId = model.MemberId,
                PlanId = model.PlanId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                EndDate = DateTime.Now.AddDays(plan.DurationInDays),
            };

            _unitOfWork.MembershipRepository.Add(membership);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0
                ? Result.Success()
                : Result.Failure("Failed to assign the membership.");
        }

        public async Task<Result> RenewMembershipAsync(int memberId, int planId, CancellationToken ct = default)
        {
            var membership = await _unitOfWork.MembershipRepository.GetMembershipAsync(memberId, planId, tracking: true, ct);
            if (membership == null) return Result.NotFound("Membership not found.");

            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(planId, ct);
            if (plan == null) return Result.NotFound("Plan not found.");

            // Extend from the current end date when still active, otherwise from today.
            var baseDate = membership.EndDate > DateTime.Now ? membership.EndDate : DateTime.Now;
            membership.EndDate = baseDate.AddDays(plan.DurationInDays);
            membership.UpdatedAt = DateTime.Now;

            _unitOfWork.MembershipRepository.Update(membership);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0
                ? Result.Success()
                : Result.Failure("Failed to renew the membership.");
        }

        public async Task<Result> UpdateMembershipAsync(EditMembershipViewModel model, CancellationToken ct = default)
        {
            var membership = await _unitOfWork.MembershipRepository.GetMembershipAsync(model.MemberId, model.PlanId, tracking: true, ct);
            if (membership == null) return Result.NotFound("Membership not found.");

            if (model.EndDate <= membership.CreatedAt)
                return Result.ValidationError("End date must be after the start date.");

            membership.EndDate = model.EndDate;
            membership.UpdatedAt = DateTime.Now;

            _unitOfWork.MembershipRepository.Update(membership);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0
                ? Result.Success()
                : Result.Failure("Failed to update the membership.");
        }

        public async Task<Result> RemoveMembershipAsync(int memberId, int planId, CancellationToken ct = default)
        {
            var membership = await _unitOfWork.MembershipRepository.GetMembershipAsync(memberId, planId, tracking: true, ct);
            if (membership == null) return Result.NotFound("Membership not found.");

            _unitOfWork.MembershipRepository.Delete(membership);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0
                ? Result.Success()
                : Result.Failure("Failed to delete the membership.");
        }

        public async Task<IEnumerable<MemberSelectViewModel>?> GetMembersForDropDownAsync(CancellationToken ct = default)
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);
            if (!members.Any()) return null;
            return _mapper.Map<IEnumerable<MemberSelectViewModel>>(members);
        }

        public async Task<IEnumerable<PlanSelectViewModel>?> GetPlansForDropDownAsync(CancellationToken ct = default)
        {
            var plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync(ct: ct);
            var activePlans = plans.Where(p => p.IsActive);
            if (!activePlans.Any()) return null;
            return _mapper.Map<IEnumerable<PlanSelectViewModel>>(activePlans);
        }
    }
}
