using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using silicon_subscriberProvider.Infrastructure.Data.Contexts;
using silicon_subscriberProvider.Infrastructure.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddDbContextFactory<DataContext>(options =>
        {
            options.UseSqlServer(Environment.GetEnvironmentVariable("SIL_SUB_DB"));
        });

        services.AddScoped<ISubscriberService, SubscriberService>();

    })
    .Build();

host.Run();
