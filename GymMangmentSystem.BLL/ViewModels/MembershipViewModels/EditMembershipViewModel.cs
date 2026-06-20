using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMangmentSystem.BLL.ViewModels.MembershipViewModels
{
    public class EditMembershipViewModel
    {
        public int MemberId { get; set; }
        public int PlanId { get; set; }

        public string MemberName { get; set; } = default!;
        public string PlanName { get; set; } = default!;

        [Required(ErrorMessage = "End date is required")]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
    }
}
