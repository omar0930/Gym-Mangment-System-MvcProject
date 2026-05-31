using GymMangmentSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMangmentSystem.DAL.Data.Models
{
    public class HealthRecord : BaseEntity
    {
        public decimal Height { get; set; }
        public decimal Width { get; set; }
        public string? Note { get; set; }
        public string BloodType { get; set; }

        //last updated = updatedat 

        //Relations
        public Member Member { get; set; } = default!;
    }
}
