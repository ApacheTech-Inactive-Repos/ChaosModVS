using Chaos.Engine.Effects.Enums;
using Chaos.Engine.Effects.Primitives;
using Vintagestory.API.Client;
using Vintagestory.GameContent;

namespace Chaos.Mod.Effects.Player
{
    public class Yeet : ChaosEffect
    {
        public override EffectType EffectType => EffectType.Player;
        public override EffectDuration Duration => EffectDuration.Instant;

        public override void OnClientStart(ICoreClientAPI capi)
        {
            capi.World.Player.Entity.Pos.Motion.Y = 2.5;
        }
    }
}