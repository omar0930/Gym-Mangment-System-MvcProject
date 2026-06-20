using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMangmentSystem.BLL.ViewModels.MembershipViewModels
{
    public class MembershipViewModel
    {
        public int MemberId { get; set; }
        public int PlanId { get; set; }
        public string MemberName { get; set; } = default!;
        public string PlanName { get; set; } = default!;
        public decimal Price { get; set; }
        public int DurationInDays { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Computed properties
        public bool IsActive => EndDate > DateTime.Now;
        public string Status => IsActive ? "Active" : "Expired";
        public string StartDateDisplay => $"{StartDate:MMM dd, yyyy}";
        public string EndDateDisplay => $"{EndDate:MMM dd, yyyy}";
        public int DaysRemaining => IsActive ? (EndDate - DateTime.Now).Days : 0;
    }
}
