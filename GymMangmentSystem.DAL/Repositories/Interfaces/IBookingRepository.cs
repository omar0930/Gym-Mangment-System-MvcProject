using GymMangmentSystem.DAL.Models;
using System.Linq.Expressions;

namespace GymMangmentSystem.DAL.Repositories.Interfaces
{
    public interface IBookingRepository : IGenericRepository<Booking>
    {
        Task<IEnumerable<Booking>> GetAllBookingsWithDetailsAsync(Expression<Func<Booking, bool>>? predicate = null, CancellationToken ct = default);
        Task<Booking?> GetBookingWithDetailsAsync(int memberId, int sessionId, CancellationToken ct = default);
        Task<Booking?> GetBookingAsync(int memberId, int sessionId, bool tracking = false, CancellationToken ct = default);
    }
}
