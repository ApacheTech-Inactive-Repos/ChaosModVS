using System;
using System.Collections.Generic;
using Chaos.Engine.Contracts.Controllers;
using Chaos.Engine.Network.Messages;
using VintageMods.Core.Extensions;
using VintageMods.Core.Helpers;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace Chaos.Engine.Controllers
{
    public class ServerGameworldController : IServerGameworldController, IGameworldController
    {
        public void CreateExplosion(ExplosionData e)
        {
            Native_CreateExplosion(e);
        }

        private static void Native_CreateExplosion(ExplosionData data)
        {
            var server = ApiEx.Server.AsServerMain();
            //data.DestructionRadius = GameMath.Clamp(data.DestructionRadius, 1.0, 16.0);
            var num = Math.Max(data.DestructionRadius, data.InjureRadius);


            static Vec3f[] TryGetCachedCubicShellNormalizedVectors(int n)
            {
                try
                {
                    return ShapeUtil.GetCachedCubicShellNormalizedVectors(n);
                }
                catch
                {
                    return TryGetCachedCubicShellNormalizedVectors(--n);
                }
            }

            var cachedCubicShellNormalizedVectors = TryGetCachedCubicShellNormalizedVectors((int) num);

            var num2 = 0.800000011920929 * data.DestructionRadius;
            var num3 = 0.4000000059604645 * data.DestructionRadius;
            var blockPos = new BlockPos();
            var num4 = (int) Math.Ceiling(num);
            var minPos = data.Position.AddCopy(-num4);
            var maxPos = data.Position.AddCopy(num4);
            server?.WorldMap.PrefetchBlockAccess.PrefetchBlocks(minPos, maxPos);
            var testSrc = new DamageSource
            {
                Source = EnumDamageSource.Explosion,
                SourcePos = data.Position.ToVec3d(),
                Type = EnumDamageType.BluntAttack
            };
            var entitiesAround = server?.GetEntitiesAround(data.Position.ToVec3d(), (float) num + 2f, (float) num + 2f,
                e => e.ShouldReceiveDamage(testSrc, (float) data.InjureRadius));
            var dictionary = new Dictionary<long, double>();
            var explosionSmokeParticles = new ExplosionSmokeParticles
            {
                basePos = new Vec3d(data.Position.X + 0.5, data.Position.Y + 0.5, data.Position.Z + 0.5)
            };
            if (entitiesAround != null)
            {
                foreach (var t in entitiesAround) dictionary[t.EntityId] = 0.0;

                var dictionary2 = new Dictionary<BlockPos, Block>();
                var cuboid = Block.DefaultCollisionBox.ToDouble();
                foreach (var t in cachedCubicShellNormalizedVectors)
                {
                    double num5;
                    var val = num5 = num2 + server.rand.Value.NextDouble() * num3;
                    var num6 = data.InjureRadius;
                    var num7 = Math.Max(val, data.InjureRadius);
                    for (var num8 = 0.0; num8 < num7; num8 += 0.25)
                    {
                        blockPos.Set(data.Position.X + (int) (t.X * num8 + 0.5),
                            data.Position.Y + (int) (t.Y * num8 + 0.5),
                            data.Position.Z + (int) (t.Z * num8 + 0.5));
                        num5 -= 0.25;
                        num6 -= 0.25;
                        if (!dictionary2.ContainsKey(blockPos))
                        {
                            Block block;
                            try
                            {
                                block = server.WorldMap.PrefetchBlockAccess.GetBlock(blockPos);
                            }
                            catch
                            {
                                block = server.WorldMap.BulkBlockAccess.GetBlock(blockPos);
                            }

                            var blastResistance = block.GetBlastResistance(server, blockPos, t, data.BlastType);
                            num5 -= blastResistance;
                            switch (num5)
                            {
                                case > 0.0:
                                    dictionary2[blockPos.Copy()] = block;
                                    num6 -= blastResistance;
                                    break;
                                case <= 0.0 when blastResistance > 0.0:
                                    num6 = 0.0;
                                    break;
                            }
                        }

                        if (num5 <= 0.0 && num6 <= 0.0) break;

                        if (!(num6 > 0.0)) continue;
                        foreach (var entity in entitiesAround)
                        {
                            cuboid.Set(blockPos.X, blockPos.Y, blockPos.Z, blockPos.X + 1, blockPos.Y + 1,
                                blockPos.Z + 1);
                            if (cuboid.IntersectsOrTouches(entity.CollisionBox, entity.ServerPos.X,
                                entity.ServerPos.Y,
                                entity.ServerPos.Z))
                                dictionary[entity.EntityId] = Math.Max(dictionary[entity.EntityId], num6);
                        }
                    }
                }

                foreach (var entity2 in entitiesAround)
                {
                    var num9 = dictionary[entity2.EntityId];
                    if (num9 == 0) continue;

                    var damage = entity2 is EntityPlayer
                        ? data.PlayerDamage
                        : entity2.GetBehavior<EntityBehaviorHealth>()?.Health ??
                          Math.Max(data.InjureRadius / Math.Max(0.5, data.InjureRadius - num9), num9);

                    var damageSource = new DamageSource
                    {
                        Source = EnumDamageSource.Explosion,
                        Type = EnumDamageType.BluntAttack,
                        SourcePos = new Vec3d(data.Position.X + 0.5, data.Position.Y, data.Position.Z + 0.5)
                    };
                    entity2.ReceiveDamage(damageSource, (float) damage);
                }

                explosionSmokeParticles.AddBlocks(dictionary2);
                foreach (var keyValuePair in dictionary2)
                {
                    if (data.SuppressItemDrops) keyValuePair.Value.Drops = null;
                    keyValuePair.Value.OnBlockExploded(server, keyValuePair.Key, data.Position, data.BlastType);
                    server.WorldMap.BulkBlockAccess.SetBlock(0, keyValuePair.Key);
                }

                server.WorldMap.BulkBlockAccess.Commit();
                foreach (var keyValuePair2 in dictionary2)
                    server.TriggerNeighbourBlocksUpdate(keyValuePair2.Key);
            }

            server?.PlaySoundAt(data.SoundFile, data.Position.X, data.Position.Y, data.Position.Z, null, false,
                (float) (24.0 * Math.Pow(data.DestructionRadius, 0.25)));
            if (data.SmokeClouds) server?.SpawnParticles(explosionSmokeParticles);
            var explosionFireParticles = ExplosionParticles.ExplosionFireParticles;
            explosionFireParticles.MinPos.Set(data.Position.X, data.Position.Y, data.Position.Z);
            explosionFireParticles.MinQuantity = 100f;
            server?.SpawnParticles(explosionFireParticles);
            var explosionFireTrailCubicles = ExplosionParticles.ExplosionFireTrailCubicles;
            explosionFireTrailCubicles.Velocity = new[]
            {
                NatFloat.createUniform(0f, 8f),
                NatFloat.createUniform(3f, 3f),
                NatFloat.createUniform(0f, 8f)
            };
            explosionFireTrailCubicles.basePos.Set(data.Position.X + 0.5, data.Position.Y + 0.5, data.Position.Z + 0.5);
            explosionFireTrailCubicles.GravityEffect = NatFloat.createUniform(0.5f, 0f);
            explosionFireTrailCubicles.LifeLength = NatFloat.createUniform(1.5f, 0.5f);
            server?.SpawnParticles(explosionFireTrailCubicles);
        }
    }
}