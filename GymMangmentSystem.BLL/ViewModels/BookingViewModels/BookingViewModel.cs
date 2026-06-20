using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMangmentSystem.BLL.ViewModels.BookingViewModels
{
    public class BookingViewModel
    {
        public int MemberId { get; set; }
        public int SessionId { get; set; }
        public string MemberName { get; set; } = default!;
        public string CategoryName { get; set; } = default!;
        public string TrainerName { get; set; } = default!;
        public string SessionDescription { get; set; } = default!;
        public DateTime SessionStartDate { get; set; }
        public DateTime SessionEndDate { get; set; }
        public bool IsAttended { get; set; }
        public DateTime BookingDate { get; set; }

        // Computed properties
        public string BookingDateDisplay => $"{BookingDate:MMM dd, yyyy}";
        public string SessionDateDisplay => $"{SessionStartDate:MMM dd, yyyy}";
        public string SessionTimeDisplay => $"{SessionStartDate:hh:mm tt} - {SessionEndDate:hh:mm tt}";
        public string Status
        {
            get
            {
                if (SessionStartDate > DateTime.Now)
                    return "Upcoming";
                else if (SessionStartDate <= DateTime.Now && SessionEndDate >= DateTime.Now)
                    return "Ongoing";
                else
                    return "Completed";
            }
        }
    }
}
