using GymMangmentSystem.DAL.Data.Models;

namespace GymMangmentSystem.DAL.Models
{
    public class Plan : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int DurationInDays { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        // Relations
        public ICollection<Membership> Memberships { get; set; } = default!;
    }
}
