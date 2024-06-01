using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using silicon_subscriberProvider.Infrastructure.Models;
using silicon_subscriberProvider.Infrastructure.Services;

namespace silicon_subscriberProvider.Functions;

public class GetSubscriber(ILogger<GetSubscriber> logger, ISubscriberService subscriberService)
{
    private readonly ILogger<GetSubscriber> _logger = logger;
    private readonly ISubscriberService _subscriberService = subscriberService;

    [Function("GetSubscriber")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        try
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var sReq = JsonConvert.DeserializeObject<SubscriberRequest>(body);

            if (sReq == null)
                return new BadRequestObjectResult(new { Error = "Please provide a valid request" });

            using var ctsTimeOut = new CancellationTokenSource(TimeSpan.FromSeconds(120 * 1000));
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ctsTimeOut.Token, req.HttpContext.RequestAborted);

            var res = await _subscriberService.GetSubscriberAsync(sReq, cts.Token);

            switch (res.Status)
            {
                case "200":
                    return new OkObjectResult(res.Subscriber);
                case "400":
                    return new BadRequestObjectResult(new { Error = $"Function GetSubscriber failed :: {res.Error}" });
                case "500":
                    return new ObjectResult(new { Error = $"Function GetSubscriber failed :: {res.Error}" }) { StatusCode = 500 };
                default:
                    return new ObjectResult(new { Error = $"Function GetSubscriber failed :: Unknown Error" }) { StatusCode = 500 };
            }
        }
        catch (Exception ex)
        {
            return new ObjectResult(new { Error = $"Function GetSubscriber failed :: {ex.Message}" }) { StatusCode = 500 };
        }
    }
}
