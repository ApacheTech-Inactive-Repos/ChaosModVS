using Chaos.Engine.Contracts;
using Chaos.Engine.Controllers;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace Chaos.Engine.API
{
    public class ChaosClientAPI : ChaosAPI, IChaosClientAPI
    {
        public ChaosClientAPI(ICoreAPI api) : base(api)
        {
            Capi = api as ICoreClientAPI;
        }

        public new BlockController.ClientSide Blocks => base.Blocks.Client;
        public new CreatureController.ClientSide Creatures => base.Creatures.Client;
        public new EntityController.ClientSide Entities => base.Entities.Client;
        public new ItemController.ClientSide Items => base.Items.Client;
        public new MetaController.ClientSide Meta => base.Meta.Client;
        public new NetworkController.ClientSide Network => base.Network.Client;
        public new PlayerController.ClientSide Player => base.Player.Client;
        public new ShaderController.ClientSide Shaders => base.Shaders.Client;
        public new TimeController.ClientSide Time => base.Time.Client;
        public new WeatherController.ClientSide Weather => base.Weather.Client;
        public new GameworldController.ClientSide World => base.World.Client;
        public ICoreClientAPI Capi { get; }
    }
}