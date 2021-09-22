using System.Collections.Generic;
using System.Linq;
using Chaos.Engine.Effects.Enums;
using Chaos.Engine.Effects.Primitives;
using HarmonyLib;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

namespace Chaos.Mod.Effects.Block
{

    //TODO: This class need neatening and extending. The basic functionality works, but it's not fully right.

    [HarmonyPatch]
    public class JesusWalkOnWater : ChaosEffect
    {
        private IEnumerable<Vintagestory.API.Common.Block> _blocks;

        public override EffectType EffectType => EffectType.Block;
        public override EffectDuration Duration => EffectDuration.Short;

        public override void OnClientStart(ICoreClientAPI capi)
        {
            _blocks = capi.World.Blocks.Where(p => p.Code?.Path.StartsWith("water-") ?? false);
            MakeSolid();
        }

        public override void OnClientTick(float dt)
        {
            var entity = Player.Entity;

            if (entity.IsEyesSubmerged() || 
                entity.Swimming || 
                entity.Controls.Sneak) 
                MakeLiquid();

            else MakeSolid();
        }

        public override void OnClientStop()
        {
            MakeLiquid();
        }

        private void MakeSolid()
        {
            foreach (var p in _blocks)
            {
                p.CollisionBoxes = new[] { new Cuboidf(new Vec3f(0,0,0), new Vec3f(1,1f,1)) };
                p.Climbable = true;
            }
        }

        private void MakeLiquid()
        {
            foreach (var p in _blocks)
            {
                p.CollisionBoxes = null;
                p.Climbable = false;
            }
        }
    }
}