namespace silicon_subscriberProvider.Infrastructure.Models
{
    public class SubscriberModel
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public bool isActive { get; set; }
    }
}
