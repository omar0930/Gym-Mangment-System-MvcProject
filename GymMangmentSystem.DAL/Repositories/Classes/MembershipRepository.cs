using GymMangmentSystem.DAL.DbContexts;
using GymMangmentSystem.DAL.Models;
using GymMangmentSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GymMangmentSystem.DAL.Repositories.Classes
{
    public class MembershipRepository : GenericRepository<Membership>, IMembershipRepository
    {
        private readonly GymDbContext _dbContext;
        public MembershipRepository(GymDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Membership>> GetAllMembershipsWithMemberAndPlanAsync(Expression<Func<Membership, bool>>? predicate = null, CancellationToken ct = default)
        {
            IQueryable<Membership> query = _dbContext.Memberships.AsNoTracking()
                .Include(m => m.Member)
                .Include(m => m.Plan);
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            return await query.ToListAsync(ct);
        }

        public Task<Membership?> GetMembershipWithMemberAndPlanAsync(int memberId, int planId, CancellationToken ct = default)
            => _dbContext.Memberships.AsNoTracking()
                .Include(m => m.Member)
                .Include(m => m.Plan)
                .FirstOrDefaultAsync(m => m.MemberId == memberId && m.PlanId == planId, ct);

        // Composite-key lookup — the generic GetByIdAsync(int) can't be used here.
        public Task<Membership?> GetMembershipAsync(int memberId, int planId, bool tracking = false, CancellationToken ct = default)
        {
            IQueryable<Membership> query = tracking ? _dbContext.Memberships : _dbContext.Memberships.AsNoTracking();
            return query.FirstOrDefaultAsync(m => m.MemberId == memberId && m.PlanId == planId, ct);
        }
    }
}
