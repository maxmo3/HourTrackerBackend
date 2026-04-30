using Microsoft.AspNetCore.Identity;

namespace HourTrackerBackend.Modals.Database
{
    public class User : IdentityUser
    {
        public bool IsAdmin { get; set; } = false;
    }
}
