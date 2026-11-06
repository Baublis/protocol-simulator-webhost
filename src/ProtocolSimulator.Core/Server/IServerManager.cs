using ProtocolSimulator.Core.Models.Common;

namespace ProtocolSimulator.Core.Server;

public interface IServerManager
{
    List<ServerModel> OpcUaModels { get; }

    List<ServerModel> ModbusModels { get; }

    int RunningServerCount { get; }

    int SimulatinServerCount { get; }

    Task<int> CreateServerAsync(ServerType type);

    Task RemoveServers(List<ServerModel> servers);

    Task StartStopServer(ServerModel server);

    Task StartStopStopSimulation(ServerModel server);

    Task RefreshModels();
}