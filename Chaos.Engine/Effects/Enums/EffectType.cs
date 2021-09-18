using System.ComponentModel;
using JetBrains.Annotations;

namespace Chaos.Engine.Effects.Enums
{
    /// <summary>
    ///     Determines the execution type of an effect.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public enum EffectType
    {
        /// <summary>
        ///     Effects that spawn, despawn, replace, or otherwise change blocks.
        /// </summary>
        [Description("Effects that spawn, despawn, replace, or otherwise change blocks.")]
        Block,

        /// <summary>
        ///     Effects that spawn, despawn, replace, or otherwise change creatures or NPCs.
        /// </summary>
        [Description("Effects that spawn, despawn, replace, or otherwise change creatures or NPCs.")]
        Creature,

        /// <summary>
        ///     Effects that spawn, despawn, replace, or otherwise change items.
        /// </summary>
        [Description("Effects that spawn, despawn, replace, or otherwise change items.")]
        Item,

        /// <summary>
        ///     Effects that affect other effects.
        /// </summary>
        [Description("Effects that affect other effects.")]
        Meta,

        /// <summary>
        ///     Effects that control or manipulate player actions, inventory, clothing, location, etc.
        /// </summary>
        [Description("Effects that control or manipulate player actions, inventory, clothing, location, etc.")]
        Player,

        /// <summary>
        ///     Effects that change how the game looks and feels.
        /// </summary>
        [Description("Effects that change how the game looks and feels.")]
        Shader,

        /// <summary>
        ///     Effects that manipulate the game time, game speed, day/night cycle, or other timed events.
        /// </summary>
        [Description("Effects that manipulate the game time, game speed, day/night cycle, or other timed events.")]
        Time,

        /// <summary>
        ///     Effects that manipulate the weather, temperature, or season.
        /// </summary>
        [Description("Effects that manipulate the weather, temperature, or season.")]
        Weather,

        /// <summary>
        ///     Effects that don't fall under any other category.
        /// </summary>
        [Description("Effects that don't fall under any other category.")]
        Misc
    }
}