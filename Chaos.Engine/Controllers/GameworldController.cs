using System;
using System.Collections.Generic;
using Chaos.Engine.Network.Messages;
using Chaos.Engine.Primitives;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;
using Vintagestory.Server;

namespace Chaos.Engine.Controllers
{
    public class GameworldController : ControllerBase<GameworldController.ServerSide, GameworldController.ClientSide>
    {
        public GameworldController(ICoreAPI api) : base(api)
        {
        }

        public class ClientSide
        {
        }

        public class ServerSide
        {
            public void CreateExplosion(ExplosionData e)
            {
                Native_CreateExplosion(e.Position, e.BlastType, e.DestructionRadius, e.InjureRadius, e.SoundFile);
            }

            private void Native_CreateExplosion(BlockPos pos, EnumBlastType blastType, double destructionRadius,
                double injureRadius, AssetLocation soundFile)
            {
                var server = Api.World as ServerMain;
                destructionRadius = GameMath.Clamp(destructionRadius, 1.0, 16.0);
                var num = Math.Max(destructionRadius, injureRadius);


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

                var cachedCubicShellNormalizedVectors = TryGetCachedCubicShellNormalizedVectors((int)num);

                var num2 = 0.800000011920929 * destructionRadius;
                var num3 = 0.4000000059604645 * destructionRadius;
                var blockPos = new BlockPos();
                var num4 = (int)Math.Ceiling(num);
                var minPos = pos.AddCopy(-num4);
                var maxPos = pos.AddCopy(num4);
                server?.WorldMap.PrefetchBlockAccess.PrefetchBlocks(minPos, maxPos);
                var testSrc = new DamageSource
                {
                    Source = EnumDamageSource.Explosion,
                    SourcePos = pos.ToVec3d(),
                    Type = EnumDamageType.BluntAttack
                };
                var entitiesAround = server?.GetEntitiesAround(pos.ToVec3d(), (float)num + 2f, (float)num + 2f,
                    e => e.ShouldReceiveDamage(testSrc, (float)injureRadius));
                var dictionary = new Dictionary<long, double>();
                var explosionSmokeParticles = new ExplosionSmokeParticles
                {
                    basePos = new Vec3d(pos.X + 0.5, pos.Y + 0.5, pos.Z + 0.5)
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
                        var num6 = injureRadius;
                        var num7 = Math.Max(val, injureRadius);
                        for (var num8 = 0.0; num8 < num7; num8 += 0.25)
                        {
                            blockPos.Set(pos.X + (int)(t.X * num8 + 0.5), pos.Y + (int)(t.Y * num8 + 0.5),
                                pos.Z + (int)(t.Z * num8 + 0.5));
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

                                var blastResistance = block.GetBlastResistance(server, blockPos, t, blastType);
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
                        var damage = entity2.GetBehavior<EntityBehaviorHealth>()?.Health ??
                                     Math.Max(injureRadius / Math.Max(0.5, injureRadius - num9), num9);
                        var damageSource = new DamageSource
                        {
                            Source = EnumDamageSource.Explosion,
                            Type = EnumDamageType.BluntAttack,
                            SourcePos = new Vec3d(pos.X + 0.5, pos.Y, pos.Z + 0.5)
                        };
                        entity2.ReceiveDamage(damageSource, (float)damage);
                    }

                    explosionSmokeParticles.AddBlocks(dictionary2);
                    foreach (var keyValuePair in dictionary2)
                    {
                        keyValuePair.Value.OnBlockExploded(server, keyValuePair.Key, pos, blastType);
                        server.WorldMap.BulkBlockAccess.SetBlock(0, keyValuePair.Key);
                    }

                    server.WorldMap.BulkBlockAccess.Commit();
                    foreach (var keyValuePair2 in dictionary2)
                        server.TriggerNeighbourBlocksUpdate(keyValuePair2.Key);
                }

                server?.PlaySoundAt(soundFile, pos.X, pos.Y, pos.Z, null, false,
                    (float)(24.0 * Math.Pow(destructionRadius, 0.25)));
                server?.SpawnParticles(explosionSmokeParticles);
                var explosionFireParticles = ExplosionParticles.ExplosionFireParticles;
                explosionFireParticles.MinPos.Set(pos.X, pos.Y, pos.Z);
                explosionFireParticles.MinQuantity = 100f;
                server?.SpawnParticles(explosionFireParticles);
                var explosionFireTrailCubicles = ExplosionParticles.ExplosionFireTrailCubicles;
                explosionFireTrailCubicles.Velocity = new[]
                {
                        NatFloat.createUniform(0f, 8f),
                        NatFloat.createUniform(3f, 3f),
                        NatFloat.createUniform(0f, 8f)
                    };
                explosionFireTrailCubicles.basePos.Set(pos.X + 0.5, pos.Y + 0.5, pos.Z + 0.5);
                explosionFireTrailCubicles.GravityEffect = NatFloat.createUniform(0.5f, 0f);
                explosionFireTrailCubicles.LifeLength = NatFloat.createUniform(1.5f, 0.5f);
                server?.SpawnParticles(explosionFireTrailCubicles);
            }
        }
    }
}