using Chaos.Engine.Contracts;
using Chaos.Engine.Controllers;
using Vintagestory.API.Common;

namespace Chaos.Engine.API
{
    public class ChaosAPI : IChaosAPI, IHasUniversalAccess
    {
        public ICoreAPI Api { get; }

        public ChaosAPI(ICoreAPI api)
        {
            Api = api;
        }

        public IChaosServerAPI Server => new ChaosServerAPI(Api);
        public IChaosClientAPI Client => new ChaosClientAPI(Api);


        public PlayerController Player => new(Api);
        public CreatureController Creatures => new(Api);
        public GameworldController World => new(Api);
        public BlockController Blocks => new(Api);
        public ItemController Items => new(Api);
        public EntityController Entities => new(Api);
        public TimeController Time => new(Api);
        public WeatherController Weather => new(Api);
        public MetaController Meta => new(Api);
        public ShaderController Shaders => new(Api);
        public NetworkController Network => new(Api);
    }
}