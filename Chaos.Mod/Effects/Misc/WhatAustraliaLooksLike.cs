using Chaos.Engine.Effects.Enums;
using Chaos.Engine.Effects.Primitives;
using Vintagestory.API.Client;
using Vintagestory.Client.NoObf;

namespace Chaos.Mod.Effects.Misc
{
    public class WhatAustraliaLooksLike : ChaosEffect
    {
        private Camera _camera;
        private int _prevFOV;

        public override EffectType EffectType => EffectType.Misc;
        public override EffectDuration Duration => EffectDuration.Short;

        public override void OnClientStart(ICoreClientAPI capi)
        {
            //_camera = capi.AsClientMain().GetField<PlayerCamera>("MainCamera");
            //_camera.SetField("upVec3", Vec3Utilsd.FromValues(0.0, -1.0, 0.0));

            _prevFOV = ClientSettings.FieldOfView;
            ClientSettings.FieldOfView += 180;
        }

        public override void OnClientStop()
        {
            //_camera.SetField("upVec3", Vec3Utilsd.FromValues(0.0, 1.0, 0.0));
            ClientSettings.FieldOfView = _prevFOV;
        }
    }
}