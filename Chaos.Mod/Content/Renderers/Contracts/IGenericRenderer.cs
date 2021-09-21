using Vintagestory.API.Client;

namespace Chaos.Mod.Content.Renderers.Contracts
{
    public interface IGenericRenderer<TShaderProgram> : IRenderer where TShaderProgram : IGenericShaderProgram
    {
        TShaderProgram Shader { get; set; }

        bool Active { get; set; }
    }
}