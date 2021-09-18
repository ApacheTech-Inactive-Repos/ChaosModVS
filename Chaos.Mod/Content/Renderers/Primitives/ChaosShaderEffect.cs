using Chaos.Engine.Effects.Extensions;
using Chaos.Engine.Effects.Primitives;
using Chaos.Mod.Content.Renderers.Contracts;
using VintageMods.Core.Helpers;
using Vintagestory.API.Client;

namespace Chaos.Mod.Content.Renderers.Primitives
{
    public abstract class ChaosShaderEffect<TRenderer, TShaderProgram> : ChaosEffect
        where TRenderer : class, IGenericRenderer<TShaderProgram>, new()
        where TShaderProgram : class, IGenericShaderProgram, new()
    {
        protected TRenderer Renderer { get; set; }
        protected TShaderProgram Shader { get; set; }
        protected abstract string PassName { get; }

        public override void OnClientStart(ICoreClientAPI capi)
        {
            capi.Event.EnqueueMainThreadTask(() =>
            {
                Shader ??= new TShaderProgram();
                Shader.Name = PassName;
                InitialiseShader(Shader);

                Renderer ??= new TRenderer();
                InitialiseRenderer(Renderer);
                Renderer.Shader = Shader;

                capi.Event.RegisterRenderer(Renderer, EnumRenderStage.AfterFinalComposition);
                capi.Event.ReloadShader += LoadShader;
                LoadShader();
                Renderer.Active = true;
            }, "");
        }

        public override void OnClientStop()
        {
            var capi = ApiEx.Client;
            capi.Event.EnqueueMainThreadTask(() =>
            {
                Renderer.Active = false;
                capi.Event.ReloadShader -= LoadShader;
                capi.Event.UnregisterRenderer(Renderer, EnumRenderStage.AfterFinalComposition);
                capi.Shader.ReloadShadersAsync();
                Renderer = null;
                Shader = null;
            }, "");
            base.OnClientStop();
        }

        protected abstract void InitialiseRenderer(TRenderer renderer);
        protected abstract void InitialiseShader(TShaderProgram shader);

        protected virtual void SetAdditionalUniforms(TShaderProgram shader)
        {
        }

        private bool LoadShader()
        {
            ApiEx.Client.Shader.RegisterFileShaderProgram(PassName, Shader);
            Shader.Compile();
            if (Renderer != null) Renderer.Shader = Shader;
            return true;
        }
    }
}