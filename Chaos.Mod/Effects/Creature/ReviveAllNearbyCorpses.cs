using Chaos.Engine.Effects.Enums;
using Chaos.Engine.Effects.Extensions;
using Chaos.Engine.Effects.Primitives;
using JetBrains.Annotations;
using Vintagestory.API.Server;

namespace Chaos.Mod.Effects.Creature
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class ReviveAllNearbyCorpses : ChaosEffect
    {
        public override EffectType EffectType => EffectType.Creature;
        public override EffectDuration Duration => EffectDuration.Instant;

        public override void OnServerStart(IServerPlayer player, ICoreServerAPI sapi)
        {
            base.OnServerStart(player, sapi);
            var blacklist = Settings["Blacklist"].AsArray(new[] {"butterfly", "beemob"});
            var corpses = sapi.World.GetCorpsesAround(
                Player.Entity.Pos.AsBlockPos, Settings["ScanRadius"].AsInt(25), blacklist);
            foreach (var corpse in corpses) corpse.Revive();
        }
    }
}