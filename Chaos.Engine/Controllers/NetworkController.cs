using Chaos.Engine.Primitives;
using Vintagestory.API.Common;

namespace Chaos.Engine.Controllers
{
    public class NetworkController : ControllerBase<NetworkController.ServerSide, NetworkController.ClientSide>
    {
        public NetworkController(ICoreAPI api) : base(api)
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