using System.IO;
using System.Reflection;
using Chaos.Engine.Contracts;
using Vintagestory.API.Datastructures;

namespace Chaos.Engine.Effects
{
    public static class ChaosModConfig
    {
        private static AggregateEffectSettings ChaosSettings => AggregateEffectSettings.Load();

        public static JsonObject GetSettings(IChaosEffect effect)
        {
            return ChaosSettings[$"{effect.Pack}"][$"{effect.EffectType}"][$"{effect.Id}"];
        }

        public static JsonObject LoadSettings(IChaosEffect effect)
        {
            var rootFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var configFolder = new DirectoryInfo(Path.Combine(rootFolder!, "assets", "chaosmod", "config"));
            var file = Path.Combine(configFolder.FullName, "effects", effect.Pack.ToLowerInvariant(), "effect-settings.json");
            return JsonObject.FromJson(File.ReadAllText(file))[$"{effect.EffectType}"][effect.Id];
        }
    }
}