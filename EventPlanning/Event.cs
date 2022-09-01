namespace EventPlanning
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int MaxCountOfUsers { get; set; }
        public string Loaction { get; set; }
        public List<User> Users { get; set; }
    }
}
