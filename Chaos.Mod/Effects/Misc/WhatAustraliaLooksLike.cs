using Chaos.Engine.Effects.Enums;
using Chaos.Engine.Effects.Primitives;
using VintageMods.Core.Extensions;
using VintageMods.Core.Reflection;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;
using Vintagestory.Client.NoObf;

namespace Chaos.Mod.Effects.Misc
{
    public class WhatAustraliaLooksLike : ChaosEffect
    {
        private Camera _camera;

        public override EffectType EffectType => EffectType.Misc;
        public override EffectDuration Duration => EffectDuration.Short;

        public override void OnClientStart(ICoreClientAPI capi)
        {
            base.OnClientStart(capi);
            _camera = capi.AsClientMain().GetField<PlayerCamera>("MainCamera");
            _camera.SetField("upVec3", Vec3Utilsd.FromValues(0.0, -1.0, 0.0));
        }

        public override void OnClientStop()
        {
            _camera.SetField("upVec3", Vec3Utilsd.FromValues(0.0, 1.0, 0.0));
            base.OnClientStop();
        }
    }
}