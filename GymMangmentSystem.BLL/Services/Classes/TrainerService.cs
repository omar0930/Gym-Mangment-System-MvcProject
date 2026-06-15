using AutoMapper;
using GymMangmentSystem.BLL.Services.InterFaces;
using GymMangmentSystem.BLL.ViewModels.TrainerViewModels;
using GymMangmentSystem.DAL.Data.Models;
using GymMangmentSystem.DAL.Repositories.Interfaces;

namespace GymMangmentSystem.BLL.Services.Classes
{
    public class TrainerService : ITrainerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private IGenericRepository<Trainer> _trainerRepository => _unitOfWork.GetRepository<Trainer>();

        public TrainerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default)
        {
            var emailExist = await _trainerRepository.AnyAsync(x => x.Email == model.Email, ct);
            var phoneExist = await _trainerRepository.AnyAsync(x => x.Phone == model.Phone, ct);
            if (emailExist || phoneExist) return false;

            var trainer = _mapper.Map<Trainer>(model);
            trainer.CreatedAt = trainer.UpdatedAt = DateTime.Now;

            _trainerRepository.Add(trainer);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0;
        }

        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default)
        {
            var trainers = await _trainerRepository.GetAllAsync(ct: ct);
            if (!trainers.Any()) return [];

            return _mapper.Map<IEnumerable<TrainerViewModel>>(trainers);
        }

        public async Task<TrainerViewModel?> GetTrainerDetailsAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await _trainerRepository.GetByIdAsync(trainerId, ct);
            if (trainer == null) return null;

            return _mapper.Map<TrainerViewModel>(trainer);
        }

        public async Task<TrainerToUpdateViewModel?> GetTrainerToUpdateAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await _trainerRepository.GetByIdAsync(trainerId, ct);
            if (trainer == null) return null;

            return _mapper.Map<TrainerToUpdateViewModel>(trainer);
        }

        public async Task<bool> UpdateTrainerAsync(int id, TrainerToUpdateViewModel model, CancellationToken ct = default)
        {
            var trainer = await _trainerRepository.GetByIdAsync(id, ct);
            if (trainer == null) return false;

            var emailTaken = await _trainerRepository.AnyAsync(x => x.Email == model.Email && x.Id != id, ct);
            var phoneTaken = await _trainerRepository.AnyAsync(x => x.Phone == model.Phone && x.Id != id, ct);
            if (emailTaken || phoneTaken) return false;

            trainer.Email = model.Email;
            trainer.Phone = model.Phone;
            trainer.Specialty = model.Specialty;
            trainer.Salary = model.Salary;
            trainer.YearsOfExperience = model.YearsOfExperience;
            trainer.Address.Street = model.Street;
            trainer.Address.City = model.City;
            trainer.Address.BuildingNumber = model.BuildingNumber;
            trainer.Address.State = model.State;
            trainer.Address.ZipCode = model.ZipCode;
            trainer.UpdatedAt = DateTime.Now;

            _trainerRepository.Update(trainer);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0;
        }

        public async Task<bool> RemoveTrainerAsync(int id, CancellationToken ct = default)
        {
            var trainer = await _trainerRepository.GetByIdAsync(id, ct);
            if (trainer == null) return false;

            _trainerRepository.Delete(trainer);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0;
        }
    }
}
