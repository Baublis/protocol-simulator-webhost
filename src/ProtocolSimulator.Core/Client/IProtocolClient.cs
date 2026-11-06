using ProtocolSimulator.Core.Models.Common;

namespace ProtocolSimulator.Core.Client;

public interface IProtocolClient : IDisposable, IAsyncDisposable
{
    string Host { get; set; }

    int Port { get; set; }

    bool IsConnected { get; }

    Task ConnectAsync();

    Task DisconnectAsync();

    Task<TagValue?> ReadAsync(string tagName);

    Task WriteAsync(string tagName, object value);
}