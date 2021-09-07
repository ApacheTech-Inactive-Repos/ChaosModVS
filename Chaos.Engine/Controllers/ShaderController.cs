using Chaos.Engine.Primitives;
using Vintagestory.API.Common;

namespace Chaos.Engine.Controllers
{
    public class ShaderController : ControllerBase<ShaderController.ServerSide, ShaderController.ClientSide>
    {
        public ShaderController(ICoreAPI api) : base(api)
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