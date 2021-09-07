using Vintagestory.API.Common;

namespace Chaos.Engine.Contracts
{
    public interface IHasUniversalAccess
    {
        ICoreAPI Api { get; }
    }
}