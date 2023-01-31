using System.Text.Json.Serialization;

namespace HelloWorld.Core.Events;

public class EventWrapper<T>
{
    [JsonPropertyName("metadata")]
    public MetaData? MetaData { get; set; }

    [JsonPropertyName("data")]
    public T? Data { get; set; }
}
