using Chaos.Engine.Primitives;
using Vintagestory.API.Common;

namespace Chaos.Engine.Controllers
{
    public class WeatherController : ControllerBase<WeatherController.ServerSide, WeatherController.ClientSide>
    {
        public WeatherController(ICoreAPI api) : base(api)
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