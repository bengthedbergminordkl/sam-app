using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Amazon.EventBridge;
using Amazon.EventBridge.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using FluentAssertions;
using HelloWorld.Core.Services;
using Moq;
using Newtonsoft.Json.Linq;
using NJsonSchema;
using Xunit;

namespace HelloWorld.Tests;
public class FunctionTest
{

    [Theory]
    [InlineData("valid_request.json", "valid_response.json","valid_eventschema.json")]
    [InlineData("invalid_request.json", "invalid_response.json", null)]
    public async Task TestHelloWorldFunctionHandler(string requestFile, string responseFile, string eventSchema)
    {
        var options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        var expectedResponse = JsonSerializer.Deserialize<APIGatewayProxyResponse>(json: File.ReadAllText(path: $"./Events/{responseFile}"), options);
        var request = JsonSerializer.Deserialize<APIGatewayProxyRequest>(json: File.ReadAllText(path: $"./Events/{requestFile}"), options);

        PutEventsRequestEntry eventRequest = null;

        var mockNetworkService = new Mock<INetworkService>();
        mockNetworkService.Setup(p => p.GetCallingIP()).ReturnsAsync("10.0.0.1");

        var mockEventBridgeClient = new Mock<IAmazonEventBridge>();
        mockEventBridgeClient.Setup(p => p.PutEventsAsync(It.IsAny<PutEventsRequest>(), default))
            .Callback<PutEventsRequest, CancellationToken>((req, token) =>
            {
                eventRequest = req.Entries.FirstOrDefault();
            });

        var function = new Function(mockNetworkService.Object, mockEventBridgeClient.Object);

        var response = await function.FunctionHandler(request, new TestLambdaContext());

        response.StatusCode.Should().Be(expectedResponse.StatusCode);
        response.Body.Should().Be(expectedResponse.Body);

        await verifyEvent(eventSchema, eventRequest);
    }

    private async Task verifyEvent(string eventSchema, PutEventsRequestEntry eventRequest)
    {
        if (eventSchema != null)
        {
            var schema = await JsonSchema.FromFileAsync($"./Events/{eventSchema}");
            var eventObject = JToken.Parse(eventRequest.Detail);
            var validationErrors = schema.Validate(eventObject);

            validationErrors.Should().BeEmpty();
        }
        else
        {
            eventRequest.Should().BeNull(because: "Event should not have been published");
        }
    }

}

