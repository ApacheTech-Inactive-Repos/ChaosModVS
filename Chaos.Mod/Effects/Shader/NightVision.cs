using Chaos.Engine.Enums;
using Chaos.Engine.Primitives;
using Chaos.Mod.Renderers;
using VintageMods.Core.Extensions;
using Vintagestory.API.Client;

// ReSharper disable StringLiteralTypo

namespace Chaos.Mod.Effects.Shader
{
    public sealed class NightVision : ChaosEffect
    {
        private IShaderProgram _nightVisionShaderProg;
        private FilteredRenderer _nightVisionRenderer;


        public override EffectType EffectType => EffectType.Shader;
        public override EffectDuration Duration => EffectDuration.Standard;

        public override void OnClientStart(ICoreClientAPI capi)
        {
            base.OnClientStart(capi);
            Api.ForClient().Event.EnqueueMainThreadTask(() =>
            {
                _nightVisionRenderer ??= new FilteredRenderer(capi, _nightVisionShaderProg)
                {
                    Mode = EnumNightVisionMode.FilterSepia,
                    NightVisionBrightness = 8
                };
                capi.Event.RegisterRenderer(_nightVisionRenderer, EnumRenderStage.AfterFinalComposition);
                capi.Event.ReloadShader += LoadShader;
                LoadShader();
            }, "");
        }

        public override void OnClientStop()
		{
			Api.ForClient().Event.EnqueueMainThreadTask(() =>
            {
                Api.ForClient().Event.ReloadShader -= LoadShader;
                Api.ForClient().Event.UnregisterRenderer(_nightVisionRenderer, EnumRenderStage.AfterFinalComposition);
                Api.ForClient().Shader.ReloadShaders();
            }, "");
            base.OnClientStop();
        }

        public bool LoadShader()
        {
            var capi = Api.ForClient();
			_nightVisionShaderProg = capi.Shader.NewShaderProgram();
			_nightVisionShaderProg.VertexShader = capi.Shader.NewShader(EnumShaderType.VertexShader);
			_nightVisionShaderProg.FragmentShader = capi.Shader.NewShader(EnumShaderType.FragmentShader);
			_nightVisionShaderProg.VertexShader.Code = GetVertexShaderCode();
            _nightVisionShaderProg.FragmentShader.Code = GetFragmentShaderCode(_nightVisionRenderer?.Mode ?? EnumNightVisionMode.Default);
            capi.Shader.RegisterMemoryShaderProgram("nightvision", _nightVisionShaderProg);
			_nightVisionShaderProg.Compile();
			if (_nightVisionRenderer != null)
			{
				_nightVisionRenderer.Shader = _nightVisionShaderProg;
			}
			return true;
		}
		
        private string GetVertexShaderCode()
		{
			return @"
                #version 330 core
                #extension GL_ARB_explicit_attrib_location: enable
                layout(location = 0) in vec3 vertex;

                out vec2 uv;

                void main(void)
                {
                    gl_Position = vec4(vertex.xy, 0, 1);
                    uv = (vertex.xy + 1.0) / 2.0;
                }";
		}
		
        private string GetFragmentShaderCode(EnumNightVisionMode mode)
		{
			var str = "#version 330 core\r\n";
			if ((mode & EnumNightVisionMode.FilterGray) > EnumNightVisionMode.FilterNone)
			{
				str += "#define GRAY 1\r\n";
			}
			if ((mode & EnumNightVisionMode.FilterSepia) > EnumNightVisionMode.FilterNone)
			{
				str += "#define SEPIA 1\r\n";
			}
			if ((mode & EnumNightVisionMode.FilterGreen) > EnumNightVisionMode.FilterNone)
			{
				str += "#define GREEN 1\r\n";
			}
			if ((mode & EnumNightVisionMode.FilterRed) > EnumNightVisionMode.FilterNone)
			{
				str += "#define RED 1\r\n";
			}
			if ((mode & EnumNightVisionMode.FilterBlue) > EnumNightVisionMode.FilterNone)
			{
				str += "#define BLUE 1\r\n";
			}
			if ((mode & EnumNightVisionMode.Compress) > EnumNightVisionMode.FilterNone)
			{
				str += "#define COMPRESS 1\r\n";
			}
			return str + @"
                uniform sampler2D primaryScene;
                uniform float intensity;
                uniform float brightness;
                in vec2 uv;
                out vec4 outColor;
                void main () {

                    vec4 color = texture(primaryScene, uv);
                    //default mix with sepia, optional gray
	                #if GRAY
                        vec3 mixColor = vec3(dot(color.rgb, vec3(0.2126, 0.7152, 0.0722)));
                    #elif SEPIA
	                    vec3 mixColor = vec3(
		                    (color.r * 0.393) + (color.g * 0.769) + (color.b * 0.189),
		                    (color.r * 0.349) + (color.g * 0.686) + (color.b * 0.168),
		                    (color.r * 0.272) + (color.g * 0.534) + (color.b * 0.131));
                    #elif GREEN
	                    vec3 mixColor = vec3(
		                    0.0f,
		                    (color.r * 0.2126) + (color.g * 0.7152) + (color.b * 0.0722),
		                    0.0f);
                    #elif RED
	                    vec3 mixColor = vec3(
		                    (color.r * 0.2126) + (color.g * 0.7152) + (color.b * 0.0722),
		                    0.0f,
		                    0.0f);
                    #elif BLUE
	                    vec3 mixColor = vec3(
		                    0.0f,
		                    0.0f,		                    
                            (color.r * 0.2126) + (color.g * 0.7152) + (color.b * 0.0722));
	                #else
                        vec3 mixColor = color.rgb;
                    #endif

                    float inten = intensity;
	                #if COMPRESS
                        float bright = ((color.r * 0.2126) + (color.g * 0.7152) + (color.b * 0.0722));
                        inten = inten * (1.0 - min(sqrt(bright), 1.0));
                        //inten = inten * (1.0 - min(bright * (1.0 + brightness) * 0.5, 1.0));
                    #endif
                    
                    float scale = 1.0 + brightness * inten;
                    outColor.r = min((color.r * (1.0 - inten) + mixColor.r * inten) * scale, 1.0);
                    outColor.g = min((color.g * (1.0 - inten) + mixColor.g * inten) * scale, 1.0);
                    outColor.b = min((color.b * (1.0 - inten) + mixColor.b * inten) * scale, 1.0);
                    outColor.a = color.a;
                }";
		}
	}
}