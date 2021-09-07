using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Chaos.Engine.Contracts;
using Chaos.Engine.Enums;

namespace Chaos.Engine.Attributes
{
    /// <summary>
    ///     Specifies that this class is an Effect that will be imported into the Chaos Engine, to be used in-game.
    /// 
    ///     Inherits from the <see cref="ExportAttribute" /> class.
    ///     Implements the <see cref="IChaosEffectMetadata" /> interface.
    /// </summary>
    /// <seealso cref="ExportAttribute" />
    /// <seealso cref="IChaosEffectMetadata" />
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ChaosEffectAttribute : ExportAttribute, IChaosEffectMetadata
    {
        /// <summary>
        ///     Initialises a new instance of the <see cref="ChaosEffectAttribute" /> class.
        /// </summary>
        public ChaosEffectAttribute() : base("ChaosEffects", typeof(IChaosEffect))
        {
            Id = "Unknown";
            Pack = "Default";
            ExecutionType = ExecutionType.Misc;
            Duration = EffectDuration.Standard;
        }

        /// <summary>
        ///     Gets or sets the effect identifier.
        /// </summary>
        /// <value>A slug used to identify this effect to the engine, and to other effects in may be incompatible with.</value>
        public string Id { get; set; }

        /// <summary>
        ///     Gets or sets which package of Effects this particular effect belongs to.
        /// </summary>
        /// <value>The package of Effects this particular effect belongs to.</value>
        public string Pack { get; set; }

        /// <summary>
        ///     Gets or sets the execution type of the effect.
        /// </summary>
        /// <value>The execution type of the effect.</value>
        public ExecutionType ExecutionType { get; set; }

        /// <summary>
        ///     Gets or sets the length of time an effect is active for.
        /// </summary>
        /// <value>The length of time an effect is active for.</value>
        public EffectDuration Duration { get; set; }

        /// <summary>
        ///     Gets or sets a list of effect ids that this effect is incompatible with. If one of these effects is already running,
        ///     this effect will be removed from the pool until all incompatible effects have ended.
        /// </summary>
        /// <value>An array of strings, identifying effects that may interfere with this effect.</value>
        public virtual IEnumerable<string> IncompatibleWith { get; set; }
    }
}