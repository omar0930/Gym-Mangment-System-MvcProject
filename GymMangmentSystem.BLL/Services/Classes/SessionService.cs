using AutoMapper;
using GymMangmentSystem.BLL.Common;
using GymMangmentSystem.BLL.Services.InterFaces;
using GymMangmentSystem.BLL.ViewModels.SessionViewModels;
using GymMangmentSystem.DAL.Models;
using GymMangmentSystem.DAL.Repositories.Interfaces;

namespace GymMangmentSystem.BLL.Services.Classes
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SessionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<SessionViewModel>?> GetAllSessionsAsync(CancellationToken ct = default)
        {
            var sessions = await _unitOfWork.SessionRepository.GetAllSeassionsWithTrainerAndCategoryAsync(ct: ct);
            if (sessions?.Any() != true) return null;
            sessions = sessions.OrderByDescending(s => s.StartDate);
            var MappedSessions = _mapper.Map<IEnumerable<SessionViewModel>>(sessions);
            foreach (var session in MappedSessions)
            {
                session.AvailableSlots = session.Capacity - (await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(session.Id, ct));
            }
            return MappedSessions;
        }

        public Task<IEnumerable<CategorySelectViewModel>?> GetCategoriesForDropDownAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public async Task<SessionViewModel?> GetSessionByIdAsync(int SessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetSessionWithTrainerAndCategoryAsync(SessionId, ct);
            if (session == null) return null;
            var MappedSession = _mapper.Map<SessionViewModel>(session);
            MappedSession.AvailableSlots = MappedSession.Capacity - (await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(MappedSession.Id, ct));
            return MappedSession;
        }

        public async Task<UpdateSessionViewModel> GetSessionToUpdateAsync(int SessionId, CancellationToken ct = default)
        {
            var Session = await _unitOfWork.GetRepository<Session>().GetByIdAsync(SessionId);
            if (Session == null) return null;
            if (!await IsSessionAvalableForUpdateAsync(Session, ct)) return null;
            return _mapper.Map<UpdateSessionViewModel>(Session);
        }

        //helper methods
        private async Task<bool> IsSessionAvalableForUpdateAsync(Session session, CancellationToken ct = default)
        {
            if (session.StartDate <= DateTime.Now) return true;
            var Booked = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(session.Id, ct);
            return Booked > 0;
        }

        public Task<IEnumerable<TrainerSelectViewModel>?> GetTrainersForDropDownAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result> RemoveSessionAsync(int SessionId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result> UpdateSessionAsync(int id, UpdateSessionViewModel model, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
