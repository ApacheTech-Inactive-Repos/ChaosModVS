namespace Chaos.Engine.Contracts.Controllers
{
    public interface IClientShaderController
    {
        void ReloadShaders();
        void RevertGfxToDefault();
    }
}