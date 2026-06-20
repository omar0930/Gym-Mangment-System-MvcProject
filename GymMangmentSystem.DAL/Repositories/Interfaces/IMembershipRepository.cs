using GymMangmentSystem.DAL.Models;
using System.Linq.Expressions;

namespace GymMangmentSystem.DAL.Repositories.Interfaces
{
    public interface IMembershipRepository : IGenericRepository<Membership>
    {
        Task<IEnumerable<Membership>> GetAllMembershipsWithMemberAndPlanAsync(Expression<Func<Membership, bool>>? predicate = null, CancellationToken ct = default);
        Task<Membership?> GetMembershipWithMemberAndPlanAsync(int memberId, int planId, CancellationToken ct = default);
        Task<Membership?> GetMembershipAsync(int memberId, int planId, bool tracking = false, CancellationToken ct = default);
    }
}
