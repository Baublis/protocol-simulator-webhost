namespace ProtocolSimulator.Core.Server;

public interface IProtocolServer : IDisposable, IAsyncDisposable
{
    int Port { get; set; }

    bool IsRunning { get; }

    Task StartAsync();

    Task StopAsync();

    event Action? SimulationTick;
}