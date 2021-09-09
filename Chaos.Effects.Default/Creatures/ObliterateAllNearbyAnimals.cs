using System.ComponentModel.Composition;
using System.Dynamic;
using System.Linq;
using Chaos.Engine.Attributes;
using Chaos.Engine.Enums;
using Chaos.Engine.Network.Messages;
using Chaos.Engine.Primitives;
using VintageMods.Core.Extensions;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Chaos.Effects.Default.Creatures
{
    [ChaosEffect]
    public sealed class ObliterateAllNearbyAnimals : ChaosEffect
    {
        public override ExecutionType ExecutionType => ExecutionType.Creature;
        public override EffectDuration Duration => EffectDuration.Instant;

        [ImportingConstructor]
        public ObliterateAllNearbyAnimals(ICoreAPI api) : base(api) { }
        
        public override void OnServerStart(IServerPlayer player, ICoreServerAPI sapi)
        {
            base.OnServerStart(player, sapi);
            var creatures = ChaosApi.Server.Creatures.GetAllNearbyCreatures(player.Entity.Pos.AsBlockPos, 25);
            var blacklist = new [] { "butterfly", "beemob" };

            foreach (var creature in creatures.SkipWhile(p => p.Code.Path.StartsWithAny(blacklist)))
            {
                ChaosApi.Server.World.CreateExplosion(new ExplosionData
                {
                    Position = creature.Pos.AsBlockPos,
                    BlastType = EnumBlastType.OreBlast,
                    DestructionRadius = 6,
                    InjureRadius = 12,
                    SoundFile = new AssetLocation("chaosmod", "sounds/effects/obliterate")
                });
            }
        }
    }
}
