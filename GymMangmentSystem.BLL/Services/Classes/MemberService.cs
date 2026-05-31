using GymMangmentSystem.BLL.Services.InterFaces;
using GymMangmentSystem.BLL.ViewModels.MemberViewModels;
using GymMangmentSystem.DAL.Data.Models;
using GymMangmentSystem.DAL.Models;
using GymMangmentSystem.DAL.Repositories.Interfaces;

namespace GymMangmentSystem.BLL.Services.Classes
{
    public class MemberService : IMemberService
    {
        private readonly IGenericRepository<Member> _memberRepository;
        private readonly IGenericRepository<Membership> _membershipRepository;

        public MemberService(
            IGenericRepository<Member> memberRepository,
            IGenericRepository<Membership> membershipRepository)
        {
            _memberRepository = memberRepository;
            _membershipRepository = membershipRepository;
        }

        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            var emailExist = await _memberRepository.AnyAsync(x => x.Email == model.Email, ct);
            var phoneExist = await _memberRepository.AnyAsync(x => x.Phone == model.Phone, ct);
            //Email or phone exists return false
            if (emailExist || phoneExist) return false;
            //return true add member to database
            var member = new Member()
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                Address = new Address()
                {
                    BuildingNumber = model.BuildingNumber,
                    City = model.City,
                    Street = model.Street,
                },
                HealthRecord = new HealthRecord()
                {
                    BloodType = model.HealthRecordViewModel.BloodType,
                    Height = model.HealthRecordViewModel.Height,
                    Note = model.HealthRecordViewModel.Note,
                    Weight = model.HealthRecordViewModel.Weight,
                }
            };

            var result = await _memberRepository.AddAsync(member);
            return result > 0;
        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var members = await _memberRepository.GetAllAsync(ct: ct);
            if (!members.Any()) return [];
            return members.Select(m => new MemberViewModel
            {
                Id = m.Id,
                Name = m.Name,
                Email = m.Email,
                Phone = m.Phone,
                Gender = m.Gender.ToString(),
                Photo = m.Photo,
            });
        }

        public async Task<MemberViewModel?> GetMemberDetailsAsync(int memberid, CancellationToken ct = default)
        {
            var member = await _memberRepository.GetByIdAsync(memberid, ct);
            if (member == null) return null;

            var viewModel = new MemberViewModel
            {
                Id = member.Id,
                Name = member.Name,
                Photo = member.Photo,
                Email = member.Email,
                Phone = member.Phone,
                DateOfBirth = member.DateOfBirth.ToShortDateString(),
                Gender = member.Gender.ToString(),
                Address = $"{member.Address.Street}, {member.Address.City}, {member.Address.BuildingNumber}",
            };

            var activeMembership = await _membershipRepository.FirstOrDefaultAsync(
                m => m.MemberId == memberid && m.EndDate > DateTime.Now,
                ct: ct,
                includes: m => m.Plan);

            if (activeMembership != null)
            {
                viewModel.PlanName = activeMembership.Plan.Name;
                viewModel.MembershipStartDate = activeMembership.CreatedAt.ToShortDateString();
                viewModel.MembershipEndDate = activeMembership.EndDate.ToShortDateString();
            }

            return viewModel;
        }

        public async Task<HealthRecordViewModel?> GetMemberHealthRecordAsync(int memberid, CancellationToken ct = default)
        {
            var member = await _memberRepository.GetByIdAsync(memberid, ct, m => m.HealthRecord);
            if (member?.HealthRecord == null) return null;

            return new HealthRecordViewModel
            {
                Height = member.HealthRecord.Height,
                Weight = member.HealthRecord.Weight,
                BloodType = member.HealthRecord.BloodType,
                Note = member.HealthRecord.Note,
            };
        }

        public async Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int memberid, CancellationToken ct = default)
        {
            var member = await _memberRepository.GetByIdAsync(memberid, ct);
            if (member == null) return null;

            return new MemberToUpdateViewModel
            {
                Id = member.Id,
                Name = member.Name,
                Photo = member.Photo,
                Email = member.Email,
                Phone = member.Phone,
                BuildingNumber = member.Address.BuildingNumber,
                City = member.Address.City,
                Street = member.Address.Street,
            };
        }

        public async Task<bool> UpdateMemberDetailsAsync(int id, MemberToUpdateViewModel model, CancellationToken ct = default)
        {
            var member = await _memberRepository.GetByIdAsync(id, ct);
            if (member == null) return false;

            var emailTaken = await _memberRepository.AnyAsync(x => x.Email == model.Email && x.Id != id, ct);
            var phoneTaken = await _memberRepository.AnyAsync(x => x.Phone == model.Phone && x.Id != id, ct);
            if (emailTaken || phoneTaken) return false;

            member.Email = model.Email;
            member.Phone = model.Phone;
            member.Address.BuildingNumber = model.BuildingNumber;
            member.Address.City = model.City;
            member.Address.Street = model.Street;
            member.UpdatedAt = DateTime.Now;

            var result = await _memberRepository.UpdateAsync(member);
            return result > 0;
        }

        public async Task<bool> RemoveMemberAsync(int id, CancellationToken ct = default)
        {
            var member = await _memberRepository.GetByIdAsync(id, ct);
            if (member == null) return false;

            var result = await _memberRepository.DeleteAsync(member);
            return result > 0;
        }
    }
}
