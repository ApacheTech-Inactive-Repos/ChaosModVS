using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace Chaos.Mod.Renderers.Contracts
{
    public interface IGenericShaderProgram : IShaderProgram
    {
        string Name { get; set; }

        Action SetAdditionalUniforms { get; set; }
    }
}