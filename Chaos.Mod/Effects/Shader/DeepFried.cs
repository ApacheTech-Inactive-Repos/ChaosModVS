using Chaos.Engine.Enums;
using Chaos.Engine.Primitives;
using VintageMods.Core.Extensions;
using Vintagestory.API.Client;
using Vintagestory.Client.NoObf;

namespace Chaos.Mod.Effects.Shader
{
    public sealed class DeepFried : ChaosEffect
    {
        private bool _prevBloom;

        public override EffectType EffectType => EffectType.Shader;
        public override EffectDuration Duration => EffectDuration.Standard;

        public override void OnClientStart(ICoreClientAPI capi)
        {
            base.OnClientStart(capi);
            _prevBloom = ClientSettings.Bloom;
            ClientSettings.BrightnessLevel = 20f;
            ClientSettings.AmbientBloomLevel = 100;
            ClientSettings.Bloom = true;
            ChaosApi.Client.Shaders.ReloadShaders();
        }

        public override void OnClientStop()
        {
            base.OnClientStop();
            ClientSettings.BrightnessLevel = 1f;
            ClientSettings.AmbientBloomLevel = 20;
            ClientSettings.GammaLevel = 2.2f;
            ClientSettings.ExtraGammaLevel = 1f;
            ClientSettings.Bloom = _prevBloom;
            ChaosApi.Client.Shaders.ReloadShaders();
        }
    }
}