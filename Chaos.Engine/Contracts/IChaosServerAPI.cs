using Chaos.Engine.Contracts.Controllers;

namespace Chaos.Engine.Contracts
{
    public interface IChaosServerAPI
    {
        IServerCreatureController Creatures { get; }
        IServerGameworldController World { get; }
        IServerMetaController Meta { get; }
        IServerShaderController Shaders { get; }
        IServerEntityController Entities { get; }
    }
}