using Vintagestory.API.Datastructures;

namespace Chaos.Engine.Contracts
{
    public interface IChaosAPI : IUniversalController<IChaosClientAPI, IChaosServerAPI>
    {
        /// <summary>
        ///     Gets the global configuration options.
        /// </summary>
        /// <value>The settings.</value>
        JsonObject GlobalConfig { get; }
    }
}