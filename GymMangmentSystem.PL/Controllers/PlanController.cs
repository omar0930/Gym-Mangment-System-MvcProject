using GymMangmentSystem.BLL.ViewModels.PlanViewModels;
using GymMangmentSystem.DAL.Models;
using GymMangmentSystem.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GymMangmentSystem.PL.Controllers
{
    public class PlanController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private IGenericRepository<Plan> planRepository => _unitOfWork.GetRepository<Plan>();

        public PlanController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var plans = await planRepository.GetAllAsync(ct: ct);
            return View(plans);
        }

        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var plan = await planRepository.GetByIdAsync(id, ct);
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
            var plan = await planRepository.GetByIdAsync(id, ct);
            if (plan == null)
            {
                TempData["ErrorMessage"] = "Plan not found.";
                return RedirectToAction(nameof(Index));
            }

            var model = new PlanToUpdateViewModel
            {
                Id = plan.Id,
                PlanName = plan.Name,
                DurationDays = plan.DurationInDays,
                Price = plan.Price,
                Description = plan.Description,
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PlanToUpdateViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var plan = await planRepository.GetByIdAsync(model.Id, ct);
            if (plan == null)
            {
                TempData["ErrorMessage"] = "Plan not found.";
                return RedirectToAction(nameof(Index));
            }

            plan.DurationInDays = model.DurationDays;
            plan.Price = model.Price;
            plan.Description = model.Description;
            plan.UpdatedAt = DateTime.Now;

            var result = await planRepository.UpdateAsync(plan);
            if (result > 0)
                TempData["SuccessMessage"] = "Plan updated successfully.";
            else
                TempData["ErrorMessage"] = "Failed to update plan.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Activate(int id, CancellationToken ct)
        {
            var plan = await planRepository.GetByIdAsync(id, ct);
            if (plan == null)
            {
                TempData["ErrorMessage"] = "Plan not found.";
                return RedirectToAction(nameof(Index));
            }

            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.Now;
            await planRepository.UpdateAsync(plan);

            TempData["SuccessMessage"] = plan.IsActive ? "Plan activated." : "Plan deactivated.";
            return RedirectToAction(nameof(Index));
        }
    }
}
