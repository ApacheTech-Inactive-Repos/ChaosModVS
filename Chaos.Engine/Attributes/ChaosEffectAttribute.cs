using System;
using System.ComponentModel.Composition;
using Chaos.Engine.Contracts;

namespace Chaos.Engine.Attributes
{
    /// <summary>
    ///     Specifies that this class is an Effect that will be imported into the Chaos Engine, to be used in-game.
    /// 
    ///     Inherits from the <see cref="ExportAttribute" /> class.
    /// </summary>
    /// <seealso cref="ExportAttribute" />
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ChaosEffectAttribute : ExportAttribute
    {
        /// <summary>
        ///     Initialises a new instance of the <see cref="ChaosEffectAttribute" /> class.
        /// </summary>
        public ChaosEffectAttribute() : base("ChaosEffects", typeof(IChaosEffect))
        {
        }
    }
}