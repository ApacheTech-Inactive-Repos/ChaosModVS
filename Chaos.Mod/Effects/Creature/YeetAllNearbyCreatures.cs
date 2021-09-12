using Chaos.Engine.Enums;
using Chaos.Engine.Primitives;
using Vintagestory.API.Server;

namespace Chaos.Mod.Effects.Creature
{
    public class YeetAllNearbyCreatures : ChaosEffect
    {
        public override EffectType EffectType => EffectType.Creature;
        public override EffectDuration Duration => EffectDuration.Instant;

        public override void OnServerStart(IServerPlayer player, ICoreServerAPI sapi)
        {
            base.OnServerStart(player, sapi);
            var blacklist = Settings["Blacklist"].AsArray(new[] {"butterfly", "beemob"});
            var creatures = ChaosApi.Server.Creatures.GetAllNearbyCreatures(
                Player.Entity.Pos.AsBlockPos, Settings["ScanRadius"].AsInt(25), blacklist);
            foreach (var creature in creatures) creature.ServerPos.Motion.Y = 2.5;
        }
    }
}