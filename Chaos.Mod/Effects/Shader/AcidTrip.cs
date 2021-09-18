using System;
using Chaos.Engine.Effects.Enums;
using Chaos.Mod.Content.Renderers;
using Chaos.Mod.Content.Renderers.Enums;
using Chaos.Mod.Content.Renderers.Primitives;
using Chaos.Mod.Content.Renderers.Shaders;
using VintageMods.Core.Helpers;
using Vintagestory.API.Client;
using Vintagestory.Client.NoObf;

namespace Chaos.Mod.Effects.Shader
{
    public sealed class AcidTrip : ChaosOverlayEffect
    {
        private int _currentFOV;
        private bool _isIncreasing;
        private Random _rand;
        private int _startFOV;

        public override EffectType EffectType => EffectType.Shader;
        public override EffectDuration Duration => EffectDuration.Short;

        protected override string PassName => "colour-overlay";

        protected override void InitialiseRenderer(OverlayRenderer renderer)
        {
            renderer.LightReactive = false;
            renderer.BlendMode = EnumBlendMode.Overlay;
        }

        protected override void InitialiseShader(OverlayShaderProgram shader)
        {
            shader.Filter = OverlayColourFilter.Rainbow;
            shader.Saturation = 0.3f;
            shader.Luminosity = 0.2f;
            shader.Intensity = 2f;
            shader.Brightness = 0f;
            shader.Speed = 0.5f;
            shader.Spread = 4f;
        }

        public override void OnClientSetup(ICoreClientAPI capi)
        {
            _currentFOV = _startFOV = ClientSettings.FieldOfView;
            _rand = new Random();
        }

        public override void OnClientTick(float dt)
        {
            if (Settings["GlitchEnabled"].AsBool(true))
                ApiEx.Client.Render.ShaderUniforms.GlitchStrength = Settings["GlitchStrength"].AsFloat(1.5f);

            if (Settings["DynamicFOVEnabled"].AsBool(true))
                ClientSettings.FieldOfView = ++_currentFOV;
        }

        public override void OnClientTakeDown(ICoreClientAPI capi)
        {
            ApiEx.Client.Render.ShaderUniforms.GlitchStrength = 0f;
            ClientSettings.FieldOfView = _startFOV;
        }
    }
}