using GymMangmentSystem.DAL.Data.Models.Enums;
using GymMangmentSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMangmentSystem.DAL.Data.Models
{
    public class Trainer : GymUser
    {
        //hireDate = Created Date
        public Specialty Specialty { get; set; }
        public decimal Salary { get; set; }
        public int YearsOfExperience { get; set; }

        //Relations
        public ICollection<Session> Sessions { get; set; } = default!;
    }
}
