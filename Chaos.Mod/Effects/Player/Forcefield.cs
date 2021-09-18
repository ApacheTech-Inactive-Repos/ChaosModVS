using System;
using System.Linq;
using Chaos.Engine.Effects.Enums;
using Chaos.Engine.Effects.Extensions;
using Chaos.Engine.Effects.Primitives;
using VintageMods.Core.Helpers;

namespace Chaos.Mod.Effects.Player
{
    public class Forcefield : ChaosEffect
    {
        public override EffectType EffectType => EffectType.Player;
        public override EffectDuration Duration => EffectDuration.Long;

        public override void OnServerTick(float dt)
        {
            AsyncEx.Server.EnqueueAsyncTask(() =>
            {
                var entities = ApiEx.Server.World.GetEntitiesAround(
                    Player.Entity.ServerPos.AsBlockPos, Settings["ScanDistance"].AsInt(15)).ToList();
                foreach (var entity in entities)
                {
                    // Inputs.
                    var startDistance = Settings["ForcefieldRadius"].AsInt(15);
                    var maxForce = Settings["MaxForce"].AsFloat(10f);

                    // Positions.
                    var entityLoc = entity.ServerPos.XYZ;
                    var playerLoc = Player.Entity.ServerPos.XYZ;
                    playerLoc.Y += Player.Entity.LocalEyePos.Y;

                    // Distance.
                    var distance = entity.ServerPos.DistanceTo(playerLoc);
                    if (distance >= startDistance) return;

                    // Force.
                    var maxForceDistance = startDistance * 0.67;
                    var forceDistance = Math.Min(Math.Max(0f, startDistance - distance), maxForceDistance);
                    var force = forceDistance / maxForceDistance * (maxForce / 10f);

                    // Yeet.
                    var direction = entityLoc.SubCopy(playerLoc).Normalize();
                    entity.ServerPos.Motion.X += direction.X * force;
                    entity.ServerPos.Motion.Y += Math.Max(0, direction.Y * force);
                    entity.ServerPos.Motion.Z += direction.Z * force;
                }
            });
        }
    }
}