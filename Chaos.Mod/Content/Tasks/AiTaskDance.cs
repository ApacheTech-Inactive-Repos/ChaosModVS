using Vintagestory.API.Common;

namespace Chaos.Mod.Content.Tasks
{
    /// <summary>
    ///     Makes an entity dance, by playing the "Dance" animation.
    ///     Implements the <see cref="Vintagestory.API.Common.AiTaskBase" />
    /// </summary>
    /// <seealso cref="Vintagestory.API.Common.AiTaskBase" />
    public class AiTaskDance : AiTaskBase
    {
        /// <summary>
        ///     Initialises a new instance of the <see cref="AiTaskDance" /> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public AiTaskDance(EntityAgent entity) : base(entity)
        {
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="AiTaskDance" /> is active.
        /// </summary>
        /// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
        public bool Active { get; set; }

        /// <summary>
        ///     Determines whether or not the entity should run the task or not.
        /// </summary>
        /// <returns><c>true</c> if the task should be run, <c>false</c> otherwise.</returns>
        public override bool ShouldExecute()
        {
            return Active;
        }

        /// <summary>
        ///     Can be used to spread information to other creatures.
        /// </summary>
        /// <param name="key">The key, denoting the type of information being shared.</param>
        /// <param name="data">The data being passed between entities.</param>
        /// <returns><c>true</c> if the notification applies to this entity, <c>false</c> otherwise.</returns>
        public override bool Notify(string key, object data)
        {
            return false;
        }

        /// <summary>
        ///     Finish the task, and reset, ready to be run again.
        /// </summary>
        /// <param name="cancelled">if set to <c>true</c>, the task was cancelled prematurely.</param>
        public override void FinishExecute(bool cancelled)
        {
            Active = false;
            base.FinishExecute(cancelled);
        }

        /// <summary>
        ///     Called when the state of the entity has changed.
        /// </summary>
        /// <param name="beforeState">The state the entity was at, before the change.</param>
        public override void OnStateChanged(EnumEntityState beforeState)
        {
            if (entity.State == EnumEntityState.Despawned) Active = false;
        }
    }
}