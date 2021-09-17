using Chaos.Engine.Contracts.Controllers;

namespace Chaos.Engine.Contracts
{
    public interface IChaosClientAPI
    {
        IClientCreatureController Creatures { get; }
        IClientGameworldController World { get; }
        IClientMetaController Meta { get; }
        IClientShaderController Shaders { get; }
        IClientEntityController Entities { get; }
    }
}