using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;

using HelloWorld.Core.Services;
using HelloWorld.Infrastructure;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Reflection.Metadata.Ecma335;
using System.Data.Common;
using System.Net;
using Amazon.EventBridge;
using Amazon.EventBridge.Model;
using HelloWorld.Core.Events;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace HelloWorld;
public class Function
{
    private readonly INetworkService _networkService;
    private readonly IAmazonEventBridge _eventBridgeClient;

    // Lambda service requires a parameterless constructor
    public Function() : this(null, null)
    {
    }

    // Internal contructor that can be used when mocking services for testing
    internal Function(INetworkService? networkService = null, IAmazonEventBridge? eventBridgeClient = null)
    {
        Startup.ConfigureServices();
        _networkService = networkService ?? Startup.GetService<INetworkService>();
        _eventBridgeClient = eventBridgeClient ?? Startup.GetService<IAmazonEventBridge>();
    }
    
    public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest apigProxyEvent, ILambdaContext context)
    {

        if (!(apigProxyEvent.QueryStringParameters?.ContainsKey("message") ?? false))
        {
            return new APIGatewayProxyResponse
            {
                Body = JsonSerializer.Serialize(
                    new { message = "Missing required query string named 'message'" },
                    new JsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    }),                
                StatusCode = 400,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };    
        }

        var location = await _networkService.GetCallingIP();
        var body = new Dictionary<string, string>
        {
            { "message", apigProxyEvent.QueryStringParameters["message"] },
            { "location", location }
        };

        await this._eventBridgeClient.PutEventsAsync(new PutEventsRequest()
        {
            Entries = new List<PutEventsRequestEntry>(1)
            {
                new PutEventsRequestEntry()
                {
                    Source = "network-service",
                    EventBusName = System.Environment.GetEnvironmentVariable("EVENT_BUS_NAME"),
                    DetailType = "new-ip-address",
                    Detail = JsonSerializer.Serialize(new EventWrapper<NewIpAddressEvent>()
                    {
                        MetaData = new MetaData()
                        {
                            Source = "network-service"
                        },
                        Data = new NewIpAddressEvent()
                        {
                            IpAddress = location
                        }
                    })
                }
            }
        });

        return new APIGatewayProxyResponse
        {
            Body = JsonSerializer.Serialize(body),
            StatusCode = 200,
            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
        };
    }
}


