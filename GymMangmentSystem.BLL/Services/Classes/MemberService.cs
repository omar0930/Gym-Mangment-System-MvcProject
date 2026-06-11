using AutoMapper;
using GymMangmentSystem.BLL.Services.AttachmentService;
using GymMangmentSystem.BLL.Services.InterFaces;
using GymMangmentSystem.BLL.ViewModels.MemberViewModels;
using GymMangmentSystem.DAL.Data.Models;
using GymMangmentSystem.DAL.Models;
using GymMangmentSystem.DAL.Repositories.Interfaces;

namespace GymMangmentSystem.BLL.Services.Classes
{
    public class MemberService : IMemberService
    {
        private const string MembersPhotoFolder = "images/MembersPictures";

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAttachmentService _attachmentService;
        private IGenericRepository<Member> _memberRepository => _unitOfWork.GetRepository<Member>();
        private IGenericRepository<Membership> _membershipRepository => _unitOfWork.GetRepository<Membership>();

        public MemberService(IUnitOfWork unitOfWork, IMapper mapper, IAttachmentService attachmentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _attachmentService = attachmentService;
        }

        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            var emailExist = await _memberRepository.AnyAsync(x => x.Email == model.Email, ct);
            var phoneExist = await _memberRepository.AnyAsync(x => x.Phone == model.Phone, ct);
            // Email or phone already in use
            if (emailExist || phoneExist) return false;

            var photoPath = await _attachmentService.UploadAsync(
                model.Photo.OpenReadStream(), model.Photo.FileName, MembersPhotoFolder, ct);
            // Reject when the photo fails validation (type/size) or saving
            if (string.IsNullOrEmpty(photoPath)) return false;

            var member = _mapper.Map<Member>(model);
            member.Photo = photoPath;
            member.CreatedAt = member.UpdatedAt = DateTime.Now;

            var result = await _memberRepository.AddAsync(member);
            return result > 0;
        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var members = await _memberRepository.GetAllAsync(ct: ct);
            if (!members.Any()) return [];
            return _mapper.Map<IEnumerable<MemberViewModel>>(members);
        }

        public async Task<MemberViewModel?> GetMemberDetailsAsync(int memberid, CancellationToken ct = default)
        {
            var member = await _memberRepository.GetByIdAsync(memberid, ct);
            if (member == null) return null;

            var viewModel = _mapper.Map<MemberViewModel>(member);

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

            return _mapper.Map<HealthRecordViewModel>(member.HealthRecord);
        }

        public async Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int memberid, CancellationToken ct = default)
        {
            var member = await _memberRepository.GetByIdAsync(memberid, ct);
            if (member == null) return null;

            return _mapper.Map<MemberToUpdateViewModel>(member);
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

            // Remove the stored photo only after the row is gone
            if (result > 0 && !string.IsNullOrEmpty(member.Photo))
                _attachmentService.Delete(Path.GetFileName(member.Photo), MembersPhotoFolder);

            return result > 0;
        }
    }
}
