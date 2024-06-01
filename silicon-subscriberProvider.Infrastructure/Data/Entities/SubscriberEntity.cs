using System.ComponentModel.DataAnnotations;

namespace silicon_subscriberProvider.Infrastructure.Data.Entities;

public class SubscriberEntity
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public bool isActive { get; set; }
}
