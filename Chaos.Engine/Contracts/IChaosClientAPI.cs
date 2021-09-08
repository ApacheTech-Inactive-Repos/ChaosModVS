using Chaos.Engine.Controllers;

namespace Chaos.Engine.Contracts
{
    public interface IChaosClientAPI : IHasClientAccess
    {
        BlockController.ClientSide Blocks { get; }
        CreatureController.ClientSide Creatures { get; }
        EntityController.ClientSide Entities { get; }
        ItemController.ClientSide Items { get; }
        MetaController.ClientSide Meta { get; }
        NetworkController.ClientSide Network { get; }
        PlayerController.ClientSide Player { get; }
        ShaderController.ClientSide Shaders { get; }
        TimeController.ClientSide Time { get; }
        WeatherController.ClientSide Weather { get; }
        GameworldController.ClientSide World { get; }

    }
}
