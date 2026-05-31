using GymMangmentSystem.BLL.Services.InterFaces;
using GymMangmentSystem.BLL.ViewModels.TrainerViewModels;
using GymMangmentSystem.DAL.Data.Models;
using GymMangmentSystem.DAL.Data.Models.Enums;
using GymMangmentSystem.DAL.Repositories.Interfaces;

namespace GymMangmentSystem.BLL.Services.Classes
{
    public class TrainerService : ITrainerService
    {
        private readonly IGenericRepository<Trainer> _trainerRepository;

        public TrainerService(IGenericRepository<Trainer> trainerRepository)
        {
            _trainerRepository = trainerRepository;
        }

        public async Task<bool> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default)
        {
            var emailExist = await _trainerRepository.AnyAsync(x => x.Email == model.Email, ct);
            var phoneExist = await _trainerRepository.AnyAsync(x => x.Phone == model.Phone, ct);
            if (emailExist || phoneExist) return false;

            var trainer = new Trainer
            {
                Name = $"{model.FirstName} {model.LastName}",
                Email = model.Email,
                Phone = model.Phone,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                Specialty = model.Specialty,
                Salary = model.Salary,
                YearsOfExperience = model.YearsOfExperience,
                Address = new Address
                {
                    Street = model.Street,
                    City = model.City,
                    BuildingNumber = model.BuildingNumber,
                    State = model.State,
                    ZipCode = model.ZipCode,
                }
            };

            var result = await _trainerRepository.AddAsync(trainer);
            return result > 0;
        }

        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default)
        {
            var trainers = await _trainerRepository.GetAllAsync(ct: ct);
            if (!trainers.Any()) return [];

            return trainers.Select(t => new TrainerViewModel
            {
                Id = t.Id,
                Name = t.Name,
                Email = t.Email,
                Phone = t.Phone,
                Gender = t.Gender.ToString(),
                DateOfBirth = t.DateOfBirth.ToShortDateString(),
                Specialty = t.Specialty.ToString(),
                Salary = t.Salary,
                YearsOfExperience = t.YearsOfExperience,
            });
        }

        public async Task<TrainerViewModel?> GetTrainerDetailsAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await _trainerRepository.GetByIdAsync(trainerId, ct);
            if (trainer == null) return null;

            return new TrainerViewModel
            {
                Id = trainer.Id,
                Name = trainer.Name,
                Email = trainer.Email,
                Phone = trainer.Phone,
                Gender = trainer.Gender.ToString(),
                DateOfBirth = trainer.DateOfBirth.ToShortDateString(),
                Specialty = trainer.Specialty.ToString(),
                Salary = trainer.Salary,
                YearsOfExperience = trainer.YearsOfExperience,
                Street = trainer.Address.Street,
                City = trainer.Address.City,
                BuildingNumber = trainer.Address.BuildingNumber,
                State = trainer.Address.State,
                ZipCode = trainer.Address.ZipCode,
            };
        }

        public async Task<TrainerToUpdateViewModel?> GetTrainerToUpdateAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await _trainerRepository.GetByIdAsync(trainerId, ct);
            if (trainer == null) return null;

            return new TrainerToUpdateViewModel
            {
                Id = trainer.Id,
                Name = trainer.Name,
                Gender = trainer.Gender.ToString(),
                DateOfBirth = trainer.DateOfBirth.ToShortDateString(),
                Email = trainer.Email,
                Phone = trainer.Phone,
                Street = trainer.Address.Street,
                City = trainer.Address.City,
                BuildingNumber = trainer.Address.BuildingNumber,
                State = trainer.Address.State,
                ZipCode = trainer.Address.ZipCode,
                Specialty = trainer.Specialty,
                Salary = trainer.Salary,
                YearsOfExperience = trainer.YearsOfExperience,
            };
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

            var result = await _trainerRepository.UpdateAsync(trainer);
            return result > 0;
        }

        public async Task<bool> RemoveTrainerAsync(int id, CancellationToken ct = default)
        {
            var trainer = await _trainerRepository.GetByIdAsync(id, ct);
            if (trainer == null) return false;

            var result = await _trainerRepository.DeleteAsync(trainer);
            return result > 0;
        }
    }
}
