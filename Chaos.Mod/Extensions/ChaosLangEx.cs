using Chaos.Engine.Contracts;
using Vintagestory.API.Config;

namespace Chaos.Mod.Extensions
{
    public static class ChaosLangEx
    {
        public static string Effect(IChaosEffectMetadata effect, string section)
        {
            return Lang.Get($"chaosmod:Effects.{effect.Id}.{section}");
        }
    }
}
