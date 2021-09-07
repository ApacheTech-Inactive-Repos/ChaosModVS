using Chaos.Engine.Primitives;
using Vintagestory.API.Common;

namespace Chaos.Engine.Controllers
{
    public class EntityController : ControllerBase<EntityController.ServerSide, EntityController.ClientSide>
    {
        public EntityController(ICoreAPI api) : base(api)
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