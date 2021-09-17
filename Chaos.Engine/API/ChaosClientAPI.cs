using Chaos.Engine.Contracts;
using Chaos.Engine.Contracts.Controllers;
using Chaos.Engine.Controllers;

namespace Chaos.Engine.API
{
    public class ChaosClientAPI : IChaosClientAPI
    {
        public IClientCreatureController Creatures { get; } = new ClientCreatureController();
        public IClientGameworldController World { get; } = new ClientGameworldController();
        public IClientMetaController Meta { get; } = new ClientMetaController();
        public IClientShaderController Shaders { get; } = new ClientShaderController();
        public IClientEntityController Entities { get; } = new ClientEntityController();
    }
}