using System.Net.Http;
using System.Threading.Tasks;
using HelloWorld.Core.Services;

namespace HelloWorld.Infrastructure.Services;

public class NetworkService : INetworkService
{
    private static readonly HttpClient client = new HttpClient();

    public async Task<string> GetCallingIP()
    {
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Add("User-Agent", "AWS Lambda .Net Client");

        var msg = await client.GetStringAsync("http://checkip.amazonaws.com/").ConfigureAwait(continueOnCapturedContext:false);

        return msg.Replace("\n","");
    }
}

