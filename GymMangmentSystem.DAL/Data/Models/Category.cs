using GymMangmentSystem.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMangmentSystem.DAL.Models
{
    public class Category : BaseEntity
    {
        public string CategoryName { get; set; } = default!;

        //Relations
        public ICollection<Session> Sessions { get; set; } = default!;
    }
}
