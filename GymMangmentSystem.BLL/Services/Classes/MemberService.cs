using GymMangmentSystem.BLL.Services.InterFaces;
using GymMangmentSystem.BLL.ViewModels.MemberViewModels;
using GymMangmentSystem.DAL.Data.Models;
using GymMangmentSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMangmentSystem.BLL.Services.Classes
{
    public class MemberService : IMemberService
    {
        private readonly IGenericRepository<Member> _memberRepository;
        public MemberService(IGenericRepository<Member> MemberRepository)
        {
            _memberRepository = MemberRepository;
        }
        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            var emailExists = await _memberRepository.AnyAsync(x => x.Email == model.Email, ct);
            var phoneExists = await _memberRepository.AnyAsync(x => x.PhoneNumber == model.PhoneNumber, ct);
            if (emailExists || phoneExists) return false;
            var Member = new Member()
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                Gender = model.Gender,
                Address = new Address()
                {
                    Street = model.Street,
                    City = model.City,
                    State = model.State,
                    ZipCode = model.ZipCode
                },

            }
            var Result = await _memberRepository.AddAsync(Member, ct);
            return Result > 0;
        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var Members = await _memberRepository.GetAllAsync(ct: ct);
            if (!Members.Any()) return [];
            var MemberViewModels = Members.Select(m => new MemberViewModel
            {
                Id = m.Id,
                Name = m.Name,
                Email = m.Email,
                Phone = m.Phone,
                Photo = m.Photo,
            }).ToList();
            return MemberViewModels;
        }

        public async Task<MemberViewModel?> GetMemberDetailsAsync(int memberid, CancellationToken ct = default)
        {
            var Member = await _memberRepository.GetByIdAsync(memberid, ct);
            if (Member == null) return null;
            var ViewModel = new MemberViewModel
            {

                Name = Member.Name,
                Email = Member.Email,
                Phone = Member.Phone,
                DateOfBirth = Member.DateOfBirth.ToShortDateString(),
                Gender = Member.Gender.ToString(),
                Address = $"{Member.Address.Street}, {Member.Address.City},{Member.Address.BuildingNumber}",
            };
            var ActiveMembership = await _memberRepository.FirstOrDefaultAsync(m => m.MemberId == memberid && m.EndDate > DateTime.Now, ct: ct);
            return ViewModel;
        }

        public Task<bool> RemoveMemberAsync(int id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        Task<HealthRecordViewModel?> IMemberService.GetMemberHealthRecordAsync(int memberid, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        Task<MemberToUpdateViewModel?> IMemberService.GetMemberToUpdateAsync(int memberid, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        Task<bool> IMemberService.UpdateMemberDetailsAsync(int id, MemberToUpdateViewModel model, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
