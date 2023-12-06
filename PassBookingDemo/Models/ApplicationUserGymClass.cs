namespace PassBookingDemo.Models
{
    public class ApplicationUserGymClass
    {
        public string ApplicationUserId { get; set; }

        public int GymClassId { get; set; }

        public ApplicationUser user { get; set; }

        public GymClass GymClass { get; set;}
    }
}
