using Chaos.Engine.Effects.Enums;
using Chaos.Engine.Effects.Primitives;

namespace Chaos.Mod.Effects.Misc
{
    public class Nothing : ChaosEffect
    {
        public override EffectType EffectType => EffectType.Misc;
        public override EffectDuration Duration => EffectDuration.Instant;
    }
}