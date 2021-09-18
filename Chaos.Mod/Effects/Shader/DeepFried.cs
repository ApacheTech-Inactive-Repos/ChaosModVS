using Chaos.Engine.Effects.Enums;
using Chaos.Engine.Effects.Extensions;
using Chaos.Engine.Effects.Primitives;
using VintageMods.Core.Helpers;
using Vintagestory.API.Client;
using Vintagestory.Client.NoObf;

namespace Chaos.Mod.Effects.Shader
{
    public sealed class DeepFried : ChaosEffect
    {
        private bool _prevBloom;

        public override EffectType EffectType => EffectType.Shader;
        public override EffectDuration Duration => EffectDuration.Short;

        public override void OnClientStart(ICoreClientAPI capi)
        {
            base.OnClientStart(capi);
            _prevBloom = ClientSettings.Bloom;
            ClientSettings.BrightnessLevel = 20f;
            ClientSettings.AmbientBloomLevel = 100;
            ClientSettings.Bloom = true;
            capi.Shader.ReloadShadersAsync();
        }

        public override void OnClientStop()
        {
            base.OnClientStop();
            ClientSettings.BrightnessLevel = 1f;
            ClientSettings.AmbientBloomLevel = 20;
            ClientSettings.GammaLevel = 2.2f;
            ClientSettings.ExtraGammaLevel = 1f;
            ClientSettings.Bloom = _prevBloom;
            ApiEx.Client.Shader.ReloadShadersAsync();
        }
    }
}