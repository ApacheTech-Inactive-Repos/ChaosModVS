using System;
using Chaos.Engine.Enums;
using Chaos.Mod.Renderers;
using Chaos.Mod.Renderers.Enums;
using Chaos.Mod.Renderers.Primitives;
using Chaos.Mod.Renderers.Shaders;
using Vintagestory.API.Client;

namespace Chaos.Mod.Effects.Shader
{
    public sealed class AlienWorld : ChaosOverlayEffect
    {
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
            shader.Filter = new Random().NextDouble() > 0.5 
                ? OverlayColourFilter.Sepia 
                : OverlayColourFilter.Greyscale;
            shader.Intensity = 5f;
            shader.Brightness = 0.5f;
        }
    }
}