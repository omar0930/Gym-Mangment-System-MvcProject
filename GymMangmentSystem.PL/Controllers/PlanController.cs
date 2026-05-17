using GymMangmentSystem.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MvcProjectG03.Controllers
{
    public class PlanController : Controller
    {
        private readonly IPlanRepository planRepository;

        public PlanController(IPlanRepository repository)
        {
            this.planRepository = repository;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var Plans = await planRepository.GetAllPlansAsync(ct: ct);
            return View(Plans);
        }
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var plan = await planRepository.GetByIdAsync(id, ct);
            if (plan == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(plan);

        }
    }
}
