using GymMangmentSystem.BLL.Common;
using GymMangmentSystem.BLL.ViewModels.BookingViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMangmentSystem.BLL.Services.InterFaces
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingViewModel>?> GetAllBookingsAsync(CancellationToken ct = default);
        Task<BookingViewModel?> GetBookingDetailsAsync(int memberId, int sessionId, CancellationToken ct = default);
        Task<Result> BookSessionAsync(CreateBookingViewModel model, CancellationToken ct = default);
        Task<Result> CancelBookingAsync(int memberId, int sessionId, CancellationToken ct = default);
        Task<Result> MarkAttendanceAsync(int memberId, int sessionId, CancellationToken ct = default);
        Task<IEnumerable<MemberSelectViewModel>?> GetMembersForDropDownAsync(CancellationToken ct = default);
        Task<IEnumerable<SessionSelectViewModel>?> GetSessionsForDropDownAsync(CancellationToken ct = default);
    }
}
