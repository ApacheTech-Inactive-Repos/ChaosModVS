using System;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace Chaos.Mod.Renderers
{
    public class FilteredRenderer : IRenderer
    {
        public EnumNightVisionMode Mode { get; set; }

        public float NightVisionBrightness { get; set; }

        public IShaderProgram Shader { get; internal set; }

        public double RenderOrder => 0.85;

        public int RenderRange => 1;

        public FilteredRenderer(ICoreClientAPI capi, IShaderProgram shader)
        {
            _capi = capi;
            Shader = shader;
            var customQuadModelData = QuadMeshUtil.GetCustomQuadModelData(-1f, -1f, 0f, 2f, 2f);
            customQuadModelData.Rgba = null;
            _quadRef = capi.Render.UploadMesh(customQuadModelData);
            _nightVisionIntensity = 0f;
            Mode = EnumNightVisionMode.Default;
            NightVisionBrightness = 0f;
            _brightness = 8f;
        }

        public void Dispose()
        {
            _capi.Render.DeleteMesh(_quadRef);
            Shader.Dispose();
        }

        public void OnRenderFrame(float deltaTime, EnumRenderStage stage)
        {
            var entity = _capi.World.Player.Entity;
            if (entity == null)
            {
                return;
            }
            var currentActiveShader = _capi.Render.CurrentActiveShader;
            currentActiveShader?.Stop();
            const float num = 2f;
            deltaTime = Math.Min(deltaTime, num);
            var num2 = _capi.World.BlockAccessor.GetLightLevel(entity.Pos.AsBlockPos, EnumLightLevelType.MaxTimeOfDayLight);
            var lightHsv = entity.LightHsv;
            if (lightHsv != null)
            {
                num2 = lightHsv.Aggregate(num2, (current, t) => Math.Max(current, t));
            }
            var num3 = (num2 == 0) ? 1.6f : 1f;
            var num4 = (1f - num2 / 16f) * num3;
            _nightVisionIntensity = (_nightVisionIntensity * (num - deltaTime) + num4 * deltaTime) / num;
            if (_nightVisionIntensity > 0f && (Mode & EnumNightVisionMode.Deactivated) == EnumNightVisionMode.FilterNone)
            {
                Shader.Use();
                _capi.Render.GlToggleBlend(true, EnumBlendMode.Overlay);
                _capi.Render.GLDisableDepthTest();
                Shader.BindTexture2D("primaryScene", _capi.Render.FrameBuffers[0].ColorTextureIds[0], 0);
                Shader.Uniform("intensity", _nightVisionIntensity);
                Shader.Uniform("brightness", NightVisionBrightness + _brightness);
                _capi.Render.RenderMesh(_quadRef);
                Shader.Stop();
            }

            currentActiveShader?.Use();
        }

        private readonly MeshRef _quadRef;
        private readonly ICoreClientAPI _capi;
        private float _nightVisionIntensity;
        private readonly float _brightness;
    }
}
