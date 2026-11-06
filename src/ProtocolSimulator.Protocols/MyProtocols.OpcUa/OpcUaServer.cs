using Microsoft.Extensions.Logging;

namespace ProtocolSimulator.Protocols.MyProtocols.OpcUa.Servers;

public class OpcUaServer : IDisposable
{
    private readonly ILogger _logger;
    public bool IsRunning { get; private set; }
    public int Port { get; set; } = 4840;

    public OpcUaServer(ILogger logger)
    {
        _logger = logger;
    }

    public Task StartUaServer()
    {
        _logger.LogInformation("OpcUaServer (заглушка) запущен на порту {Port}", Port);
        IsRunning = true;
        return Task.CompletedTask;
    }

    public Task StopUaServer()
    {
        _logger.LogInformation("OpcUaServer (заглушка) остановлен");
        IsRunning = false;
        return Task.CompletedTask;
    }

    public void Dispose() { }
}