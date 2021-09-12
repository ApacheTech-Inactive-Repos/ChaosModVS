using Chaos.Engine.Enums;
using Chaos.Engine.Primitives;

namespace Chaos.Mod.Effects.Misc
{
    public class Nothing : ChaosEffect
    {
        public override EffectType EffectType => EffectType.Misc;
        public override EffectDuration Duration => EffectDuration.Instant;
    }
}

namespace Chaos.Mod.Effects.Weather
{
}