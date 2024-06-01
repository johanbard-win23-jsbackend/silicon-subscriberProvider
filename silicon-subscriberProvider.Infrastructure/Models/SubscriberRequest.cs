namespace silicon_subscriberProvider.Infrastructure.Models;

public class SubscriberRequest
{
    public int? Id { get; set; }
    public string Email { get; set; } = null!;
    public bool isActive { get; set; }
}
