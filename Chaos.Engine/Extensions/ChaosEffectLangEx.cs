using Chaos.Engine.Contracts;
using Vintagestory.API.Config;

namespace Chaos.Engine.Extensions
{
    /// <summary>
    ///     Contains extension methods for retrieving localised strings, for displaying Chaos Effect information.
    /// </summary>
    public static class ChaosEffectLangEx
    {
        /// <summary>
        ///     Gets the lang entry for given effect and section, returns the key itself it the entry does not exist.
        /// </summary>
        /// <param name="effect">The Effect to get information from.</param>
        /// <param name="section">The specific section to get the entry for.</param>
        /// <returns></returns>
        private static string Effect(IChaosEffectMetadata effect, string section)
        {
            return Lang.Get($"chaosmod:Effects.{effect.Pack}.{effect.ExecutionType}.{effect.Id}.{section}");
        }

        /// <summary>
        ///     Gets the localised string for the Title of the specified effect.
        /// </summary>
        /// <param name="effect">The Effect to get information from.</param>
        /// <returns></returns>
        public static string Title(this IChaosEffectMetadata effect) => Effect(effect, "Title");

        /// <summary>
        ///     Gets the localised string for the Description of the specified effect.
        /// </summary>
        /// <param name="effect">The Effect to get information from.</param>
        /// <returns></returns>
        public static string Description(this IChaosEffectMetadata effect) => Effect(effect, "Description");
    }
}