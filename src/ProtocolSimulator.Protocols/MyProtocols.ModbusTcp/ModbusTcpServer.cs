using Microsoft.Extensions.Logging;

namespace ProtocolSimulator.Protocols.MyProtocols.ModbusTcp.Servers;

public class ModbusTcpServer : IDisposable
{
    private readonly ILogger _logger;
    public bool IsRunning { get; private set; }
    public int Port { get; set; } = 502;
    public byte UnitIdentifier { get; set; } = 1;

    public ModbusTcpServer(ILogger logger)
    {
        _logger = logger;
    }

    public Task StartAsync()
    {
        _logger.LogInformation("ModbusTcpServer (заглушка) запущен на порту {Port}", Port);
        IsRunning = true;
        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        _logger.LogInformation("ModbusTcpServer (заглушка) остановлен");
        IsRunning = false;
        return Task.CompletedTask;
    }

    // Методы для установки значений (пока просто пустые)
    public void SetHoldingRegister(int address, short value) { }
    public void SetHoldingRegister(int address, ushort value) { }
    public void SetHoldingRegister(int address, uint value) { }
    public void SetHoldingRegister(int address, float value) { }
    public void SetHoldingRegister(int address, double value) { }
    public void SetHoldingRegister(int address, ulong value) { }

    public void Dispose() { }
}