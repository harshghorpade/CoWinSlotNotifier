using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoWinService.Domain.Model
{
    // object to send SMS or E-mail notification
    public class MessageFormat
    {
        public string name { get; set; }
        public string address { get; set; }
        public float available_capacity { get; set; }
        public int min_age_limit { get; set; }
        public string vaccine { get; set; }
        public int pincode { get; set; }
    }
}
