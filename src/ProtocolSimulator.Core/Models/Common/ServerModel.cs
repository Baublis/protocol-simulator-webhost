namespace ProtocolSimulator.Core.Models.Common;

public class ServerModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Port { get; set; }

    public byte ModBusKp { get; set; } = 0;

    public ServerType Type { get; set; }

    public ServerStatus Status { get; set; } = ServerStatus.Stopped;

    public int VariableCount { get; set; }

    public bool IsSimulating { get; set; }
}