namespace ProtocolSimulator.Core.Models.Common;

public class TagValue
{
    public object? Value { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.Now;

    public uint StatusCode { get; set; } = 0; // Good = 0

    public bool IsGood => StatusCode == 0;
}