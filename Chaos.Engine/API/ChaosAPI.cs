using System.ComponentModel.Composition;
using Chaos.Engine.Contracts;
using Chaos.Engine.Controllers;
using VintageMods.Core.IO.Extensions;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace Chaos.Engine.API
{
    [Export]
    public class ChaosAPI : IChaosAPI, IHasUniversalAccess
    {
        [Import]
        public ICoreAPI Api { get; }

        [ImportingConstructor]
        public ChaosAPI(ICoreAPI api)
        {
            Api = api;
        }

        public IChaosServerAPI Server => new ChaosServerAPI(Api);
        public IChaosClientAPI Client => new ChaosClientAPI(Api);
        public JsonObject GlobalConfig => Api.GetModFile("global-config.json").AsRawJsonObject();


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