using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using Chaos.Engine.Contracts;
using Chaos.Engine.Extensions;
using Chaos.Engine.Network.Messages;
using Chaos.Engine.Primitives;
using VintageMods.Core.Extensions;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

namespace Chaos.Mod.Effects
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

        private static IClientNetworkChannel ClientNetworkChannel { get; set; }
        private static IServerNetworkChannel ServerNetworkChannel { get; set; }


        public class ClientSide
        {
            public void Initialise(IClientNetworkChannel channel)
            {
                ClientNetworkChannel = channel.RegisterMessageType<HandleEffectPacket>();
            }

            public void StartExecute(string id)
            {
                var effect = Effects.FirstOrDefault(p => p.Id == id);
                if (effect is null) return;
                    effect.OnClientStart(Api as ICoreClientAPI);
                ClientNetworkChannel.SendPacket(new HandleEffectPacket(id));
            }

        }

        public class ServerSide
        {
            public void Initialise(IServerNetworkChannel channel)
            {
                ServerNetworkChannel = channel
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

            var packPath = Path.Combine(GamePaths.DataPath, "ChaosMod", "Packs");
            Directory.CreateDirectory(packPath);
            cat.Catalogs.Add(new DirectoryCatalog(packPath));
            
            new CompositionContainer(cat).ComposeParts(this);
            Effects ??= EffectImports;
        }
    }
}