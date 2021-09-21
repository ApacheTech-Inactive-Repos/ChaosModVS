using Chaos.Mod.Content.Renderers.Shaders;
using JetBrains.Annotations;
using Vintagestory.API.Client;

namespace Chaos.Mod.Content.Renderers.Contracts
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public interface IOverlayRenderer : IGenericRenderer<OverlayShaderProgram>
    {
        bool LightReactive { get; set; }
        EnumBlendMode BlendMode { get; set; }
        float BrightnessMultiplier { get; set; }
    }
}