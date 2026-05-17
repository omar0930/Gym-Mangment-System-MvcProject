using GymMangmentSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using MvcProjectG03.DbContexts;
using MvcProjectG03.Models;

namespace GymMangmentSystem.DAL.Repositories.Classes
{
    public class PlanRepository : IPlanRepository
    {
        private readonly GymDbContext _dbcontext;
        public PlanRepository(GymDbContext _dbcontext)
        {
            this._dbcontext = _dbcontext;
        }
        public async Task<int> AddAsync(Plan plan)
        {
            _dbcontext.Plans.Add(plan);
            return await _dbcontext.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(Plan plan)
        {
            _dbcontext.Plans.Remove(plan);
            return await _dbcontext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Plan>> GetAllPlansAsync(bool tracking = false, CancellationToken ct = default)
        {
            IQueryable<Plan> query = tracking ? _dbcontext.Plans : _dbcontext.Plans.AsNoTracking();
            return await query.ToListAsync(ct);
        }

        public async Task<Plan?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _dbcontext.Plans.FindAsync(id, ct);
        }

        public async Task<int> UpdateAsync(Plan plan)
        {
            _dbcontext.Plans.Update(plan);
            return await _dbcontext.SaveChangesAsync();
        }
    }
}
