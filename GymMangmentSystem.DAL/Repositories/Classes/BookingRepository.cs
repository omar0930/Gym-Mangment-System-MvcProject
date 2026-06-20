using GymMangmentSystem.DAL.DbContexts;
using GymMangmentSystem.DAL.Models;
using GymMangmentSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GymMangmentSystem.DAL.Repositories.Classes
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        private readonly GymDbContext _dbContext;
        public BookingRepository(GymDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Booking>> GetAllBookingsWithDetailsAsync(Expression<Func<Booking, bool>>? predicate = null, CancellationToken ct = default)
        {
            IQueryable<Booking> query = _dbContext.Bookings.AsNoTracking()
                .Include(b => b.Member)
                .Include(b => b.Session).ThenInclude(s => s.Trainer)
                .Include(b => b.Session).ThenInclude(s => s.Category);
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            return await query.ToListAsync(ct);
        }

        public Task<Booking?> GetBookingWithDetailsAsync(int memberId, int sessionId, CancellationToken ct = default)
            => _dbContext.Bookings.AsNoTracking()
                .Include(b => b.Member)
                .Include(b => b.Session).ThenInclude(s => s.Trainer)
                .Include(b => b.Session).ThenInclude(s => s.Category)
                .FirstOrDefaultAsync(b => b.MemberId == memberId && b.SessionId == sessionId, ct);

        // Composite-key lookup — the generic GetByIdAsync(int) can't be used here.
        public Task<Booking?> GetBookingAsync(int memberId, int sessionId, bool tracking = false, CancellationToken ct = default)
        {
            IQueryable<Booking> query = tracking ? _dbContext.Bookings : _dbContext.Bookings.AsNoTracking();
            return query.FirstOrDefaultAsync(b => b.MemberId == memberId && b.SessionId == sessionId, ct);
        }
    }
}
