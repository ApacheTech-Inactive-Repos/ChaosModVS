using Vintagestory.API.Server;

namespace Chaos.Engine.Contracts
{
    public interface IHasServerAccess
    {
        ICoreServerAPI Sapi { get; }
    }
}