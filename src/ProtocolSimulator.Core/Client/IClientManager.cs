using ProtocolSimulator.Core.Models.Common;

namespace ProtocolSimulator.Core.Client;

public interface IClientManager
{
    Task<int> CreateClientAsync(ServerType type);

    Task RemoveClientAsync(int id);
}