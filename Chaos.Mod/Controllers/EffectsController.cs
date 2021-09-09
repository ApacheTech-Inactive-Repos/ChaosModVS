using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.ReflectionModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Chaos.Engine.Contracts;
using Chaos.Engine.Extensions;
using Chaos.Engine.Network.Messages;
using Chaos.Engine.Primitives;
using VintageMods.Core.Extensions;
using VintageMods.Core.IO;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

namespace Chaos.Mod.Controllers
{
    // HACK: This class is currently tightly coupled to the ModSystem. Further refactoring must be done.

    public class EffectsController : ControllerBase<EffectsController.ServerSide, EffectsController.ClientSide>
    {
        private new static ICoreAPI Api { get; set; }

        [Export]
        private ICoreAPI ExportableCoreAPI { get; set; }

        [ImportMany("ChaosEffects", typeof(IChaosEffect), AllowRecomposition = true)]
        private IEnumerable<IChaosEffect> EffectImports { get; set; }

        private static IEnumerable<IChaosEffect> Effects { get; set; }

        private static IClientNetworkChannel ClientChannel { get; set; }
        private static IServerNetworkChannel ServerChannel { get; set; }


        public class ClientSide
        {
            public void Initialise(IClientNetworkChannel channel)
            {
                ClientChannel ??= channel.RegisterMessageType<HandleEffectPacket>();
            }

            public void StartExecute(string id)
            {
                var effect = Effects.FirstOrDefault(p => p.Id == id);
                if (effect is null) return;
                    effect.OnClientStart(Api as ICoreClientAPI);
                ClientChannel.SendPacket(new HandleEffectPacket(id));
            }

        }

        public class ServerSide
        {
            public void Initialise(IServerNetworkChannel channel)
            {
                ServerChannel ??= channel
                    .RegisterMessageType<HandleEffectPacket>()
                    .SetMessageHandler<HandleEffectPacket>(StartExecute);
            }

            public void StartExecute(IServerPlayer player, HandleEffectPacket packet)
            {
                var effect = Effects.FirstOrDefault(p => p.Id == packet.EffectId);
                if (effect is null) return;
                effect.OnServerStart(player, Api as ICoreServerAPI);
                player.SendMessage(effect.Title());
                player.SendMessage(effect.Description());
            }

        }

        public EffectsController(ICoreAPI api) : base(api)
        {
            Api ??= ExportableCoreAPI ??= api;

            var cat = new AggregateCatalog();
            cat.Catalogs.Add(new AssemblyCatalog(GetType().Assembly));

            // HACK: Added a Directory Catalogue, in preparation for separate Effects libraries.

            //? A lot of this will need to be refactored out, and handled globally, by the ChaosAPI.
            //? Maybe, even extending the FileManager class to handle watched Directories as well.


            //? MEF Composition needs to be expanded, so that you import a Pack. Not just a list of Effects.
            //? When importing a pack, you'd need to copy the assets files from embedded resources, into
            //? the correct locations in game. (PARTIALLY IMPLEMENTED)
            
            //? You can do the same with settings files, but they are more easy to manage.

            var directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var packPath = Path.Combine(GamePaths.DataPath, "ChaosMod", "Packs");
            Directory.CreateDirectory(packPath);
            cat.Catalogs.Add(new DirectoryCatalog(directoryName!));
            var container = new CompositionContainer(cat);

            foreach (var assembly in container.GetAssembliesWithExports())
            {
                ResourceManager.DisembedAssets(assembly, directoryName);
            }
            
            container.ComposeParts(this);
            Effects ??= EffectImports;
        }
    }

    public static class MefExtensions
    {
        public static IEnumerable<Assembly> GetAssembliesWithExports(this CompositionContainer container)
        {
            return container.Catalog?.Parts
                .Select(part => ReflectionModelServices.GetPartType(part).Value.Assembly)
                .Distinct()
                .ToList();
        }

        public static void AddIfNotPresent<T>(this List<T> array, T item)
        {
            if (!array.Contains(item)) array.Add(item);
        }
    }
}