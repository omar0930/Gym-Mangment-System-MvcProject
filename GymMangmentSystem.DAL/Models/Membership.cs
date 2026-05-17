using MvcProjectG03.Models;

namespace GymMangmentSystem.DAL.Models
{
    public class Membership
    {
        public int MemberId { get; set; }
        public Member Member { get; set; } = default!;

        public int PlanId { get; set; }
        public Plan Plan { get; set; } = default!;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
