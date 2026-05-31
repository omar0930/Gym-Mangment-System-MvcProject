using GymMangmentSystem.DAL.Data.Models;
using MvcProjectG03.Models;

namespace GymMangmentSystem.DAL.Models
{
    public class Membership : BaseEntity
    {
        public int MemberId { get; set; }
        public Member Member { get; set; } = default!;

        public int PlanId { get; set; }
        public Plan Plan { get; set; } = default!;

        public DateTime EndDate { get; set; }
        public string Status => EndDate > DateTime.Now ? "Active" : "Expired";
        public bool IsActive => EndDate > DateTime.Now;
    }
}
