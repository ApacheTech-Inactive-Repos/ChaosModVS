using System.ComponentModel;
using JetBrains.Annotations;

namespace Chaos.Engine.Effects.Enums
{
    /// <summary>
    ///     Determines the length of time an effect is active for.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public enum EffectDuration
    {
        /// <summary>
        ///     Represents an effect that happens instantaneously.
        /// </summary>
        [Description("Instant Effect")] Instant,

        /// <summary>
        ///     Represents an effect that occurs for a short time period (Default: 30 seconds).
        /// </summary>
        [Description("Short Duration Effect")] Short,

        /// <summary>
        ///     Represents an effect that occurs for the standard time period (Default: 60 seconds).
        /// </summary>
        [Description("Standard Duration Effect")]
        Standard,

        /// <summary>
        ///     Represents an effect that occurs for a long time period (Default: 120 seconds).
        /// </summary>
        [Description("Long Duration Effect")] Long,

        /// <summary>
        ///     Represents an effect that, once started, will persist until the effect is manually shut down, or the game session
        ///     ends.
        /// </summary>
        [Description("Permanent Effect")] Permanent,

        /// <summary>
        ///     Represents an effect that occurs for a specified, custom time period (Default: 30 seconds).
        /// </summary>
        [Description("Custom Duration Effect")]
        Custom
    }
}