using Amazon.EventBridge;
using HelloWorld.Core.Services;
using HelloWorld.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HelloWorld.Infrastructure;
public class Startup
{
    public static ServiceProvider? Services { get; private set;}

    public static void ConfigureServices()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton<INetworkService, NetworkService>();
        serviceCollection.AddSingleton<IAmazonEventBridge, AmazonEventBridgeClient>();
        Services = serviceCollection.BuildServiceProvider();        
    }

    public static T GetService<T>() where T : notnull => Services!.GetRequiredService<T>();

}