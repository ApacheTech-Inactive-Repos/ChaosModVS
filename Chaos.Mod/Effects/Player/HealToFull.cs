using Chaos.Engine.Attributes;
using Chaos.Engine.Enums;
using Chaos.Engine.Primitives;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace Chaos.Mod.Effects.Player
{
    public class HealToFull : ChaosEffect
    {
        public override EffectType EffectType => EffectType.Player;
        public override EffectDuration Duration => EffectDuration.Instant;

        public override void OnServerStart(IServerPlayer player, ICoreServerAPI sapi)
        {
            base.OnServerStart(player, sapi);
            var hunger = player.Entity.GetBehavior<EntityBehaviorHunger>();
            hunger.SaturationLossDelayFruit = 600f;
            hunger.SaturationLossDelayVegetable = 600f;
            hunger.SaturationLossDelayProtein = 600f;
            hunger.SaturationLossDelayGrain = 600f;
            hunger.SaturationLossDelayDairy = 600f;

            hunger.Saturation = hunger.MaxSaturation;

            hunger.VegetableLevel = hunger.MaxSaturation;
            hunger.ProteinLevel = hunger.MaxSaturation;
            hunger.FruitLevel = hunger.MaxSaturation;
            hunger.DairyLevel = hunger.MaxSaturation;
            hunger.GrainLevel = hunger.MaxSaturation;

            var health = player.Entity.GetBehavior<EntityBehaviorHealth>();
            health.Health = health.MaxHealth;
            health.MarkDirty();

        }
    }
}