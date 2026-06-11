using GymMangmentSystem.BLL.Services.InterFaces;
using GymMangmentSystem.BLL.ViewModels.PlanViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GymMangmentSystem.PL.Controllers
{
    public class PlanController : Controller
    {
        private readonly IPlanService _planService;

        public PlanController(IPlanService planService)
        {
            _planService = planService;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var plans = await _planService.GetAllPlansAsync(ct);
            return View(plans);
        }

        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var plan = await _planService.GetPlanDetailsAsync(id, ct);
            if (plan == null)
            {
                TempData["ErrorMessage"] = "Plan not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var model = await _planService.GetPlanToUpdateAsync(id, ct);
            if (model == null)
            {
                TempData["ErrorMessage"] = "Plan not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PlanToUpdateViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _planService.UpdatePlanAsync(model, ct);
            if (result)
                TempData["SuccessMessage"] = "Plan updated successfully.";
            else
                TempData["ErrorMessage"] = "Failed to update plan.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Activate(int id, CancellationToken ct)
        {
            var isActive = await _planService.ToggleActiveAsync(id, ct);
            if (isActive == null)
                TempData["ErrorMessage"] = "Plan not found.";
            else
                TempData["SuccessMessage"] = isActive.Value ? "Plan activated." : "Plan deactivated.";

            return RedirectToAction(nameof(Index));
        }
    }
}
