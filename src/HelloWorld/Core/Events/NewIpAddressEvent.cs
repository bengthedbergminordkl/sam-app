using System.Text.Json.Serialization;

namespace HelloWorld.Core.Events;

public class NewIpAddressEvent
{

    [JsonPropertyName("ipAddress")]
    public string? IpAddress { get; set; }
}
