using ProtocolSimulator.Core.Models.Common;

namespace ProtocolSimulator.Core.Server;

public interface IProtocolServerFactory
{
    ServerType Type { get; }

    IProtocolServer Create(string name, int port);
}