using ProtocolSimulator.Core.Models.Common;

namespace ProtocolSimulator.Core.Models.OpcUa;

public class OpcVariableModel
{
    public string Name { get; set; } = "00000";
    public object? Value { get; set; }
    public Type DataTypeNet { get; set; } = SimulatorDataTypes.Boolean;
    public uint StatusCode { get; set; } = 0;
    public DateTime Timestamp { get; set; } = DateTime.Now;
}