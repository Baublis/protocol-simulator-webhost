using ProtocolSimulator.Core.Models.Common;

namespace ProtocolSimulator.Core.Models.ModbusTcp;

public class ModbusVariableModel
{
    public Type DataTypeNet { get; set; } = SimulatorDataTypes.UInt16;
    public uint CloneCount { get; set; } = 1;
    public bool Simulated { get; set; } = true;
    public object? Value { get; set; } = 0;
    public int Address { get; set; } = 1;
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string StringAddress => $"{Address - 1:D5}";
    public string StringTypeValue => DataTypeNet.Name;
}