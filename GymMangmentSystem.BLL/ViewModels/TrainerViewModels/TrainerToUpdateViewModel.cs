using GymMangmentSystem.DAL.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace GymMangmentSystem.BLL.ViewModels.TrainerViewModels
{
    public class TrainerToUpdateViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Gender { get; set; }
        public string? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "Phone Number Is Required")]
        [RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "Phone number must be a valid Egyptian mobile number")]
        public string Phone { get; set; } = default!;

        [Required(ErrorMessage = "Street Is Required")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Street must be between 2 and 150 characters")]
        public string Street { get; set; } = default!;

        [Required(ErrorMessage = "City Is Required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "City must be between 2 and 100 characters")]
        public string City { get; set; } = default!;

        public string? State { get; set; }

        public string? ZipCode { get; set; }

        [Required(ErrorMessage = "Building Number Is Required")]
        [Range(1, 9000, ErrorMessage = "Building Number must be greater than 0")]
        public int BuildingNumber { get; set; }

        [Required(ErrorMessage = "Specialization is required")]
        public Specialty Specialty { get; set; }

        [Required(ErrorMessage = "Salary is required")]
        [Range(0.01, 1000000, ErrorMessage = "Salary must be greater than 0")]
        public decimal Salary { get; set; }

        [Required(ErrorMessage = "Years Of Experience is required")]
        [Range(0, 60, ErrorMessage = "Years Of Experience must be between 0 and 60")]
        public int YearsOfExperience { get; set; }
    }
}
