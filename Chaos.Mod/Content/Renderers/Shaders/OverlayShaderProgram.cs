using System;
using Chaos.Mod.Content.Renderers.Contracts;
using Chaos.Mod.Content.Renderers.Enums;
using JetBrains.Annotations;
using VintageMods.Core.Extensions;
using VintageMods.Core.Helpers;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;
using Vintagestory.Client.NoObf;
using Action = Vintagestory.API.Common.Action;

namespace Chaos.Mod.Content.Renderers.Shaders
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class OverlayShaderProgram : ShaderProgram, IOverlayShaderProgram
    {
        private readonly ICoreClientAPI _capi;
        
        public OverlayShaderProgram()
        {
            _capi = ApiEx.Client;
            LoadFromFile = true;
            AssetDomain = "chaosmod";
        }

        public void UpdateUniforms()
        {
            BindTexture2D("iChannel0", _capi.Render.FrameBuffers[0].ColorTextureIds[0], 0);
            Uniform("iResolution", ApiEx.Client.ClientWindowSize());
            Uniform("iTime", GameMath.Mod(_capi.InWorldEllapsedMilliseconds, 1000f));

            Uniform("iBrightness", Brightness);
            Uniform("iCompress", Compress ? 1 : 0);
            Uniform("iFilter", (int)Filter);
            Uniform("iIntensity", Intensity);

            Uniform("iLuminosity", Luminosity);
            Uniform("iSaturation", Saturation);
            Uniform("iSpeed", Speed);
            Uniform("iSpread", Spread);
        }

        public OverlayColourFilter Filter { get; set; }

        public bool Compress { get; set; }

        public float Intensity { get; set; }

        public float Saturation { get; set; }

        public float Luminosity { get; set; }

        public float Speed { get; set; }

        public float Spread { get; set; } = 0.1f;

        public float Brightness { get; set; }

        public string Name
        {
            get => PassName;
            set => PassName = value;
        }

        public Action SetAdditionalUniforms { get; set; } = () => { };
    }
}