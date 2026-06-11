using GymMangmentSystem.BLL.Services.InterFaces;
using GymMangmentSystem.BLL.ViewModels.HomeViewModels;
using GymMangmentSystem.DAL.Data.Models;
using GymMangmentSystem.DAL.Models;
using GymMangmentSystem.DAL.Repositories.Interfaces;

namespace GymMangmentSystem.BLL.Services.Classes
{
    /// <summary>
    /// Computes the home-page KPIs on the fly. Every number is a fresh,
    /// cross-module aggregation (Members, Trainers, Sessions) — nothing is stored.
    /// </summary>
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AnalyticsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DashboardViewModel> GetDashboardAsync(CancellationToken ct = default)
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);
            var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct: ct);
            var sessions = await _unitOfWork.GetRepository<Session>().GetAllAsync(ct: ct);
            var memberships = await _unitOfWork.GetRepository<Membership>().GetAllAsync(ct: ct);

            var now = DateTime.Now;

            return new DashboardViewModel
            {
                TotalMembers = members.Count(),
                // Active = members with at least one non-expired membership.
                ActiveMembers = memberships
                    .Where(m => m.EndDate > now)
                    .Select(m => m.MemberId)
                    .Distinct()
                    .Count(),
                Trainers = trainers.Count(),
                UpcomingSessions = sessions.Count(s => s.StartDate > now),
                OngoingSessions = sessions.Count(s => s.StartDate <= now && s.EndDate >= now),
                CompletedSessions = sessions.Count(s => s.EndDate < now),
            };
        }
    }
}
