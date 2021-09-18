using System;
using System.Collections.Generic;
using System.Linq;
using Chaos.Engine.Network.Messages;
using JetBrains.Annotations;
using VintageMods.Core.Extensions;
using VintageMods.Core.Helpers;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.Client.NoObf;
using Vintagestory.GameContent;
using Vintagestory.Server;

namespace Chaos.Engine.Effects.Extensions
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class ApiExtensions
    {
        public static void ReloadShadersAsync(this IShaderAPI api)
        {
            AsyncEx.Client.EnqueueMainThreadTask(() => api.ReloadShaders());
        }

        public static void RevertGfxToDefault(this ClientSettings settings)
        {
            ClientSettings.BrightnessLevel = 1f;
            ClientSettings.AmbientBloomLevel = 20;
            ClientSettings.GammaLevel = 2.2f;
            ClientSettings.ExtraGammaLevel = 1f;
            ClientSettings.Bloom = false;
            ApiEx.Client.Shader.ReloadShadersAsync();
        }

        public static IEnumerable<EntityAgent> GetCreaturesAround(this IWorldAccessor world, BlockPos pos, int radius,
            string[] blacklist = null)
        {
            return world.GetEntitiesAround(pos.ToVec3d(), radius, radius, e =>
            {
                if (e.Code.Path.ContainsAny(blacklist ?? new string[] { })) return false;
                return e is EntityAgent {Alive: true} and not EntityPlayer;
            }).Cast<EntityAgent>();
        }

        public static IEnumerable<EntityAgent> GetCorpsesAround(this IWorldAccessor world, BlockPos pos, int radius,
            string[] blacklist = null)
        {
            return world.GetEntitiesAround(pos.ToVec3d(), radius, radius, e =>
            {
                if (e.Code.Path.ContainsAny(blacklist ?? new string[] { })) return false;
                return e is EntityAgent {Alive: false};
            }).Cast<EntityAgent>();
        }

        public static IEnumerable<Entity> GetEntitiesAround(this IWorldAccessor world, BlockPos pos, int radius,
            string[] blacklist = null)
        {
            return world.GetEntitiesAround(pos.ToVec3d(), radius, radius, e =>
            {
                if (e.Code.Path.ContainsAny(blacklist ?? new string[] { })) return false;
                return e is EntityPlayer {Alive: true};
            });
        }

        public static void CustomExplosion(this ServerMain server, CustomExplosionData data)
        {
            //data.DestructionRadius = GameMath.Clamp(data.DestructionRadius, 1.0, 16.0);
            var maxInitialDamage = Math.Max(data.DestructionRadius, data.InjureRadius);


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

            var cachedCubicShellNormalizedVectors = TryGetCachedCubicShellNormalizedVectors((int) maxInitialDamage);

            var initialBlockSplashDamage = 0.800000011920929 * data.DestructionRadius;
            var initialEntitySplashDamage = 0.4000000059604645 * data.DestructionRadius;
            var blockPos = new BlockPos();
            var maxDamage = (int) Math.Ceiling(maxInitialDamage);
            var minPos = data.Position.AddCopy(-maxDamage);
            var maxPos = data.Position.AddCopy(maxDamage);
            server?.WorldMap.PrefetchBlockAccess.PrefetchBlocks(minPos, maxPos);
            var testSrc = new DamageSource
            {
                Source = EnumDamageSource.Explosion,
                SourcePos = data.Position.ToVec3d(),
                Type = EnumDamageType.BluntAttack
            };
            var entitiesAround = server?.GetEntitiesAround(data.Position.ToVec3d(), (float) maxInitialDamage + 2f,
                (float) maxInitialDamage + 2f,
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
                    double blockDamage;
                    var val = blockDamage = initialBlockSplashDamage +
                                            server.rand.Value.NextDouble() * initialEntitySplashDamage;
                    var entityDamage = data.InjureRadius;
                    var num7 = Math.Max(val, data.InjureRadius);
                    for (var num8 = 0.0; num8 < num7; num8 += 0.25)
                    {
                        blockPos.Set(data.Position.X + (int) (t.X * num8 + 0.5),
                            data.Position.Y + (int) (t.Y * num8 + 0.5),
                            data.Position.Z + (int) (t.Z * num8 + 0.5));
                        blockDamage -= 0.25;
                        entityDamage -= 0.25;
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

                            if (block.Code.Path == "air") continue;

                            var blastResistance = block.GetBlastResistance(server, blockPos, t, data.BlastType);
                            blockDamage -= blastResistance;
                            switch (blockDamage)
                            {
                                case > 0.0:
                                    dictionary2[blockPos.Copy()] = block;
                                    entityDamage -= blastResistance;
                                    break;
                                case <= 0.0 when blastResistance > 0.0:
                                    entityDamage = 0.0;
                                    break;
                            }
                        }

                        if (blockDamage <= 0.0 && entityDamage <= 0.0) break;

                        if (!(entityDamage > 0.0)) continue;
                        foreach (var entity in entitiesAround)
                        {
                            cuboid.Set(blockPos.X, blockPos.Y, blockPos.Z, blockPos.X + 1, blockPos.Y + 1,
                                blockPos.Z + 1);
                            if (cuboid.IntersectsOrTouches(entity.CollisionBox, entity.ServerPos.X,
                                entity.ServerPos.Y,
                                entity.ServerPos.Z))
                                dictionary[entity.EntityId] = Math.Max(dictionary[entity.EntityId], entityDamage);
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
                    try
                    {
                        if (data.SuppressItemDrops) keyValuePair.Value.Drops = new BlockDropItemStack[] { };
                        keyValuePair.Value.OnBlockExploded(server, keyValuePair.Key, data.Position, data.BlastType);
                        server.WorldMap.BulkBlockAccess.SetBlock(0, keyValuePair.Key);
                    }
                    catch (NullReferenceException e)
                    {
                        // ignored
                    }
                    catch (Exception e)
                    {
                        // ignored
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