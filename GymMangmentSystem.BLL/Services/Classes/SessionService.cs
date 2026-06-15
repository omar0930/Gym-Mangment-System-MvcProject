using AutoMapper;
using GymMangmentSystem.BLL.Common;
using GymMangmentSystem.BLL.Services.InterFaces;
using GymMangmentSystem.BLL.ViewModels.SessionViewModels;
using GymMangmentSystem.DAL.Data.Models;
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

        public async Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct = default)
        {
            if (model.EndDate <= model.StartDate)
                return Result.ValidationError("End date must be after the start date.");

            var trainerExists = await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Id == model.TrainerId, ct);
            if (!trainerExists) return Result.NotFound("Selected trainer was not found.");

            var categoryExists = await _unitOfWork.GetRepository<Category>().AnyAsync(c => c.Id == model.CategoryId, ct);
            if (!categoryExists) return Result.NotFound("Selected category was not found.");

            var session = _mapper.Map<Session>(model);
            session.CreatedAt = session.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Session>().Add(session);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0
                ? Result.Success()
                : Result.Failure("Failed to create the session.");
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

        public async Task<IEnumerable<CategorySelectViewModel>?> GetCategoriesForDropDownAsync(CancellationToken ct = default)
        {
            var categories = await _unitOfWork.GetRepository<Category>().GetAllAsync(ct: ct);
            if (!categories.Any()) return null;
            return _mapper.Map<IEnumerable<CategorySelectViewModel>>(categories);
        }

        public async Task<SessionViewModel?> GetSessionByIdAsync(int SessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetSessionWithTrainerAndCategoryAsync(SessionId, ct);
            if (session == null) return null;
            var MappedSession = _mapper.Map<SessionViewModel>(session);
            MappedSession.AvailableSlots = MappedSession.Capacity - (await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(MappedSession.Id, ct));
            return MappedSession;
        }

        public async Task<UpdateSessionViewModel?> GetSessionToUpdateAsync(int SessionId, CancellationToken ct = default)
        {
            var Session = await _unitOfWork.GetRepository<Session>().GetByIdAsync(SessionId, ct);
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

        public async Task<IEnumerable<TrainerSelectViewModel>?> GetTrainersForDropDownAsync(CancellationToken ct = default)
        {
            var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct: ct);
            if (!trainers.Any()) return null;
            return _mapper.Map<IEnumerable<TrainerSelectViewModel>>(trainers);
        }

        public async Task<Result> RemoveSessionAsync(int SessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.GetRepository<Session>().GetByIdAsync(SessionId, ct);
            if (session == null) return Result.NotFound("Session not found.");

            _unitOfWork.GetRepository<Session>().Delete(session);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0
                ? Result.Success()
                : Result.Failure("Failed to delete the session.");
        }

        public async Task<Result> UpdateSessionAsync(int id, UpdateSessionViewModel model, CancellationToken ct = default)
        {
            if (model.EndDate <= model.StartDate)
                return Result.ValidationError("End date must be after the start date.");

            var session = await _unitOfWork.GetRepository<Session>().GetByIdAsync(id, ct);
            if (session == null) return Result.NotFound("Session not found.");

            var trainerExists = await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Id == model.TrainerId, ct);
            if (!trainerExists) return Result.NotFound("Selected trainer was not found.");

            session.Descreption = model.Description;
            session.StartDate = model.StartDate;
            session.EndDate = model.EndDate;
            session.TrainerId = model.TrainerId;
            session.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Session>().Update(session);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0
                ? Result.Success()
                : Result.Failure("Failed to update the session.");
        }
    }
}
