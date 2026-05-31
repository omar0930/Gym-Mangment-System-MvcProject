using GymMangmentSystem.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMangmentSystem.DAL.Models
{
    public class Session : BaseEntity
    {
        public string Descreption { get; set; } = default!;
        public int Capacity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        //Relations
        public Trainer Trainer { get; set; } = default!;
        public int TrainerId { get; set; }
        public Category Category { get; set; } = default!;
        public int CategoryId { get; set; }
        public ICollection<Booking> SessionMembers { get; set; } = default!;
    }
}
