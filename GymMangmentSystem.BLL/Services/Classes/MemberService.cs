using AutoMapper;
using GymMangmentSystem.BLL.Common;
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

        public async Task<Result> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            if (await _memberRepository.AnyAsync(x => x.Email == model.Email, ct))
                return Result.Failure("A member with this email already exists.");
            if (await _memberRepository.AnyAsync(x => x.Phone == model.Phone, ct))
                return Result.Failure("A member with this phone number already exists.");

            var photoPath = await _attachmentService.UploadAsync(
                model.Photo.OpenReadStream(), model.Photo.FileName, MembersPhotoFolder, ct);
            // Reject when the photo fails validation (type/size) or saving
            if (string.IsNullOrEmpty(photoPath))
                return Result.ValidationError("Photo upload failed. Allowed types are jpg/jpeg/png up to 5 MB.");

            var member = _mapper.Map<Member>(model);
            member.Photo = photoPath;
            member.CreatedAt = member.UpdatedAt = DateTime.Now;

            _memberRepository.Add(member);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            if (result > 0) return Result.Success();

            // Persisting the row failed — don't leave the uploaded file orphaned
            _attachmentService.Delete(Path.GetFileName(photoPath), MembersPhotoFolder);
            return Result.Failure("Failed to create the member.");
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

        public async Task<Result> UpdateMemberDetailsAsync(int id, MemberToUpdateViewModel model, CancellationToken ct = default)
        {
            var member = await _memberRepository.GetByIdAsync(id, ct);
            if (member == null) return Result.NotFound("Member not found.");

            // Self-exclusion: the email/phone must not belong to a different member.
            if (await _memberRepository.AnyAsync(x => x.Email == model.Email && x.Id != id, ct))
                return Result.Failure("Another member is already using this email.");
            if (await _memberRepository.AnyAsync(x => x.Phone == model.Phone && x.Id != id, ct))
                return Result.Failure("Another member is already using this phone number.");

            member.Email = model.Email;
            member.Phone = model.Phone;
            member.Address.BuildingNumber = model.BuildingNumber;
            member.Address.City = model.City;
            member.Address.Street = model.Street;
            member.UpdatedAt = DateTime.Now;

            _memberRepository.Update(member);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Success() : Result.Failure("Failed to update the member.");
        }

        public async Task<Result> RemoveMemberAsync(int id, CancellationToken ct = default)
        {
            var member = await _memberRepository.GetByIdAsync(id, ct);
            if (member == null) return Result.NotFound("Member not found.");

            var hasUpcomingSessions = await _unitOfWork.GetRepository<Booking>()
                .AnyAsync(b => b.MemberId == id && b.Session.StartDate > DateTime.Now, ct);
            if (hasUpcomingSessions)
                return Result.Failure("Cannot delete a member with upcoming booked sessions.");

            _memberRepository.Delete(member);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            if (result == 0) return Result.Failure("Failed to delete the member.");

            // Remove the stored photo only after the row is gone
            if (!string.IsNullOrEmpty(member.Photo))
                _attachmentService.Delete(Path.GetFileName(member.Photo), MembersPhotoFolder);

            return Result.Success();
        }
    }
}
