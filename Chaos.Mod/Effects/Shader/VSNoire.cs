using Chaos.Engine.Effects.Enums;
using Chaos.Mod.Content.Renderers;
using Chaos.Mod.Content.Renderers.Enums;
using Chaos.Mod.Content.Renderers.Primitives;
using Chaos.Mod.Content.Renderers.Shaders;
using VintageMods.Core.Helpers;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.Client.NoObf;

namespace Chaos.Mod.Effects.Shader
{
    public sealed class VSNoire : ChaosOverlayEffect
    {
        private int _userMusicVolume;
        private ILoadedSound _jazzMusic;
        public override EffectType EffectType => EffectType.Shader;
        public override EffectDuration Duration => EffectDuration.Standard;
        protected override string PassName => "colour-overlay";

        protected override void InitialiseRenderer(OverlayRenderer renderer)
        {
            renderer.LightReactive = false;
            renderer.BlendMode = EnumBlendMode.Overlay;
        }

        protected override void InitialiseShader(OverlayShaderProgram shader)
        {
            shader.Filter = OverlayColourFilter.Greyscale;
            shader.Brightness = 0f;
            shader.Intensity = 1f;
        }

        public override void OnClientSetup(ICoreClientAPI capi)
        {
            capi.Event.EnqueueMainThreadTask(() =>
            {
                _jazzMusic = capi.World.LoadSound(new SoundParams
                {
                    Location = new AssetLocation("chaosmod", "sounds/music/vsnoire.ogg"),
                    ShouldLoop = false,
                    SoundType = EnumSoundType.Sound,
                    RelativePosition = true,
                    Position = Vec3f.Zero,
                    Range = 32f,
                    DisposeOnFinish = true,
                    Volume = 0.2f
                });
            }, "");

            (_userMusicVolume, ClientSettings.MusicLevel) = (ClientSettings.MusicLevel, _userMusicVolume);
            capi.CurrentMusicTrack?.FadeOut(2f);
        }

        public override void OnClientStart(ICoreClientAPI capi)
        {
            capi.Event.EnqueueMainThreadTask(() =>
            {
                _jazzMusic.Start();
            }, "");
            base.OnClientStart(capi);
        }

        public override void OnClientStop()
        {
            ApiEx.Client.Event.EnqueueMainThreadTask(() =>
            {
                _jazzMusic.Stop();
            }, "");
            base.OnClientStop();
        }

        public override void OnClientTakeDown(ICoreClientAPI capi)
        {
            (_userMusicVolume, ClientSettings.MusicLevel) = (ClientSettings.MusicLevel, _userMusicVolume);
        }
    }
}