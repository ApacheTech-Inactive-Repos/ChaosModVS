using System;
using System.Linq;
using Chaos.Mod.Content.Renderers.Contracts;
using Chaos.Mod.Content.Renderers.Enums;
using Chaos.Mod.Content.Renderers.Shaders;
using JetBrains.Annotations;
using VintageMods.Core.Extensions;
using VintageMods.Core.Helpers;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace Chaos.Mod.Content.Renderers
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class OverlayRenderer : IOverlayRenderer
    {
        private readonly ICoreClientAPI _capi;

        private readonly MeshRef _quadRef;
        

        public OverlayRenderer()
        {
            _capi = ApiEx.Client;
            Active = false;
            var customQuadModelData = QuadMeshUtil.GetCustomQuadModelData(-1f, -1f, 0f, 2f, 2f);
            customQuadModelData.Rgba = null;
            _quadRef = _capi.Render.UploadMesh(customQuadModelData);
        }

        public EnumBlendMode BlendMode { get; set; } = EnumBlendMode.Overlay;

        public OverlayShaderProgram Shader { get; set; }

        public float BrightnessMultiplier { get; set; } = 1f;

        public bool Active { get; set; }

        public void OnRenderFrame(float dt, EnumRenderStage stage)
        {
            if (!Active) return;
            var currentActiveShader = _capi.Render.CurrentActiveShader;
            currentActiveShader?.Stop();

            var entity = _capi.World.Player.Entity;

            if (LightReactive)
            {
                const float num = 3f;
                dt = Math.Min(dt, num);
                var lightLevel =
                    _capi.World.BlockAccessor.GetLightLevel(entity.Pos.AsBlockPos,
                        EnumLightLevelType.MaxTimeOfDayLight);
                var lightHsv = entity.LightHsv;

                if (lightHsv != null) lightLevel = lightHsv.Aggregate(lightLevel, (current, t) => Math.Max(current, t));

                var pitchBlackMultiplier = lightLevel == 0 ? BrightnessMultiplier * 1.6f : BrightnessMultiplier * 1f;

                var scaleFactor = (1f - lightLevel / 16f) * pitchBlackMultiplier;
                Shader.Intensity = (Shader.Intensity * (num - dt) + scaleFactor * dt) / num;
                //Shader.Brightness = ((Shader.Brightness * (num - dt) + scaleFactor * dt) / num);
            }

            if (Shader.Intensity > 0)
            {
                Shader.Use();
                _capi.Render.GlToggleBlend(true, BlendMode);
                _capi.Render.GLDisableDepthTest();
                Shader.UpdateUniforms();
                _capi.Render.RenderMesh(_quadRef);
                Shader.Stop();
            }

            currentActiveShader?.Use();
        }

        public bool LightReactive { get; set; }

        public void Dispose()
        {
            _quadRef?.Dispose();
            Shader?.Dispose();
        }

        public double RenderOrder => 0.85;

        public int RenderRange => 1;
    }
}