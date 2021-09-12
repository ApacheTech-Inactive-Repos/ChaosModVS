using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Chaos.Engine.Exceptions;
using Newtonsoft.Json.Linq;
using Vintagestory.API.Datastructures;

namespace Chaos.Engine.Effects
{
    internal class AggregateEffectSettings
    {
        private static IDictionary<string, JsonObject> Files { get; } = new Dictionary<string, JsonObject>();

        private AggregateEffectSettings()
        {
            Files.Clear();
            var rootFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var di = new DirectoryInfo(Path.Combine(rootFolder!, "assets", "chaosmod", "config"));
            foreach (var fi in di.EnumerateFiles("effect-settings.json", SearchOption.AllDirectories))
            {
                // TODO: Refactor to solve the need to parse twice, into JObject, and JsonObject.
                var fileContents = fi.OpenText().ReadToEnd();
                var json = JObject.Parse(fileContents);
                var obj = JsonObject.FromJson(fileContents);
                
                var packName = json.Properties().Select(p => p.Name).First();

                var settings = obj[packName];

                Files.Add(packName, settings);
            }
        }


        internal JsonObject this[string path]
        {
            get
            {
                if (Files.ContainsKey(path)) return Files[path];
                throw new ChaosException($"Cannot load settings from pack: {path}");
            }
        }

        internal static AggregateEffectSettings Load()
        {
            return new();
        }
    }
}