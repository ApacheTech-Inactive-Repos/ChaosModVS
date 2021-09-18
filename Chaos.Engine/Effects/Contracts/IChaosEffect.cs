using JetBrains.Annotations;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;

namespace Chaos.Engine.Effects.Contracts
{
    /// <summary>
    ///     Represents a static effect for the ChaosMod VS Chaos Engine. These are effects that fire once, and last for as long
    ///     as the duration specifies before being turned off.
    ///     They are fire-and-forget, and require no callbacks each game tick.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public interface IChaosEffect : IChaosEffectMetadata
    {
        /// <summary>
        ///     Gets a value indicating whether this effect is running.
        /// </summary>
        /// <value><c>true</c> if running; otherwise, <c>false</c>.</value>
        bool Running { get; set; }

        /// <summary>
        ///     Gets or sets the global settings for the mod.
        /// </summary>
        /// <value>The global settings.</value>
        JsonObject GlobalConfig { get; set; }

        /// <summary>
        ///     Gets the settings for the effect.
        /// </summary>
        /// <value>The settings.</value>
        JsonObject Settings { get; set; }

        /// <summary>
        ///     The player that acts as the target for the effect.
        /// </summary>
        /// <value>A side-agnostic interface of the target Player.</value>
        IPlayer Player { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating which game tick the effect started on.
        /// </summary>
        /// <value><c>true</c> if [start tick]; otherwise, <c>false</c>.</value>
        public long StartTick { get; set; }

        void OnClientSetup(ICoreClientAPI capi);

        /// <summary>
        ///     Called when the effect is first run. Handles client-side responsibilities.
        /// </summary>
        void OnClientStart(ICoreClientAPI capi);

        /// <summary>
        ///     Called when the effect is first run. Handles server-side responsibilities.
        /// </summary>
        /// <param name="player">The player that made the request to the server.</param>
        /// <param name="sapi">The server side core game API.</param>
        void OnServerStart(IServerPlayer player, ICoreServerAPI sapi);

        /// <summary>
        ///     Called every client-side game tick.
        /// </summary>
        void OnClientTick(float dt);

        /// <summary>
        ///     Called every server-side game tick.
        /// </summary>
        void OnServerTick(float dt);

        /// <summary>
        ///     Called when the effect has ended. Handles client-side clean-up responsibilities.
        /// </summary>
        void OnClientStop();

        void OnClientTakeDown(ICoreClientAPI capi);

        void OnServerTakeDown(IServerPlayer player, ICoreServerAPI sapi);

        /// <summary>
        ///     Called when the effect has ended. Handles server-side clean-up responsibilities.
        /// </summary>
        void OnServerStop();

        /// <summary>
        ///     Disposes this instance, stopping the effect on both server, and client.
        /// </summary>
        void Dispose();

        void OnServerSetup(IServerPlayer player, ICoreServerAPI sapi);
    }
}