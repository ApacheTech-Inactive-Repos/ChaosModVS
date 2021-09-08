using Chaos.Engine.Controllers;

namespace Chaos.Engine.Contracts
{
    public interface IChaosServerAPI : IHasServerAccess
    {
        BlockController.ServerSide Blocks { get; }
        CreatureController.ServerSide Creatures { get; }
        EntityController.ServerSide Entities { get; }
        ItemController.ServerSide Items { get; }
        MetaController.ServerSide Meta { get; }
        NetworkController.ServerSide Network { get; }
        PlayerController.ServerSide Player { get; }
        ShaderController.ServerSide Shaders { get; }
        TimeController.ServerSide Time { get; }
        WeatherController.ServerSide Weather { get; }
        GameworldController.ServerSide World { get; }
    }
}
