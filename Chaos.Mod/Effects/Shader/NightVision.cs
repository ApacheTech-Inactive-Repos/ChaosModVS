using Chaos.Engine.Effects.Enums;
using Chaos.Mod.Content.Renderers;
using Chaos.Mod.Content.Renderers.Enums;
using Chaos.Mod.Content.Renderers.Primitives;
using Chaos.Mod.Content.Renderers.Shaders;
using Vintagestory.API.Client;

namespace Chaos.Mod.Effects.Shader
{
    public sealed class NightVision : ChaosOverlayEffect
    {
        public override EffectType EffectType => EffectType.Shader;
        public override EffectDuration Duration => EffectDuration.Standard;
        protected override string PassName => "colour-overlay";

        protected override void InitialiseRenderer(OverlayRenderer renderer)
        {
            renderer.LightReactive = true;
            renderer.BlendMode = EnumBlendMode.Overlay;
            renderer.BrightnessMultiplier = 2f;
        }

        protected override void InitialiseShader(OverlayShaderProgram shader)
        {
            shader.Filter = OverlayColourFilter.Green;
            shader.Intensity = 2f;
            shader.Brightness = 8f;
        }
    }
}