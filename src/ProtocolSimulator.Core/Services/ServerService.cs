using Microsoft.Extensions.DependencyInjection;
using ProtocolSimulator.Core.Models.Common;
using ProtocolSimulator.Core.Server;
using System.Collections.Concurrent;

namespace ProtocolSimulator.Core.Services;

public class ServerService : IServerManager, IDisposable
{
    private const int ModbusDefaultPort = 502;
    private const int OpcDefaultPort = 4840;

    private int _opcNumber;
    private int _modbusNumber;
    private bool _disposed;

    private readonly ConcurrentDictionary<int, OpcUaService> _opcUaServices = new();
    private readonly ConcurrentDictionary<int, ModbusTcpService> _modbusTcpServices = new();
    private readonly ConcurrentDictionary<int, ServerModel> _opcUaModels = new();
    private readonly ConcurrentDictionary<int, ServerModel> _modbusModels = new();

    private readonly IServiceProvider _serviceProvider;

    public ServerService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    // ===== Публичные свойства =====

    public List<ServerModel> OpcUaModels => _opcUaModels.Values.ToList();
    public List<ServerModel> ModbusModels => _modbusModels.Values.ToList();

    public HashSet<int> OpcPorts => _opcUaModels.Values.Select(e => e.Port).ToHashSet();
    public HashSet<int> ModbusPorts => _modbusModels.Values.Select(e => e.Port).ToHashSet();

    public int RunningServerCount =>
        OpcUaModels.Count(s => s.Status == ServerStatus.Running) +
        ModbusModels.Count(s => s.Status == ServerStatus.Running);

    public int SimulatinServerCount =>
        OpcUaModels.Count(s => s.IsSimulating) +
        ModbusModels.Count(s => s.IsSimulating);

    // ===== Create / Remove =====

    public async Task<int> CreateServerAsync(ServerType type)
    {
        if (type == ServerType.OpcUa) return await CreateOpcUaServiceAsync();
        if (type == ServerType.ModbusTcp) return await CreateModbusServiceAsync();
        throw new NotSupportedException($"Тип сервера '{type}' не поддерживается");
    }

    public async Task<int> CreateModbusServiceAsync()
    {
        _modbusNumber++;
        var id = _modbusNumber;

        var serviceInstance = ActivatorUtilities.CreateInstance<ModbusTcpService>(_serviceProvider);

        _modbusTcpServices[id] = serviceInstance;
        _modbusModels[id] = new ServerModel
        {
            Id = id,
            Name = $"Modbus TCP Сервер {id}",
            Port = SelectPort(ModbusDefaultPort, ModbusPorts),
            ModBusKp = 1,
            Type = ServerType.ModbusTcp
        };

        // Заглушка: просто настраиваем порт в самом сервере
        serviceInstance.Server.Port = _modbusModels[id].Port;

        return await Task.FromResult(id);
    }

    public async Task<int> CreateOpcUaServiceAsync()
    {
        _opcNumber++;
        var id = _opcNumber;

        var serviceInstance = ActivatorUtilities.CreateInstance<OpcUaService>(_serviceProvider);

        _opcUaServices[id] = serviceInstance;
        _opcUaModels[id] = new ServerModel
        {
            Id = id,
            Name = $"OPC UA Сервер {id}",
            Port = SelectPort(OpcDefaultPort, OpcPorts),
            Type = ServerType.OpcUa
        };

        serviceInstance.Server.Port = _opcUaModels[id].Port;

        return await Task.FromResult(id);
    }

    public async Task RemoveServers(List<ServerModel> servers)
    {
        foreach (var server in servers)
        {
            if (server.Type == ServerType.OpcUa && _opcUaServices.TryRemove(server.Id, out var opc))
            {
                opc?.Dispose();
                _opcUaModels.TryRemove(server.Id, out _);
            }
            else if (server.Type == ServerType.ModbusTcp && _modbusTcpServices.TryRemove(server.Id, out var modbus))
            {
                modbus?.Dispose();
                _modbusModels.TryRemove(server.Id, out _);
            }
        }
        await Task.CompletedTask;
    }

    // ===== Start / Stop =====

    public async Task StartStopServer(ServerModel serverModel)
    {
        if (serverModel.Type == ServerType.OpcUa &&
            _opcUaServices.TryGetValue(serverModel.Id, out var opcService))
        {
            var running = serverModel.Status == ServerStatus.Running;
            try
            {
                if (running)
                {
                    await opcService.Server.StopUaServer();
                    serverModel.Status = ServerStatus.Stopped;
                }
                else
                {
                    await opcService.Server.StartUaServer();
                    serverModel.Status = opcService.Server.IsRunning
                        ? ServerStatus.Running
                        : ServerStatus.Error;
                }
            }
            catch
            {
                serverModel.Status = ServerStatus.Error;
            }
        }
        else if (serverModel.Type == ServerType.ModbusTcp &&
                 _modbusTcpServices.TryGetValue(serverModel.Id, out var modbusService))
        {
            var running = serverModel.Status == ServerStatus.Running;
            try
            {
                if (running)
                {
                    await modbusService.Server.StopAsync();
                    serverModel.Status = ServerStatus.Stopped;
                }
                else
                {
                    await modbusService.Server.StartAsync();
                    serverModel.Status = modbusService.Server.IsRunning
                        ? ServerStatus.Running
                        : ServerStatus.Error;
                }
            }
            catch
            {
                serverModel.Status = ServerStatus.Error;
            }
        }
    }

    public async Task StartStopStopSimulation(ServerModel serverModel)
    {
        if (serverModel.Type == ServerType.OpcUa &&
            _opcUaServices.TryGetValue(serverModel.Id, out var opcService))
        {
            if (serverModel.IsSimulating)
            {
                await opcService.StopSimulation();
            }
            else
            {
                await opcService.StartSimulation();
            }
            serverModel.IsSimulating = opcService.SimulateRunning;
        }
        else if (serverModel.Type == ServerType.ModbusTcp &&
                 _modbusTcpServices.TryGetValue(serverModel.Id, out var modbusService))
        {
            if (serverModel.IsSimulating)
            {
                await modbusService.StopSimulation();
            }
            else
            {
                await modbusService.StartSimulation();
            }
            serverModel.IsSimulating = modbusService.SimulateRunning;
        }
    }

    // ===== Refresh / Get =====

    public Task RefreshModels() => Task.CompletedTask;

    public Task<(OpcUaService?, ServerModel?)> GetOpcUaServer(int id) =>
        Task.FromResult((
            _opcUaServices.GetValueOrDefault(id),
            _opcUaModels.GetValueOrDefault(id)
        ));

    public Task<(ModbusTcpService?, ServerModel?)> GetModBusServer(int id) =>
        Task.FromResult((
            _modbusTcpServices.GetValueOrDefault(id),
            _modbusModels.GetValueOrDefault(id)
        ));

    // ===== Helpers =====

    private static int SelectPort(int startPort, HashSet<int> ports)
    {
        var port = startPort;
        while (!ports.Add(port)) port++;
        return port;
    }

    // ===== IDisposable =====

    public void Dispose()
    {
        if (_disposed) return;

        foreach (var s in _opcUaServices.Values) s.Dispose();
        foreach (var s in _modbusTcpServices.Values) s.Dispose();
        _opcUaServices.Clear();
        _modbusTcpServices.Clear();
        _opcUaModels.Clear();
        _modbusModels.Clear();

        _disposed = true;
    }
}