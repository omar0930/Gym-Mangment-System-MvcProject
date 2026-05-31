using GymMangmentSystem.DAL.Models;
using GymMangmentSystem.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GymMangmentSystem.PL.Controllers
{
    public class PlanController : Controller
    {
        private readonly IGenericRepository<Plan> planRepository;

        public PlanController(IGenericRepository<Plan> repository)
        {
            this.planRepository = repository;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var Plans = await planRepository.GetAllAsync(ct: ct);
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
