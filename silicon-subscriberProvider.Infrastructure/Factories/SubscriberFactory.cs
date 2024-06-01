using silicon_subscriberProvider.Infrastructure.Data.Entities;
using silicon_subscriberProvider.Infrastructure.Models;

namespace silicon_subscriberProvider.Infrastructure.Factories;

public static class SubscriberFactory
{
    public static SubscriberEntity Create(SubscriberModel model)
    {
        return new SubscriberEntity
        {
            Id = model.Id,
            Email = model.Email,
            isActive = model.isActive
        };
    }

    public static SubscriberModel Create(SubscriberEntity entity)
    {
        return new SubscriberModel
        {
            Id = entity.Id,
            Email = entity.Email,
            isActive = entity.isActive
        };
    }
}
