using Chaos.Engine.Effects.Enums;
using Chaos.Mod.Content.Renderers;
using Chaos.Mod.Content.Renderers.Enums;
using Chaos.Mod.Content.Renderers.Primitives;
using Chaos.Mod.Content.Renderers.Shaders;
using Vintagestory.API.Client;

namespace Chaos.Mod.Effects.Shader
{
    public sealed class CatsEyes : ChaosOverlayEffect
    {
        public override EffectType EffectType => EffectType.Shader;
        public override EffectDuration Duration => EffectDuration.Standard;
        protected override string PassName => "colour-overlay";

        protected override void InitialiseRenderer(OverlayRenderer renderer)
        {
            renderer.LightReactive = true;
            renderer.BlendMode = EnumBlendMode.Overlay;
        }

        protected override void InitialiseShader(OverlayShaderProgram shader)
        {
            shader.Filter = OverlayColourFilter.Sepia;
            shader.Intensity = 1f;
            shader.Brightness = 16f;
        }
    }
}