namespace GymMangmentSystem.DAL.Models
{
    public class Booking
    {
        public int MemberId { get; set; }
        public Member Member { get; set; } = default!;

        public int SessionId { get; set; }
        public Session Session { get; set; } = default!;

        public bool IsAttended { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
