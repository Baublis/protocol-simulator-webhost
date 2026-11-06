namespace ProtocolSimulator.Core.Models.Common;

public class SimulationSettingsModel
{
    public uint NumberChanges { get; set; } = 10;

    public uint TimeChanges { get; set; } = 2;

    public int LowerValue { get; set; } = -50;

    public int UpperValue { get; set; } = 50;
}