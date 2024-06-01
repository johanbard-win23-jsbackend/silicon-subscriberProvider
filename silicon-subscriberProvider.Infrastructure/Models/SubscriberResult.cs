namespace silicon_subscriberProvider.Infrastructure.Models
{
    public class SubscriberResult
    {
        public SubscriberModel? Subscriber {  get; set; }
        public IEnumerable<SubscriberModel>? Subscribers { get; set; }
        public IEnumerable<string>? Emails { get; set; }
        public string Status { get; set; } = null!;
        public string? Error { get; set;}
    }
}
