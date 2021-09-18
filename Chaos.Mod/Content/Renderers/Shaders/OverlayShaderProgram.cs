using Chaos.Mod.Content.Renderers.Contracts;
using Chaos.Mod.Content.Renderers.Enums;
using JetBrains.Annotations;
using VintageMods.Core.Helpers;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.Client.NoObf;

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

        public void UpdateTexture()
        {
            BindTexture2D("primaryScene", _capi.Render.FrameBuffers[0].ColorTextureIds[0], 0);
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