using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Calendar.EF.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string Nickname { get; set; }
        public string Password { get; set; }

        public List<Event> Events { get; set; }

    }
}
