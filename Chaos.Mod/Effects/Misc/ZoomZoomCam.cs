using Chaos.Engine.Attributes;
using Chaos.Engine.Enums;
using Chaos.Engine.Primitives;
using Vintagestory.API.Client;
using Vintagestory.Client.NoObf;

namespace Chaos.Mod.Effects.Misc
{
    [ChaosEffect]
    public class ZoomZoomCam : ChaosEffect
    {
        private int _currentFOV;
        private bool _isIncreasing;
        private int _startFOV;

        public override EffectType EffectType => EffectType.Misc;
        public override EffectDuration Duration => EffectDuration.Short;

        public override void OnClientStart(ICoreClientAPI capi)
        {
            base.OnClientStart(capi);
            _currentFOV = _startFOV = ClientSettings.FieldOfView;
        }

        public override void OnClientTick(float dt)
        {
            base.OnClientTick(dt);
            var fov = _isIncreasing ? ++_currentFOV : --_currentFOV;
            if (fov >= Settings["MaxFieldOfView"].AsInt(150)) _isIncreasing = false;
            if (fov <= Settings["MinFieldOfView"].AsInt(30)) _isIncreasing = true;
            ClientSettings.FieldOfView = fov;
        }

        public override void OnClientStop()
        {
            base.OnClientStop();
            ClientSettings.FieldOfView = _startFOV;
        }
    }
}