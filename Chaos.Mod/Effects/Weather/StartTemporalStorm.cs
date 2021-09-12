using Chaos.Engine.Enums;
using Chaos.Engine.Primitives;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace Chaos.Mod.Effects.Weather
{
    public class StartTemporalStorm : ChaosEffect
    {
        public override EffectType EffectType => EffectType.Weather;
        public override EffectDuration Duration => EffectDuration.Instant;

        public override void OnServerStart(IServerPlayer player, ICoreServerAPI sapi)
        {
            base.OnServerStart(player, sapi);
            sapi.ModLoader.GetModSystem<SystemTemporalStability>()
                .StormData.nextStormTotalDays = sapi.World.Calendar.TotalDays;
        }
    }
}