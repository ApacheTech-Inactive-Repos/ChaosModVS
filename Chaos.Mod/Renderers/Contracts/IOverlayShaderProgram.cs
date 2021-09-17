using Chaos.Mod.Renderers.Enums;

namespace Chaos.Mod.Renderers.Contracts
{
    public interface IOverlayShaderProgram : IGenericShaderProgram
    {
        float Brightness { get; set; }
        bool Compress { get; set; }
        OverlayColourFilter Filter { get; set; }
        float Intensity { get; set; }
        float Saturation { get; set; }
        float Luminosity { get; set; }
        float Speed { get; set; }
        float Spread { get; set; }

        void UpdateTexture();
    }
}