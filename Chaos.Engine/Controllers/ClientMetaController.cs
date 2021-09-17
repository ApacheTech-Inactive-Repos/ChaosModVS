using Chaos.Engine.Contracts.Controllers;

namespace Chaos.Engine.Controllers
{
    public class ClientMetaController : IClientMetaController, IMetaController
    {
        public bool DisableEffects { get; set; }
    }
}