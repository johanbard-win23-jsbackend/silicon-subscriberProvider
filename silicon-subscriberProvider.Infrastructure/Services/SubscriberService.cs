using Microsoft.EntityFrameworkCore;
using silicon_subscriberProvider.Infrastructure.Data.Contexts;
using silicon_subscriberProvider.Infrastructure.Data.Entities;
using silicon_subscriberProvider.Infrastructure.Factories;
using silicon_subscriberProvider.Infrastructure.Models;

namespace silicon_subscriberProvider.Infrastructure.Services;

public interface ISubscriberService
{
    public Task<SubscriberResult> CreateSubscriberAsync(SubscriberRequest sReq, CancellationToken cts);

    public Task<SubscriberResult> GetSubscriberAsync(SubscriberRequest sReq, CancellationToken cts);

    public Task<SubscriberResult> GetAllSubscribersAsync(CancellationToken cts);

    public Task<SubscriberResult> GetAllActiveSubscribersEmailAsync(CancellationToken cts);

    public Task<SubscriberResult> UpdateSubscriberAsync(SubscriberRequest sReq, CancellationToken cts);

    public Task<SubscriberResult> DeleteSubscriberAsync(SubscriberRequest sReq, CancellationToken cts);
}

public class SubscriberService(IDbContextFactory<DataContext> contextFactory) : ISubscriberService
{
    private readonly IDbContextFactory<DataContext> _contextFactory = contextFactory;

    public async Task<SubscriberResult> CreateSubscriberAsync(SubscriberRequest sReq, CancellationToken cts)
    {
        if (sReq != null && sReq.Email != null)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                if (!await context.Subscribers.AnyAsync(x => x.Email == sReq.Email, cts))
                {
                    await context.Subscribers.AddAsync(new SubscriberEntity { Email = sReq.Email, isActive = sReq.isActive }, cts);
                    await context.SaveChangesAsync(cts);

                    var entity = await context.Subscribers.FirstOrDefaultAsync(x => x.Email == sReq.Email, cts);

                    if (entity != null)
                        return new SubscriberResult { Status = "200", Subscriber = SubscriberFactory.Create(entity) };
                    else
                        return new SubscriberResult { Status = "500", Error = "New subscriber not created" };
                }
                else
                {
                    return new SubscriberResult { Status = "409", Error = "Conflict" };
                }
                    
            }
            catch (Exception ex) { return new SubscriberResult { Status = "500", Error = ex.Message }; }  
        }

        return new SubscriberResult { Status = "400", Error = "Bad request" };
    }

    public async Task<SubscriberResult> DeleteSubscriberAsync(SubscriberRequest sReq, CancellationToken cts)
    {
        if (sReq != null && sReq.Email != null)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                var entity = await context.Subscribers.FirstOrDefaultAsync(x => x.Email == sReq.Email);

                if (entity != null) 
                {
                    context.Subscribers.Remove(entity);
                    await context.SaveChangesAsync();

                    return new SubscriberResult { Status = "200" };
                }
                else
                {
                    return new SubscriberResult { Status = "400", Error = "Email not in db" };
                }
                
            }
            catch (Exception ex) { return new SubscriberResult { Status = "500", Error = ex.Message }; }
        }

        return new SubscriberResult { Status = "400", Error = "Bad request" };
    }

    public async Task<SubscriberResult> GetAllSubscribersAsync(CancellationToken cts)
    {
        
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            var entities = await context.Subscribers.ToListAsync();

            if (entities != null)
            {
                var subscribers = entities.Select(x => SubscriberFactory.Create(x));

                return new SubscriberResult { Status = "200", Subscribers = subscribers };
            }
        }
        catch (Exception ex) { return new SubscriberResult { Status = "500", Error = ex.Message }; }
    
        return new SubscriberResult { Status = "500", Error = "Unknown error" };
    }

    public async Task<SubscriberResult> GetAllActiveSubscribersEmailAsync(CancellationToken cts)
    {
        try
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            var entities = await context.Subscribers.ToListAsync();

            if (entities != null)
            {
                var emails = new List<string>();

                foreach (var e in entities)
                {
                    if(e.isActive)
                        emails.Add(e.Email);
                }
                return new SubscriberResult { Status = "200", Emails = emails};
            }
        }
        catch (Exception ex) { return new SubscriberResult { Status = "500", Error = ex.Message }; }

        return new SubscriberResult { Status = "500", Error = "Unknown error" };
    }

    public async Task<SubscriberResult> GetSubscriberAsync(SubscriberRequest sReq, CancellationToken cts)
    {
        if (sReq != null)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                SubscriberEntity entity = new SubscriberEntity();

                if (sReq.Email != null)
                {
                    var res = await context.Subscribers.FirstOrDefaultAsync(x => x.Email == sReq.Email);
                    if (res != null)
                    {
                        entity = res;
                        return new SubscriberResult { Status = "200", Subscriber = SubscriberFactory.Create(entity) };
                    }
                    else
                        return new SubscriberResult { Status = "400", Error = "Bad request" };
                }   
                else if (sReq.Id != null)
                {
                    var res = await context.Subscribers.FirstOrDefaultAsync(x => x.Id == sReq.Id);
                    if (res != null)
                    {
                        entity = res;
                        return new SubscriberResult { Status = "200", Subscriber = SubscriberFactory.Create(entity) };
                    }
                    else
                        return new SubscriberResult { Status = "400", Error = "Bad request" };
                }
            }
            catch (Exception ex) { return new SubscriberResult { Status = "500", Error = ex.Message }; }
        }

        return new SubscriberResult { Status = "400", Error = "Bad request" };
    }

    public async Task<SubscriberResult> UpdateSubscriberAsync(SubscriberRequest sReq, CancellationToken cts)
    {
        if (sReq != null && sReq.Id != null && sReq.Email != null)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                var existingEntity = await context.Subscribers.FirstOrDefaultAsync(x => x.Id == sReq.Id, cts);

                if (existingEntity != null)
                {
                    var updatedEntity = new SubscriberEntity
                    {
                        Email = sReq.Email,
                        isActive = sReq.isActive
                    };

                    updatedEntity.Id = existingEntity!.Id;
                    context.Entry(existingEntity).CurrentValues.SetValues(updatedEntity);
                    await context.SaveChangesAsync();

                    var entity = await context.Subscribers.FirstOrDefaultAsync(x => x.Id == updatedEntity.Id);

                    return new SubscriberResult { Status = "200", Subscriber = SubscriberFactory.Create(entity!) };
                }

            }
            catch (Exception ex) { return new SubscriberResult { Status = "500", Error = ex.Message }; }
        }

        return new SubscriberResult { Status = "400", Error = "Bad request" };
    }
}
