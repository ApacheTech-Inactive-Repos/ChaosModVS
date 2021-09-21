using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace Chaos.Mod.Content.Renderers.Contracts
{
    public interface IGenericShaderProgram : IShaderProgram
    {
        string Name { get; set; }
        void UpdateUniforms();
    }
}