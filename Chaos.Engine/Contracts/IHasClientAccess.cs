using Vintagestory.API.Client;

namespace Chaos.Engine.Contracts
{
    public interface IHasClientAccess
    {
        ICoreClientAPI Capi { get; }
    }
}