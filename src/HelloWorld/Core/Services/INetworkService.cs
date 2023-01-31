
using System.Threading.Tasks;

namespace HelloWorld.Core.Services;

public interface INetworkService
{
    public Task<string> GetCallingIP();
}