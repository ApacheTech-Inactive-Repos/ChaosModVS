using Vintagestory.API.Common;

namespace Chaos.Engine.Primitives
{
    public abstract class ChaosEffectSettings
    {
        protected ICoreAPI Api { get; private set; }
        private string Id { get; set; }

        internal static T Load<T>(ICoreAPI api, string id) where T: ChaosEffectSettings, new()
        {
            return new() { Api = api };
        }
    }
}