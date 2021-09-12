using System.Collections.Generic;
using Chaos.Engine.Enums;
using JetBrains.Annotations;

namespace Chaos.Engine.Contracts
{
    /// <summary>
    ///     Represents the metadata needed to import a set of effects into the Chaos Engine.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public interface IChaosEffectMetadata
    {
        /// <summary>
        ///     Gets or sets the effect identifier.
        /// </summary>
        /// <value>A slug used to identify this effect to the engine, and to other effects in may be incompatible with.</value>
        string Id { get; }

        /// <summary>
        ///     Gets or sets which package of Effects this particular effect belongs to.
        /// </summary>
        /// <value>The package of Effects this particular effect belongs to.</value>
        string Pack { get; }

        /// <summary>
        /// Gets or sets the execution type of the effect.
        /// </summary>
        /// <value>The execution type of the effect.</value>
        EffectType EffectType { get; }

        /// <summary>
        ///     Gets or sets the length of time an effect is active for.
        /// </summary>
        /// <value>The length of time an effect is active for.</value>
        EffectDuration Duration { get; }

        /// <summary>
        ///     Gets or sets a list of effect ids that this effect is incompatible with. If one of these effects is already running,
        ///     this effect will be removed from the pool until all incompatible effects have ended.
        /// </summary>
        /// <value>An array of strings, identifying effects that may interfere with this effect.</value>
        IEnumerable<string> IncompatibleWith { get; }
    }
}