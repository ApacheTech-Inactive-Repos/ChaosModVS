using Chaos.Engine.Controllers;
using Vintagestory.API.Datastructures;

namespace Chaos.Engine.Contracts
{
    public interface IChaosAPI
    {
        IChaosServerAPI Server { get; }
        IChaosClientAPI Client { get; }
        BlockController Blocks { get; }
        CreatureController Creatures { get; }
        EntityController Entities { get; }
        ItemController Items { get; }
        MetaController Meta { get; }
        NetworkController Network { get; }
        PlayerController Player { get; }
        ShaderController Shaders { get; }
        TimeController Time { get; }
        WeatherController Weather { get; }
        GameworldController World { get; }

        /// <summary>
        ///     Gets the global configuration options.
        /// </summary>
        /// <value>The settings.</value>
        JsonObject GlobalConfig { get; }
    }
}