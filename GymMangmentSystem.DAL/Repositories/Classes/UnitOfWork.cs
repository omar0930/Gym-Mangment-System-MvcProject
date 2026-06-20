using GymMangmentSystem.DAL.Data.Models;
using GymMangmentSystem.DAL.DbContexts;
using GymMangmentSystem.DAL.Repositories.Classes;
using GymMangmentSystem.DAL.Repositories.Interfaces;

public class UnitOfWork : IUnitOfWork
{
    private readonly Dictionary<string, object> _repositories = [];
    private readonly GymDbContext _dbContext;

    public UnitOfWork(GymDbContext dbContext, ISessionRepository sessionRepository, IMembershipRepository membershipRepository, IBookingRepository bookingRepository)
    {
        _dbContext = dbContext;
        SessionRepository = sessionRepository;
        MembershipRepository = membershipRepository;
        BookingRepository = bookingRepository;
    }

    public ISessionRepository SessionRepository { get; }
    public IMembershipRepository MembershipRepository { get; }
    public IBookingRepository BookingRepository { get; }

    public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new()
    {
        var TypeName = typeof(TEntity).Name;
        if (_repositories.TryGetValue(TypeName, out object? Value))
        {
            return (IGenericRepository<TEntity>)Value;
        }
        var repository = new GenericRepository<TEntity>(_dbContext);
        _repositories[TypeName] = repository;
        return repository;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _dbContext.SaveChangesAsync(ct);
}