using GymMangmentSystem.BLL.Services.InterFaces;
using GymMangmentSystem.BLL.ViewModels.BookingViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymMangmentSystem.PL.Controllers
{
    public class SessionScheduleController : Controller
    {
        private readonly IBookingService _bookingService;

        public SessionScheduleController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var bookings = await _bookingService.GetAllBookingsAsync(ct);
            return View(bookings);
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            await PopulateDropDownsAsync(ct);
            return View("BookSession", new CreateBookingViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBookingViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDownsAsync(ct);
                return View("BookSession", model);
            }

            var result = await _bookingService.BookSessionAsync(model, ct);
            if (result.success)
            {
                TempData["SuccessMessage"] = "Session booked successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = result.ErrorMessage;
            await PopulateDropDownsAsync(ct);
            return View("BookSession", model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int memberId, int sessionId, CancellationToken ct)
        {
            var booking = await _bookingService.GetBookingDetailsAsync(memberId, sessionId, ct);
            if (booking == null)
            {
                TempData["ErrorMessage"] = "Booking not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(booking);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAttendance(int memberId, int sessionId, CancellationToken ct)
        {
            var result = await _bookingService.MarkAttendanceAsync(memberId, sessionId, ct);
            if (result.success)
                TempData["SuccessMessage"] = "Attendance updated successfully.";
            else
                TempData["ErrorMessage"] = result.ErrorMessage;

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Cancel(int memberId, int sessionId, CancellationToken ct)
        {
            var booking = await _bookingService.GetBookingDetailsAsync(memberId, sessionId, ct);
            if (booking == null)
            {
                TempData["ErrorMessage"] = "Booking not found.";
                return RedirectToAction(nameof(Index));
            }
            return View("CancelBooking", booking);
        }

        [HttpPost]
        public async Task<IActionResult> CancelConfirmed(int memberId, int sessionId, CancellationToken ct)
        {
            var result = await _bookingService.CancelBookingAsync(memberId, sessionId, ct);
            if (result.success)
                TempData["SuccessMessage"] = "Booking cancelled successfully.";
            else
                TempData["ErrorMessage"] = result.ErrorMessage;

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropDownsAsync(CancellationToken ct)
        {
            var members = await _bookingService.GetMembersForDropDownAsync(ct);
            var sessions = await _bookingService.GetSessionsForDropDownAsync(ct);

            ViewBag.Members = new SelectList(members ?? [], nameof(MemberSelectViewModel.Id), nameof(MemberSelectViewModel.Name));
            ViewBag.Sessions = new SelectList(sessions ?? [], nameof(SessionSelectViewModel.Id), nameof(SessionSelectViewModel.Display));
        }
    }
}
