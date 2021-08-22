using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calendar.Models
{
    public class EventModel
    {
        public Guid Id { get; set; }
        public DateTime Time { get; set; }
        public string Data { get; set; }
    }
}
