using AutoMapper;
using GymMangmentSystem.BLL.Common;
using GymMangmentSystem.BLL.Services.InterFaces;
using GymMangmentSystem.BLL.ViewModels.BookingViewModels;
using GymMangmentSystem.DAL.Data.Models;
using GymMangmentSystem.DAL.Models;
using GymMangmentSystem.DAL.Repositories.Interfaces;

namespace GymMangmentSystem.BLL.Services.Classes
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BookingViewModel>?> GetAllBookingsAsync(CancellationToken ct = default)
        {
            var bookings = await _unitOfWork.BookingRepository.GetAllBookingsWithDetailsAsync(ct: ct);
            if (bookings?.Any() != true) return null;
            bookings = bookings.OrderByDescending(b => b.Session.StartDate);
            return _mapper.Map<IEnumerable<BookingViewModel>>(bookings);
        }

        public async Task<BookingViewModel?> GetBookingDetailsAsync(int memberId, int sessionId, CancellationToken ct = default)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingWithDetailsAsync(memberId, sessionId, ct);
            if (booking == null) return null;
            return _mapper.Map<BookingViewModel>(booking);
        }

        public async Task<Result> BookSessionAsync(CreateBookingViewModel model, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(model.MemberId, ct);
            if (member == null) return Result.NotFound("Selected member was not found.");

            var session = await _unitOfWork.GetRepository<Session>().GetByIdAsync(model.SessionId, ct);
            if (session == null) return Result.NotFound("Selected session was not found.");

            if (session.StartDate <= DateTime.Now)
                return Result.ValidationError("Cannot book a session that has already started.");

            var hasActiveMembership = await _unitOfWork.MembershipRepository.AnyAsync(m => m.MemberId == model.MemberId && m.EndDate > DateTime.Now, ct);
            if (!hasActiveMembership)
                return Result.Failure("Member does not have an active membership.");

            var alreadyBooked = await _unitOfWork.BookingRepository.AnyAsync(b => b.MemberId == model.MemberId && b.SessionId == model.SessionId, ct);
            if (alreadyBooked)
                return Result.Failure("This member has already booked this session.");

            var bookedSlots = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(model.SessionId, ct);
            if (bookedSlots >= session.Capacity)
                return Result.Failure("This session is fully booked.");

            var booking = new Booking
            {
                MemberId = model.MemberId,
                SessionId = model.SessionId,
                IsAttended = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            _unitOfWork.BookingRepository.Add(booking);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0
                ? Result.Success()
                : Result.Failure("Failed to book the session.");
        }

        public async Task<Result> CancelBookingAsync(int memberId, int sessionId, CancellationToken ct = default)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingAsync(memberId, sessionId, tracking: true, ct);
            if (booking == null) return Result.NotFound("Booking not found.");

            _unitOfWork.BookingRepository.Delete(booking);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0
                ? Result.Success()
                : Result.Failure("Failed to cancel the booking.");
        }

        public async Task<Result> MarkAttendanceAsync(int memberId, int sessionId, CancellationToken ct = default)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingAsync(memberId, sessionId, tracking: true, ct);
            if (booking == null) return Result.NotFound("Booking not found.");

            booking.IsAttended = !booking.IsAttended;
            booking.UpdatedAt = DateTime.Now;

            _unitOfWork.BookingRepository.Update(booking);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0
                ? Result.Success()
                : Result.Failure("Failed to update attendance.");
        }

        public async Task<IEnumerable<MemberSelectViewModel>?> GetMembersForDropDownAsync(CancellationToken ct = default)
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);
            if (!members.Any()) return null;
            return _mapper.Map<IEnumerable<MemberSelectViewModel>>(members);
        }

        public async Task<IEnumerable<SessionSelectViewModel>?> GetSessionsForDropDownAsync(CancellationToken ct = default)
        {
            // Only upcoming sessions can be booked.
            var sessions = await _unitOfWork.SessionRepository.GetAllSeassionsWithTrainerAndCategoryAsync(s => s.StartDate > DateTime.Now, ct);
            if (sessions?.Any() != true) return null;
            return sessions
                .OrderBy(s => s.StartDate)
                .Select(s => new SessionSelectViewModel
                {
                    Id = s.Id,
                    Display = $"{s.Category.CategoryName} - {s.StartDate:MMM dd, yyyy hh:mm tt}",
                });
        }
    }
}
