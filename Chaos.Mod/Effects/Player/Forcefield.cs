using System;
using System.Linq;
using Chaos.Engine.Enums;
using Chaos.Engine.Primitives;
using VintageMods.Core.Extensions;
using VintageMods.Core.Helpers;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace Chaos.Mod.Effects.Player
{
    public class Forcefield : ChaosEffect
    {
        public override EffectType EffectType => EffectType.Player;
        public override EffectDuration Duration => EffectDuration.Short;

        public override void OnServerStart(IServerPlayer player, ICoreServerAPI sapi)
        {
            ((IServerPlayer)Player).SendMessage("Start");
        }

        public override void OnServerTick(float dt)
        {
            AsyncEx.Server.EnqueueAsyncTask(() =>
            {
                var entities = ChaosApi.Server.Entities.GetAllNearbyEntities(
                    Player.Entity.ServerPos.AsBlockPos, Settings["ScanDistance"].AsInt(15)).ToList();
                foreach (var entity in entities)
                {
                    ApplyForcefieldEffect(entity);
                }
            });
        }

        public override void OnServerStop()
        {
            ApiEx.Server.World.PlaySoundAt(new AssetLocation("sounds/effect/deepbell"), Player.Entity, null, false, 32f, 0.5f);
            ((IServerPlayer)Player).SendMessage("Stopped Forcefield Effect");
            base.OnServerStop();
        }

        private void ApplyForcefieldEffect(Entity entity)
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
            var force = (forceDistance / maxForceDistance) * (maxForce / 10f);

            // Yeet.
            var direction = entityLoc.SubCopy(playerLoc).Normalize();
            entity.ServerPos.Motion.X += direction.X * force;
            entity.ServerPos.Motion.Y += Math.Max(0, direction.Y * force);
            entity.ServerPos.Motion.Z += direction.Z * force;
        }

        private void FromGtaV(Entity entity)
        {

        }
    }

    public static class EntityExtensions
    {
        public static void ApplyForce(this Entity entity, Vec3d direction, float force)
        {
            entity.ServerPos.Motion.X += direction.X * force;
            entity.ServerPos.Motion.Y = direction.Y * force;
            entity.ServerPos.Motion.Z = direction.Z * force;
        }

        public static Vec3d GetNormalisedDirectionTo(this Vec3d from, Vec3d to)
        {
            return to.SubCopy(from).Normalize();
        }

        public static void Repel(this Entity source, Entity entity, float startDistance, float maxForceDistance, float maxForce)
        {
            var entityLoc = entity.ServerPos.XYZ;
            var playerLoc = source.ServerPos.XYZ;
            var distance = entity.ServerPos.DistanceTo(playerLoc);
            if (distance >= startDistance) return;
            var forceVec = playerLoc.GetNormalisedDirectionTo(entityLoc);
            var forceDistance = Math.Min(Math.Max(0f, startDistance - distance), maxForceDistance);
            var force = (forceDistance / maxForceDistance) * (maxForce / 10f);
            ApplyForce(entity, forceVec, (float)force);
        }

        public static void Attract(this Entity source, Entity entity, float startDistance, float maxForceDistance, float maxForce)
        {
            var entityLoc = entity.ServerPos.XYZ;
            var playerLoc = source.ServerPos.XYZ;
            var distance = entity.ServerPos.DistanceTo(playerLoc);
            if (distance >= startDistance) return;
            var forceVec = entityLoc.GetNormalisedDirectionTo(playerLoc);
            var forceDistance = Math.Min(Math.Max(0f, (startDistance - distance)), maxForceDistance);
            var force = (forceDistance / maxForceDistance) * (maxForce / 10f);
            ApplyForce(entity, forceVec, (float)force);
        }
    }
}