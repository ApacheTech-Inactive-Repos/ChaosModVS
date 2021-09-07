using Chaos.Engine.Primitives;
using Vintagestory.API.Common;

namespace Chaos.Engine.Controllers
{
    public class MetaController : ControllerBase<MetaController.ServerSide, MetaController.ClientSide>
    {
        public MetaController(ICoreAPI api) : base(api)
        {
        }

        public class ClientSide
        {
        }

        public class ServerSide
        {
        }
    }
}