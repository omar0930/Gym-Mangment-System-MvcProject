namespace GymMangmentSystem.BLL.ViewModels.TrainerViewModels
{
    public class TrainerViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string DateOfBirth { get; set; } = null!;
        public string Specialty { get; set; } = null!;
        public decimal Salary { get; set; }
        public int YearsOfExperience { get; set; }
        // Address
        public string Street { get; set; } = null!;
        public string City { get; set; } = null!;
        public int BuildingNumber { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
    }
}
