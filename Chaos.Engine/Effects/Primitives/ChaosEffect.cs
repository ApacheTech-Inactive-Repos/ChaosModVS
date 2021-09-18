using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Chaos.Engine.Effects.Contracts;
using Chaos.Engine.Effects.Enums;
using JetBrains.Annotations;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;

namespace Chaos.Engine.Effects.Primitives
{
    /// <summary>
    ///     Acts as a base class for all Chaos Mod effects.
    /// </summary>
    /// <remarks>
    ///     Effects are run on a separate thread, and so care must be taken when writing effects, to ensure that tasks
    ///     that need to be run on the MainUI thread, such as sending chat messages, or running OpenGL scripts, must be
    ///     passed to the Main Thread using `NetworkEx.Client|Server.EnqueueMainThreadTask(System.Action action)`.
    /// </remarks>
    /// <seealso cref="IChaosEffect" />
    /// <seealso cref="IDisposable" />
    [UsedImplicitly(ImplicitUseTargetFlags.WithInheritors | ImplicitUseTargetFlags.WithMembers)]
    public abstract class ChaosEffect : IChaosEffect, IDisposable, IComparable<ChaosEffect>
    {
        /// <summary>
        ///     Initialises a new instance of the <see cref="ChaosEffect" /> class.
        /// </summary>
        protected ChaosEffect()
        {
            Id = GetType().Name;
            LoadSettings();
        }

        #region Implementation of IComparable Pattern.

        /// <summary>
        ///     Compares the current instance with another object of the same type and returns
        ///     an integer that indicates whether the current instance precedes, follows, or
        ///     occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(ChaosEffect other)
        {
            return
                this == other ? 0 :
                other is null ? 1 :
                string.Compare(Id, other.Id, StringComparison.Ordinal);
        }

        #endregion

        #region Private Methods

        private void LoadSettings()
        {
            var rootFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var configFolder = new DirectoryInfo(Path.Combine(rootFolder!, "assets", "chaosmod", "config"));
            var file = Path.Combine(configFolder.FullName, "effects", Pack.ToLowerInvariant(), "effect-settings.json");
            Settings = JsonObject.FromJson(File.ReadAllText(file))[$"{EffectType}"][Id];
        }

        #endregion

        #region Metadata

        /// <summary>
        ///     Gets the player object.
        /// </summary>
        /// <value>The player.</value>
        public IPlayer Player { get; set; }

        /// <summary>
        ///     Gets or sets the global settings for the mod.
        /// </summary>
        /// <value>The global settings.</value>
        public JsonObject GlobalConfig { get; set; }

        /// <summary>
        ///     Gets the settings for the effect.
        /// </summary>
        /// <value>The settings.</value>
        public JsonObject Settings { get; set; }

        /// <summary>
        ///     Gets a value indicating whether this effect is running.
        /// </summary>
        /// <value><c>true</c> if running; otherwise, <c>false</c>.</value>
        public bool Running { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating which game tick the effect started on.
        /// </summary>
        /// <value><c>true</c> if [start tick]; otherwise, <c>false</c>.</value>
        public long StartTick { get; set; }

        #endregion

        #region Client Methods

        /// <summary>
        ///     Called when the effect is first run. Handles client-side responsibilities.
        /// </summary>
        public virtual void OnClientSetup(ICoreClientAPI capi)
        {
        }

        /// <summary>
        ///     Called when the effect start. Handles client-side responsibilities.
        /// </summary>
        public virtual void OnClientStart(ICoreClientAPI capi)
        {
        }

        /// <summary>
        ///     Called every client-side game tick.
        /// </summary>
        /// <param name="dt">The time, in milliseconds, between this method call, and the last method call.</param>
        public virtual void OnClientTick(float dt)
        {
        }

        /// <summary>
        ///     Called when the effect has ended. Handles client-side clean-up responsibilities.
        /// </summary>
        public virtual void OnClientStop()
        {
        }

        /// <summary>
        ///     Called when the effect is first run. Handles client-side responsibilities.
        /// </summary>
        public virtual void OnClientTakeDown(ICoreClientAPI capi)
        {
        }

        #endregion

        #region Server Methods

        /// <summary>
        ///     Called when the effect is first run. Handles server-side responsibilities.
        /// </summary>
        /// <param name="player">The player that made the request to the server.</param>
        /// <param name="sapi">The server side core game API.</param>
        public virtual void OnServerSetup(IServerPlayer player, ICoreServerAPI sapi)
        {
        }

        /// <summary>
        ///     Called when the effect is first run. Handles server-side responsibilities.
        /// </summary>
        /// <param name="player">The player that made the request to the server.</param>
        /// <param name="sapi">The server side core game API.</param>
        public virtual void OnServerStart(IServerPlayer player, ICoreServerAPI sapi)
        {
        }

        /// <summary>
        ///     Called every server-side game tick.
        /// </summary>
        /// <param name="dt">The time, in milliseconds, between this method call, and the last method call.</param>
        public virtual void OnServerTick(float dt)
        {
        }

        /// <summary>
        ///     Called when the effect has ended. Handles server-side clean-up responsibilities.
        /// </summary>
        public virtual void OnServerStop()
        {
        }


        /// <summary>
        ///     Called when the effect ends. Handles server-side responsibilities.
        /// </summary>
        /// <param name="player">The player that made the request to the server.</param>
        /// <param name="sapi">The server side core game API.</param>
        public virtual void OnServerTakeDown(IServerPlayer player, ICoreServerAPI sapi)
        {
        }

        #endregion

        #region Implementation of IDisposable Pattern.

        /// <summary>
        ///     Stops the effect, if running, and releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <remarks>
        ///     Base method will automatically stop the effect, if it is running. Take this into consideration when overriding.
        ///     Release unmanaged resources regardless of `disposing` state. Only release managed resources if `disposing` is
        ///     <c>true</c>.
        /// </remarks>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!Running) return;
            OnClientStop();
            OnServerStop();
        }

        /// <summary>
        ///     Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Finalises an instance of the <see cref="ChaosEffect" /> class.
        /// </summary>
        ~ChaosEffect()
        {
            Dispose(false);
        }

        #endregion

        #region Implementation of IChaosEffectMetadata.

        /// <summary>
        ///     Gets or sets the effect identifier.
        /// </summary>
        /// <value>A slug used to identify this effect to the engine, and to other effects in may be incompatible with.</value>
        public string Id { get; }

        /// <summary>
        ///     Gets or sets the execution type of the effect.
        /// </summary>
        /// <value>The execution type of the effect.</value>
        public abstract EffectType EffectType { get; }

        /// <summary>
        ///     Gets or sets the length of time an effect is active for.
        /// </summary>
        /// <value>The length of time an effect is active for.</value>
        public abstract EffectDuration Duration { get; }

        /// <summary>
        ///     Gets or sets which package of Effects this particular effect belongs to.
        /// </summary>
        /// <value>The package of Effects this particular effect belongs to.</value>
        public virtual string Pack => "Default";

        /// <summary>
        ///     Gets or sets a list of effect ids that this effect is incompatible with. If one of these effects is already
        ///     running,
        ///     this effect will be removed from the pool until all incompatible effects have ended.
        /// </summary>
        /// <value>An array of strings, identifying effects that may interfere with this effect.</value>
        public virtual IEnumerable<string> IncompatibleWith => new string[] { };

        #endregion
    }
}