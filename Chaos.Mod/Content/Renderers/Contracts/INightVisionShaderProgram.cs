namespace Chaos.Mod.Content.Renderers.Contracts
{
    public interface INightVisionShaderProgram : IGenericShaderProgram
    {
        float BufferAmplification { get; set; }
        float ElapsedTime { get; set; }
        float IntensityAdjust { get; set; }
        float[] ModelMatrix { get; set; }
        float NoiseAmplification { get; set; }
        float[] ProjectionMatrix { get; set; }
        float[] ViewMatrix { get; set; }

        void UpdateTextures();
    }
}