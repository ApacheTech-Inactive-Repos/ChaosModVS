using Chaos.Mod.Renderers.Shaders;
using JetBrains.Annotations;
using Vintagestory.API.Client;

namespace Chaos.Mod.Renderers.Contracts
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public interface IOverlayRenderer : IGenericRenderer<OverlayShaderProgram>
    {
        bool LightReactive { get; set; }
        bool Active { get; set; }
        EnumBlendMode BlendMode { get; set; }
        float BrightnessMultiplier { get; set; }
    }
}