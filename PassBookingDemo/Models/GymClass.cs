namespace PassBookingDemo.Models
{
    public class GymClass
    {
        public int Id { get; set; }

        public string Name { get; set; }    

        public string Description { get; set; }

        public DateTime StartTime { get; set; }

        public TimeSpan Duration { get; set; }

        public DateTime EndTime { get { return StartTime + Duration; } }

        public ICollection<ApplicationUserGymClass> AttendingMembers { get; set; } = new List<ApplicationUserGymClass>();




    }
}
