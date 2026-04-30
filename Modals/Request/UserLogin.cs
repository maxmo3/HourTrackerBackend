using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HourTrackerBackend.Modals.Request
{
    public class UserLogin
    {
        public required string username { get; set; }
        public required string password { get; set; }
        public string? registrationCode { get; set; }
    }
}
