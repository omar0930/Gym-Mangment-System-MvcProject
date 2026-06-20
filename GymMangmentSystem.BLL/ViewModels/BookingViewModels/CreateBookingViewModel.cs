using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMangmentSystem.BLL.ViewModels.BookingViewModels
{
    public class CreateBookingViewModel
    {
        [Required(ErrorMessage = "Member is required")]
        [Display(Name = "Member")]
        public int MemberId { get; set; }

        [Required(ErrorMessage = "Session is required")]
        [Display(Name = "Session")]
        public int SessionId { get; set; }
    }
}
