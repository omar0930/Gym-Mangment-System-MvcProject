using GymMangmentSystem.DAL.Data.Models.Enums;
using GymMangmentSystem.DAL.Models;

namespace GymMangmentSystem.DAL.Data.Models
{
    public class HealthRecord : BaseEntity
    {
        public decimal Height { get; set; }
        public decimal Width { get; set; }
        public decimal Weight { get; set; }
        public string? Note { get; set; }
        public BloodType BloodType { get; set; }

        //last updated = updatedat 

        //Relations
        public Member Member { get; set; } = default!;
    }
}
