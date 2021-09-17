using Chaos.Engine.Contracts.Controllers;
using Chaos.Engine.Primitives;

namespace Chaos.Engine.Controllers
{
    public class ClientMetaController : IClientMetaController, IMetaController
    {
        public bool DisableEffects { get; set; }
    }
}