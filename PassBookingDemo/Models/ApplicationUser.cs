using Microsoft.AspNetCore.Identity;

namespace PassBookingDemo.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<ApplicationUserGymClass> AttendedClasses { get; set; }
    }
}
