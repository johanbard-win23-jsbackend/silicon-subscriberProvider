using Microsoft.EntityFrameworkCore;
using silicon_subscriberProvider.Infrastructure.Data.Entities;

namespace silicon_subscriberProvider.Infrastructure.Data.Contexts;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<SubscriberEntity> Subscribers { get; set; }
}
