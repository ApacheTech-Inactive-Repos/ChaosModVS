using Chaos.Engine.Enums;
using Chaos.Engine.Network.Messages;
using Chaos.Engine.Primitives;
using JetBrains.Annotations;
using VintageMods.Core.Extensions;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Chaos.Mod.Effects.Player
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class ThermonuclearHandGrenade : ChaosEffect
    {
        public override EffectType EffectType => EffectType.Player;

        public override EffectDuration Duration => EffectDuration.Instant;

        public override void OnServerStart(IServerPlayer player, ICoreServerAPI sapi)
        {
            base.OnServerStart(player, sapi);
            //TODO: Spawn ore bomb in players hand - 7 second fuse.
            ChaosApi.Server.World.CreateExplosion(new ExplosionData
            {
                Position = player.Entity.Pos.AsBlockPos,
                BlastType = EnumEx.Parse<EnumBlastType>(Settings["BlastType"].AsString("OreBlast")),
                DestructionRadius = Settings["DestructionRadius"].AsDouble(85),
                InjureRadius = Settings["InjureRadius"].AsDouble(85),
                SoundFile = new AssetLocation("chaosmod", "sounds/effects/obliterate"),
                SmokeClouds = Settings["SmokeClouds"].AsBool(true),
                PlayerDamage = Settings["PlayerDamage"].AsFloat(),
                SuppressItemDrops = Settings["SuppressItemDrops"].AsBool(true)
            });
        }
    }
}