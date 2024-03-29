﻿using Chaos.Engine.Effects.Enums;
using Chaos.Engine.Effects.Extensions;
using Chaos.Engine.Effects.Primitives;
using Chaos.Engine.Network.Messages;
using JetBrains.Annotations;
using VintageMods.Core.Extensions;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Chaos.Mod.Effects.Creature
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class ObliterateAllNearbyAnimals : ChaosEffect
    {
        public override EffectType EffectType => EffectType.Creature;

        public override EffectDuration Duration => EffectDuration.Instant;

        public override void OnServerStart(IServerPlayer player, ICoreServerAPI sapi)
        {
            base.OnServerStart(player, sapi);
            var blacklist = Settings["Blacklist"].AsArray(new[] {"butterfly", "beemob"});
            var creatures = sapi.World.GetCreaturesAround(player.Entity.Pos.AsBlockPos,
                Settings["ScanRadius"].AsInt(25), blacklist);

            foreach (var creature in creatures)
                sapi.AsServerMain().CustomExplosion(new CustomExplosionData
                {
                    Position = creature.Pos.AsBlockPos,
                    BlastType = EnumEx.Parse<EnumBlastType>(Settings["BlastType"].AsString("OreBlast")),
                    DestructionRadius = Settings["DestructionRadius"].AsDouble(6.0),
                    InjureRadius = Settings["InjureRadius"].AsDouble(12.0),
                    SoundFile = new AssetLocation("chaosmod", "sounds/effects/obliterate"),
                    SmokeClouds = Settings["SmokeClouds"].AsBool(true),
                    PlayerDamage = Settings["PlayerDamage"].AsFloat(5f),
                    SuppressItemDrops = Settings["SuppressItemDrops"].AsBool()
                });
        }
    }
}