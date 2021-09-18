using Chaos.Engine.Effects.Enums;
using Chaos.Engine.Effects.Primitives;
using Vintagestory.API.Client;
using Vintagestory.Client.NoObf;

namespace Chaos.Mod.Effects.Misc
{
    public class QuakeProFOV : ChaosEffect
    {
        private int _prevFOV;

        public override EffectType EffectType => EffectType.Misc;
        public override EffectDuration Duration => EffectDuration.Standard;

        public override void OnClientStart(ICoreClientAPI capi)
        {
            base.OnClientStart(capi);
            _prevFOV = ClientSettings.FieldOfView;
            ClientSettings.FieldOfView = Settings["FieldOfView"].AsInt(110);
        }

        public override void OnClientStop()
        {
            base.OnClientStop();
            ClientSettings.FieldOfView = _prevFOV;
        }
    }
}