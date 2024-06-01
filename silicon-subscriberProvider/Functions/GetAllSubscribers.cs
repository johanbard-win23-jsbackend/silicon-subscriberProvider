using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using silicon_subscriberProvider.Infrastructure.Services;

namespace silicon_subscriberProvider.Functions;

public class GetAllSubscribers(ILogger<GetAllSubscribers> logger, ISubscriberService subscriberService)
{
    private readonly ILogger<GetAllSubscribers> _logger = logger;
    private readonly ISubscriberService _subscriberService = subscriberService;

    [Function("GetAllSubscribers")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        try
        {
            using var ctsTimeOut = new CancellationTokenSource(TimeSpan.FromSeconds(120 * 1000));
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ctsTimeOut.Token, req.HttpContext.RequestAborted);

            var res = await _subscriberService.GetAllSubscribersAsync(cts.Token);

            switch (res.Status)
            {
                case "200":
                    return new OkObjectResult(res.Subscribers);
                case "500":
                    return new ObjectResult(new { Error = $"Function GetAllSubscribers failed :: {res.Error}" }) { StatusCode = 500 };
                default:
                    return new ObjectResult(new { Error = $"Function GetAllSubscribers failed :: Unknown Error" }) { StatusCode = 500 };
            }
        }
        catch (Exception ex)
        {
            return new ObjectResult(new { Error = $"Function GetAllSubscribers failed :: {ex.Message}" }) { StatusCode = 500 };
        }
    }
}
