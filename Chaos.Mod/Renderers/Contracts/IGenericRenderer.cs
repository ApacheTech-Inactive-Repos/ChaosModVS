using System;
using Vintagestory.API.Client;

namespace Chaos.Mod.Renderers.Contracts
{
    public interface IGenericRenderer<TShaderProgram> : IRenderer where TShaderProgram : IGenericShaderProgram, new()
    {
        TShaderProgram Shader { get; set; }

        bool Active { get; set; }
    }
}