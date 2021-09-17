using Chaos.Engine.Enums;
using Chaos.Mod.Effects.Weather;
using Chaos.Mod.Renderers;
using Chaos.Mod.Renderers.Enums;
using Chaos.Mod.Renderers.Primitives;
using Chaos.Mod.Renderers.Shaders;
using VintageMods.Core.Extensions;
using VintageMods.Core.Reflection;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace Chaos.Mod.Effects.Shader
{
    public sealed class AcidTrip : ChaosOverlayEffect
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
            shader.Filter = OverlayColourFilter.Rainbow;
            shader.Saturation = 0.3f;
            shader.Luminosity = 0.2f;
            shader.Intensity = 2f;
            shader.Brightness = 0f;
            shader.Speed = 0.5f;
            shader.Spread = 4f;
        }

        public override void OnClientTick(float dt)
        {
            ApiEx.Client.Render.ShaderUniforms.GlitchStrength = 1.5f;
            ApiEx.Client.Render.ShaderUniforms.GlobalWorldWarp = 0f;
            base.OnClientTick(dt);
        }
    }
}