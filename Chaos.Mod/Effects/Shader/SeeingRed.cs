using Chaos.Engine.Enums;
using Chaos.Mod.Renderers;
using Chaos.Mod.Renderers.Enums;
using Chaos.Mod.Renderers.Primitives;
using Chaos.Mod.Renderers.Shaders;
using Vintagestory.API.Client;

namespace Chaos.Mod.Effects.Shader
{
    public sealed class SeeingRed : ChaosOverlayEffect
    {
        public override EffectType EffectType => EffectType.Shader;
        public override EffectDuration Duration => EffectDuration.Standard;
        protected override string PassName => "colour-overlay";

        protected override void InitialiseRenderer(OverlayRenderer renderer)
        {
            renderer.LightReactive = false;
            renderer.BlendMode = EnumBlendMode.Overlay;
        }

        protected override void InitialiseShader(OverlayShaderProgram shader)
        {
            shader.Filter = OverlayColourFilter.Red;
            shader.Brightness = 1f;
            shader.Intensity = 1f;
        }
    }
}