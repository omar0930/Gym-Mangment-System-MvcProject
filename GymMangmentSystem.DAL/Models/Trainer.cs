using GymMangmentSystem.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMangmentSystem.DAL.Models
{
    public class Trainer : GymUser
    {
        //hireDate = Created Date
        public Specialty Specialty { get; set; }
    }
}
