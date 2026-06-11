using GymMangmentSystem.BLL.Common;
using GymMangmentSystem.BLL.Services.InterFaces;
using GymMangmentSystem.BLL.ViewModels.SessionViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymMangmentSystem.PL.Controllers
{
    public class SessionController : Controller
    {
        private readonly ISessionService _sessionService;

        public SessionController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var sessions = await _sessionService.GetAllSessionsAsync(ct);
            return View(sessions);
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            await PopulateDropDownsAsync(ct);
            return View("CreateSession", new CreateSessionViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSessionViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDownsAsync(ct);
                return View("CreateSession", model);
            }

            var result = await _sessionService.CreateSessionAsync(model, ct);
            if (result.success)
            {
                TempData["SuccessMessage"] = "Session created successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = result.ErrorMessage;
            await PopulateDropDownsAsync(ct);
            return View("CreateSession", model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var session = await _sessionService.GetSessionByIdAsync(id, ct);
            if (session == null)
            {
                TempData["ErrorMessage"] = "Session not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(session);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var session = await _sessionService.GetSessionToUpdateAsync(id, ct);
            if (session == null)
            {
                TempData["ErrorMessage"] = "Session not found or can no longer be edited.";
                return RedirectToAction(nameof(Index));
            }
            await PopulateDropDownsAsync(ct);
            return View("EditSession", session);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateSessionViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDownsAsync(ct);
                return View("EditSession", model);
            }

            var result = await _sessionService.UpdateSessionAsync(model.Id, model, ct);
            if (result.success)
            {
                TempData["SuccessMessage"] = "Session updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = result.ErrorMessage;
            await PopulateDropDownsAsync(ct);
            return View("EditSession", model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var session = await _sessionService.GetSessionByIdAsync(id, ct);
            if (session == null)
            {
                TempData["ErrorMessage"] = "Session not found.";
                return RedirectToAction(nameof(Index));
            }
            return View("DeleteSession", session);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var result = await _sessionService.RemoveSessionAsync(id, ct);
            if (result.success)
                TempData["SuccessMessage"] = "Session deleted successfully.";
            else
                TempData["ErrorMessage"] = result.ErrorMessage;

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropDownsAsync(CancellationToken ct)
        {
            var categories = await _sessionService.GetCategoriesForDropDownAsync(ct);
            var trainers = await _sessionService.GetTrainersForDropDownAsync(ct);

            ViewBag.Categories = new SelectList(categories ?? [], nameof(CategorySelectViewModel.Id), nameof(CategorySelectViewModel.CategoryName));
            ViewBag.Trainers = new SelectList(trainers ?? [], nameof(TrainerSelectViewModel.Id), nameof(TrainerSelectViewModel.Name));
        }
    }
}
