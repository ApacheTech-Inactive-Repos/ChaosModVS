using Chaos.Engine.Contracts;
using Chaos.Engine.Contracts.Controllers;
using Chaos.Engine.Controllers;

namespace Chaos.Engine.API
{
    public class ChaosServerAPI : IChaosServerAPI
    {
        public IServerCreatureController Creatures { get; } = new ServerCreatureController();
        public IServerGameworldController World { get; } = new ServerGameworldController();
        public IServerMetaController Meta { get; } = new ServerMetaController();
        public IServerShaderController Shaders { get; } = new ServerShaderController();
        public IServerEntityController Entities { get; } = new ServerEntityController();
    }
}