using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReuniometroWPA.Model
{
    public class Attendee
    {
        public string Name { get; set; }
        public int Number { get; set; }
        public decimal CostPerHour { get; set; }
        public decimal TotalCost { get { return Number * CostPerHour; } }
    }
}
