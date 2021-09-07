using System;
using System.ComponentModel.Composition;
using Chaos.Engine.API;
using Chaos.Engine.Contracts;
using JetBrains.Annotations;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Chaos.Engine.Primitives
{
    /// <summary>
    ///     Acts as a base class for all Chaos Mod effects.
    /// </summary>
    /// <remarks>
    ///     Effects are run on a separate EffectsRunner thread, and so care must be taken when writing effects, to ensure that
    ///     tasks that need to be run on the MainUI thread, such as sending chat messages, or running OpenGL scripts, must be
    ///     passed to the Main Thread using `ChaosApi.ExecuteOnMainThread(System.Action action, System.String identifier)`.
    /// It's also recommended to set a local Watch on the EffectsRunner thread, when debugging.
    /// </remarks>
    /// <seealso cref="IChaosEffect" />
    /// <seealso cref="IDisposable" />
    public abstract class ChaosEffect : IChaosEffect, IDisposable
    {
        /// <summary>
        ///     Initialises a new instance of the <see cref="ChaosEffect"/> class.
        /// </summary>
        /// <param name="api">The core Chaos Engine API.</param>
        protected ChaosEffect(ICoreAPI api)
        {
            Api = api;
            ChaosApi = new ChaosAPI(Api);
        }

        /// <summary>
        ///     Gets the chaos API.
        /// </summary>
        /// <value>The chaos API.</value>
        protected IChaosAPI ChaosApi { get; private set; }

        /// <summary>
        ///     Gets the core Chaos Engine API.
        /// </summary>
        /// <value>The core Chaos Engine API.</value>
        protected ICoreAPI Api { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether this effect is running.
        /// </summary>
        /// <value><c>true</c> if running; otherwise, <c>false</c>.</value>
        public bool Running { get; protected set; }

        /// <summary>
        ///     Called when the effect is first run. Handles client-side responsibilities.
        /// </summary>
        public virtual void OnClientStart(ICoreClientAPI capi)
        {
            Running = true;
        }

        /// <summary>
        ///     Called when the effect is first run. Handles server-side responsibilities.
        /// </summary>
        /// <param name="player">The player that made the request to the server.</param>
        /// <param name="sapi">The server side core game API.</param>
        public virtual void OnServerStart(IServerPlayer player, ICoreServerAPI sapi)
        {
            Running = true;
        }

        /// <summary>
        ///     Called every client-side game tick.
        /// </summary>
        /// <param name="dt">The time, in milliseconds, between this method call, and the last method call.</param>
        public virtual void OnClientTick(float dt)
        {
            Running = true;
        }

        /// <summary>
        /// Called every server-side game tick.
        /// </summary>
        /// <param name="dt">The time, in milliseconds, between this method call, and the last method call.</param>
        public virtual void OnServerTick(float dt)
        {
            Running = true;
        }

        /// <summary>
        ///     Called when the effect has ended. Handles client-side clean-up responsibilities.
        /// </summary>
        public virtual void OnClientStop()
        {
            Running = false;
        }

        /// <summary>
        ///     Called when the effect has ended. Handles server-side clean-up responsibilities.
        /// </summary>
        public virtual void OnServerStop()
        {
            Running = false;
        }

        #region Implementation of IDisposable Pattern.

        /// <summary>
        ///     Stops the effect, if running, and releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <remarks>
        ///     Base method will automatically stop the effect, if it is running. Take this into consideration when overriding.
        ///     Release unmanaged resources regardless of `disposing` state. Only release managed resources if `disposing` is <c>true</c>.
        /// </remarks>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
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
        ///     Finalises an instance of the <see cref="ChaosEffect"/> class.
        /// </summary>
        ~ChaosEffect()
        {
            Dispose(false);
        }

        #endregion
    }
}