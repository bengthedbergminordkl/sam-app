using System.Text.Json.Serialization;

namespace HelloWorld.Core.Events;

public class MetaData
{
    [JsonPropertyName("source")]
    public string? Source { get; set; }
}
