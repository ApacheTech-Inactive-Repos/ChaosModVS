using Chaos.Engine.Primitives;
using Vintagestory.API.Common;

namespace Chaos.Engine.Controllers
{
    public class PlayerController : ControllerBase<PlayerController.ServerSide, PlayerController.ClientSide>
    {
        public PlayerController(ICoreAPI api) : base(api)
        {

        }

        public class ServerSide
        {

        }

        public class ClientSide
        {

        }
    }
}