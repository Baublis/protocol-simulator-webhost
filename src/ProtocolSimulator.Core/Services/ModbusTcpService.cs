using Microsoft.Extensions.Logging;
using ProtocolSimulator.Core.Models.Common;
using ProtocolSimulator.Core.Models.ModbusTcp;
using ProtocolSimulator.Protocols.MyProtocols.ModbusTcp.Servers;
using Timer = System.Threading.Timer;

namespace ProtocolSimulator.Core.Services;

public class ModbusTcpService : IDisposable
{
    private readonly ILogger<ModbusTcpServer> _logger;
    private Timer? _simulationTimer;
    private bool _disposed;

    public ModbusTcpServer Server { get; private set; }
    public int NextAddress { get; set; } = 2;
    public Dictionary<int, ModbusVariableModel> Variables { get; } = new();
    public HashSet<ModbusVariableModel> SimulationVariables { get; } = new();
    public SimulationSettingsModel SimulationSettings { get; private set; } = new();
    public bool SimulateRunning => _simulationTimer != null;

    public event Action? SimulateEvent;

    public ModbusTcpService(ILogger<ModbusTcpServer> logger)
    {
        _logger = logger;
        Server = new ModbusTcpServer(logger);
    }

    public void SaveSimulationSettings(SimulationSettingsModel settings) => SimulationSettings = settings;

    public Task StartSimulation()
    {
        var period = SimulationSettings.TimeChanges * 1000;
        _simulationTimer = new Timer(Simulate, null, 0, period);
        return Task.CompletedTask;
    }

    public async Task StopSimulation()
    {
        if (_simulationTimer != null)
        {
            await _simulationTimer.DisposeAsync();
            _simulationTimer = null;
        }
    }

    private void Simulate(object? state)
    {
        // Заглушка. Реальная логика (SetFloatValue и т.д.) будет добавлена
        // при переносе MyProtocols.ModbusTcp.
        SimulateEvent?.Invoke();
    }

    public void Dispose()
    {
        if (_disposed) return;
        StopSimulation().Wait();
        Server?.Dispose();
        _simulationTimer?.Dispose();
        _disposed = true;
    }
}