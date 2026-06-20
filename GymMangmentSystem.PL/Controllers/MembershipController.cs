using GymMangmentSystem.BLL.Services.InterFaces;
using GymMangmentSystem.BLL.ViewModels.MembershipViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymMangmentSystem.PL.Controllers
{
    public class MembershipController : Controller
    {
        private readonly IMembershipService _membershipService;

        public MembershipController(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var memberships = await _membershipService.GetAllMembershipsAsync(ct);
            return View(memberships);
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            await PopulateDropDownsAsync(ct);
            return View("CreateMembership", new CreateMembershipViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateMembershipViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDownsAsync(ct);
                return View("CreateMembership", model);
            }

            var result = await _membershipService.AssignMembershipAsync(model, ct);
            if (result.success)
            {
                TempData["SuccessMessage"] = "Membership assigned successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = result.ErrorMessage;
            await PopulateDropDownsAsync(ct);
            return View("CreateMembership", model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int memberId, int planId, CancellationToken ct)
        {
            var membership = await _membershipService.GetMembershipDetailsAsync(memberId, planId, ct);
            if (membership == null)
            {
                TempData["ErrorMessage"] = "Membership not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(membership);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int memberId, int planId, CancellationToken ct)
        {
            var membership = await _membershipService.GetMembershipToEditAsync(memberId, planId, ct);
            if (membership == null)
            {
                TempData["ErrorMessage"] = "Membership not found.";
                return RedirectToAction(nameof(Index));
            }
            return View("EditMembership", membership);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditMembershipViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View("EditMembership", model);

            var result = await _membershipService.UpdateMembershipAsync(model, ct);
            if (result.success)
            {
                TempData["SuccessMessage"] = "Membership updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = result.ErrorMessage;
            return View("EditMembership", model);
        }

        [HttpPost]
        public async Task<IActionResult> Renew(int memberId, int planId, CancellationToken ct)
        {
            var result = await _membershipService.RenewMembershipAsync(memberId, planId, ct);
            if (result.success)
                TempData["SuccessMessage"] = "Membership renewed successfully.";
            else
                TempData["ErrorMessage"] = result.ErrorMessage;

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int memberId, int planId, CancellationToken ct)
        {
            var membership = await _membershipService.GetMembershipDetailsAsync(memberId, planId, ct);
            if (membership == null)
            {
                TempData["ErrorMessage"] = "Membership not found.";
                return RedirectToAction(nameof(Index));
            }
            return View("DeleteMembership", membership);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int memberId, int planId, CancellationToken ct)
        {
            var result = await _membershipService.RemoveMembershipAsync(memberId, planId, ct);
            if (result.success)
                TempData["SuccessMessage"] = "Membership deleted successfully.";
            else
                TempData["ErrorMessage"] = result.ErrorMessage;

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropDownsAsync(CancellationToken ct)
        {
            var members = await _membershipService.GetMembersForDropDownAsync(ct);
            var plans = await _membershipService.GetPlansForDropDownAsync(ct);

            ViewBag.Members = new SelectList(members ?? [], nameof(MemberSelectViewModel.Id), nameof(MemberSelectViewModel.Name));
            ViewBag.Plans = new SelectList(plans ?? [], nameof(PlanSelectViewModel.Id), nameof(PlanSelectViewModel.Name));
        }
    }
}
