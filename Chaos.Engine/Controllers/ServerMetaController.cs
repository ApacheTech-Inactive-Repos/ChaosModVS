using Chaos.Engine.Contracts.Controllers;

namespace Chaos.Engine.Controllers
{
    public class ServerMetaController : IServerMetaController, IMetaController
    {
        public bool DisableEffects { get; set; }
    }
}