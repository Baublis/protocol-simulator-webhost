using Microsoft.Extensions.Logging;
using ProtocolSimulator.Core.Models.Common;
using ProtocolSimulator.Core.Models.OpcUa;
using ProtocolSimulator.Protocols.MyProtocols.OpcUa.Servers;
using Timer = System.Threading.Timer;

namespace ProtocolSimulator.Core.Services;

public class OpcUaService : IDisposable
{
    private readonly ILogger<OpcUaServer> _logger;
    private Timer? _simulationTimer;
    private bool _disposed;

    public OpcUaServer Server { get; private set; }
    public Dictionary<string, OpcVariableModel> SimulationVariables { get; } = new();
    public SimulationSettingsModel SimulationSettings { get; private set; } = new();
    public bool SimulateRunning => _simulationTimer != null;

    public event Action? SimulateEvent;

    public OpcUaService(ILogger<OpcUaServer> logger)
    {
        _logger = logger;
        Server = new OpcUaServer(logger);
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
        // Заглушка. Реальная логика будет добавлена при переносе MyProtocols.OpcUa.
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