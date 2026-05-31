using System.ComponentModel.DataAnnotations;

namespace GymMangmentSystem.BLL.ViewModels.PlanViewModels
{
    public class PlanToUpdateViewModel
    {
        public int Id { get; set; }

        public string PlanName { get; set; } = default!;

        [Required(ErrorMessage = "Duration Is Required")]
        [Range(1, 365, ErrorMessage = "Duration must be between 1 and 365 days")]
        public int DurationDays { get; set; }

        [Required(ErrorMessage = "Price Is Required")]
        [Range(0, 100000, ErrorMessage = "Price must be a positive value")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Description Is Required")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Description must be between 5 and 200 characters")]
        public string Description { get; set; } = default!;
    }
}
