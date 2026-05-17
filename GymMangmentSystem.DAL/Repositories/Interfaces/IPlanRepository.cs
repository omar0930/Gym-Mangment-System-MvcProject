using MvcProjectG03.Models;

namespace GymMangmentSystem.DAL.Repositories.Interfaces
{
    public interface IPlanRepository
    {
        Task<IEnumerable<Plan>> GetAllPlansAsync(bool tracking = false, CancellationToken ct = default);
        Task<Plan?> GetByIdAsync(int id, CancellationToken ct = default);
        public Task<int> AddAsync(Plan plan);
        public Task<int> UpdateAsync(Plan plan);
        public Task<int> DeleteAsync(Plan plan);
    }
}
