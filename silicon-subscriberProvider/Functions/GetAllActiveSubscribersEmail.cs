using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using silicon_subscriberProvider.Infrastructure.Services;

namespace silicon_subscriberProvider.Functions;

public class GetAllActiveSubscribersEmail(ILogger<GetAllActiveSubscribersEmail> logger, ISubscriberService subscriberService)
{
    private readonly ILogger<GetAllActiveSubscribersEmail> _logger = logger;
    private readonly ISubscriberService _subscriberService = subscriberService;

    [Function("GetAllActiveSubscribersEmail")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        try
        {
            using var ctsTimeOut = new CancellationTokenSource(TimeSpan.FromSeconds(120 * 1000));
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ctsTimeOut.Token, req.HttpContext.RequestAborted);

            var res = await _subscriberService.GetAllActiveSubscribersEmailAsync(cts.Token);

            switch (res.Status)
            {
                case "200":
                    return new OkObjectResult(res.Emails);
                case "500":
                    return new ObjectResult(new { Error = $"Function DeleteSubscriber failed :: {res.Error}" }) { StatusCode = 500 };
                default:
                    return new ObjectResult(new { Error = $"Function DeleteSubscriber failed :: Unknown Error" }) { StatusCode = 500 };
            }
        }
        catch (Exception ex)
        {
            return new ObjectResult(new { Error = $"Function GetAllSubscribers failed :: {ex.Message}" }) { StatusCode = 500 };
        }
    }
}
