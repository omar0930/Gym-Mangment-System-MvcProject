using GymMangmentSystem.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMangmentSystem.DAL.Models
{
    public class Member : GymUser
    {
        public string? Photo { get; set; } = default!;

        //joinDate = Created Date

        //Relations
        public HealthRecord HealthRecord { get; set; } = default!;
        public ICollection<Membership> Memberships { get; set; } = default!;
        public ICollection<Booking> MemberSessions { get; set; } = default!;

    }
}
