using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Calendar.EF.Entities
{
    public class Event
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime Time { get; set; }
        public string Data { get; set; }

        public Guid OwnerId { get; set; }
        public User Owner { get; set; }

    }
}
