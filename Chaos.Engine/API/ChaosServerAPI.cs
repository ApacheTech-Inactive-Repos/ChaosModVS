using Chaos.Engine.Contracts;
using Chaos.Engine.Controllers;
using Chaos.Engine.Primitives;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Chaos.Engine.API
{
    public class ChaosServerAPI : ChaosAPI, IChaosServerAPI
    {
        public ChaosServerAPI(ICoreAPI api) : base(api)
        {
            Sapi = api as ICoreServerAPI;
        }

        public ICoreServerAPI Sapi { get; }

        public new BlockController.ServerSide Blocks => base.Blocks.Server;
        public new CreatureController.ServerSide Creatures => base.Creatures.Server;
        public new EntityController.ServerSide Entities => base.Entities.Server;
        public new ItemController.ServerSide Items => base.Items.Server;
        public new MetaController.ServerSide Meta => base.Meta.Server;
        public new NetworkController.ServerSide Network => base.Network.Server;
        public new PlayerController.ServerSide Player => base.Player.Server;
        public new ShaderController.ServerSide Shaders => base.Shaders.Server;
        public new TimeController.ServerSide Time => base.Time.Server;
        public new WeatherController.ServerSide Weather => base.Weather.Server;
        public new GameworldController.ServerSide World => base.World.Server;
    }
}
