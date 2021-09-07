using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using Chaos.Engine.Contracts;
using Chaos.Engine.Network.Messages;
using Chaos.Mod.Extensions;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

namespace Chaos.Mod.Effects
{
    // HACK: This class is currently tightly coupled to the ModSystem. Further refactoring must be done.

    public class EffectsController
    {
        [Export]
        private ICoreAPI Api { get; }

        [ImportMany("ChaosEffects", typeof(IChaosEffect), AllowRecomposition = true)]
        private IEnumerable<Lazy<IChaosEffect, IChaosEffectMetadata>> Effects { get; set; }

        public IClientNetworkChannel ClientNetworkChannel { get; set; }
        public IServerNetworkChannel ServerNetworkChannel { get; set; }

        public EffectsController(ICoreAPI api)
        {
            Api = api;

            var cat = new AggregateCatalog();
            cat.Catalogs.Add(new AssemblyCatalog(GetType().Assembly));

            // HACK: Added a Directory Catalogue, in preparation for separate Effects libraries.

            //? A lot of this will need to be refactored out, and handled globally, by the ChaosAPI.
            //? Maybe, even extending the FileManager class to handle watched Directories as well.

            var packPath = Path.Combine(GamePaths.DataPath, "ChaosMod", "Packs");
            Directory.CreateDirectory(packPath);
            cat.Catalogs.Add(new DirectoryCatalog(packPath));
            
            new CompositionContainer(cat).ComposeParts(this);
        }

        public void Start(string id)
        {
            var lazyEffect = Effects.FirstOrDefault(p => p.Metadata.Id == id);
            if (lazyEffect is null) return;
            lazyEffect.Value.OnClientStart(Api as ICoreClientAPI);
            ClientNetworkChannel.SendPacket(new HandleEffectPacket(id));
        }

        public void HandleEffect(IServerPlayer player, HandleEffectPacket packet)
        {
            var lazyEffect = Effects.FirstOrDefault(p => p.Metadata.Id == packet.EffectId);
            if (lazyEffect is null) return;
            lazyEffect.Value.OnServerStart(player, Api as ICoreServerAPI);
            player.SendMessage(ChaosLangEx.Effect(lazyEffect.Metadata, "Title"));
            player.SendMessage(ChaosLangEx.Effect(lazyEffect.Metadata, "Description"));
        }
    }
}