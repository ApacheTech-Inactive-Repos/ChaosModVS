using Chaos.Engine.Contracts.Controllers;
using VintageMods.Core.Extensions;
using VintageMods.Core.Helpers;
using Vintagestory.Client.NoObf;

namespace Chaos.Engine.Controllers
{
    public class ClientShaderController : IClientShaderController, IShaderController
    {
        public void ReloadShaders()
        {
            AsyncEx.Client.EnqueueMainThreadTask(() => ApiEx.Client.Shader.ReloadShaders());
        }

        public void RevertGfxToDefault()
        {
            ClientSettings.BrightnessLevel = 1f;
            ClientSettings.AmbientBloomLevel = 20;
            ClientSettings.GammaLevel = 2.2f;
            ClientSettings.ExtraGammaLevel = 1f;
            ClientSettings.Bloom = false;
            ReloadShaders();
        }
    }
}