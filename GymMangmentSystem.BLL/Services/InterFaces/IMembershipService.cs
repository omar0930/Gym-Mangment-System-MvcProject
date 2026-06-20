using GymMangmentSystem.BLL.Common;
using GymMangmentSystem.BLL.ViewModels.MembershipViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMangmentSystem.BLL.Services.InterFaces
{
    public interface IMembershipService
    {
        Task<IEnumerable<MembershipViewModel>?> GetAllMembershipsAsync(CancellationToken ct = default);
        Task<MembershipViewModel?> GetMembershipDetailsAsync(int memberId, int planId, CancellationToken ct = default);
        Task<EditMembershipViewModel?> GetMembershipToEditAsync(int memberId, int planId, CancellationToken ct = default);
        Task<Result> AssignMembershipAsync(CreateMembershipViewModel model, CancellationToken ct = default);
        Task<Result> RenewMembershipAsync(int memberId, int planId, CancellationToken ct = default);
        Task<Result> UpdateMembershipAsync(EditMembershipViewModel model, CancellationToken ct = default);
        Task<Result> RemoveMembershipAsync(int memberId, int planId, CancellationToken ct = default);
        Task<IEnumerable<MemberSelectViewModel>?> GetMembersForDropDownAsync(CancellationToken ct = default);
        Task<IEnumerable<PlanSelectViewModel>?> GetPlansForDropDownAsync(CancellationToken ct = default);
    }
}
