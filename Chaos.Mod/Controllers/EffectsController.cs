using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using AutoMapper.Configuration.Conventions;
using Chaos.Engine.API;
using Chaos.Engine.Attributes;
using Chaos.Engine.Contracts;
using Chaos.Engine.Extensions;
using Chaos.Engine.Network.Messages;
using Chaos.Engine.Primitives;
using Chaos.Engine.Systems;
using Chaos.Mod.Extensions;
using HarmonyLib;
using VintageMods.Core.Extensions;
using VintageMods.Core.IO;
using VintageMods.Core.IO.Extensions;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;

namespace Chaos.Mod.Controllers
{
    // HACK: This class is currently tightly coupled to the ModSystem. Further refactoring must be done.

    public class EffectsController
    {
        private readonly IChaosAPI _chaosApi;

        [Export]
        private ICoreAPI Api { get; set; }

        [ImportMany("ChaosEffects", typeof(IChaosEffect), AllowRecomposition = true)]
        private IEnumerable<IChaosEffect> Effects { get; set; }

        private IClientNetworkChannel ClientChannel { get; set; }
        private IServerNetworkChannel ServerChannel { get; set; }


        public void InitialiseClient(IClientNetworkChannel channel)
        {
            ClientChannel ??= channel.RegisterMessageType<HandleEffectPacket>();
        }

        public void StartExecuteClient(string id)
        {
            var effect = Effects.FirstOrDefault(p => p.Id == id);
            if (effect is null) return;


            var async = (Api as ICoreClientAPI).GetVanillaClientSystem<ClientSystemChaosMod>();

            async.EnqueueAsyncTask(() =>
            {
                ClientChannel.SendPacket(new HandleEffectPacket(id));
                effect.OnClientStart(Api as ICoreClientAPI);
            });
        }

        public void InitialiseServer(IServerNetworkChannel channel)
        {
            ServerChannel ??= channel
                .RegisterMessageType<HandleEffectPacket>()
                .SetMessageHandler<HandleEffectPacket>(StartExecuteServer);
        }

        private void StartExecuteServer(IServerPlayer player, HandleEffectPacket packet)
        {
            var effect = Effects.FirstOrDefault(p => p.Id == packet.EffectId);
            if (effect is null) return;

            var async = (Api as ICoreServerAPI).GetVanillaServerSystem<ServerSystemChaosMod>();
            async.EnqueueAsyncTask(() =>
            {
                effect.OnServerStart(player, Api as ICoreServerAPI);
            });

            player.SendMessage(effect.Title());
            player.SendMessage(effect.Description());
        }

        public EffectsController(ICoreAPI api)
        {
            Api ??= api;
            _chaosApi ??= new ChaosAPI(api);
            Effects = GetType().Assembly.GetEnumerableOfType<ChaosEffect>();
            foreach (var effect in Effects)
            {
                effect.Api = api;
                effect.ChaosApi = _chaosApi;
                // TODO: Any other effect initialisation logic goes here.
            }
        }

        private void ComposeEffects()
        {
            var cat = new AggregateCatalog();

            //? A lot of this will need to be refactored out, and handled globally, by the ChaosAPI.
            //? Maybe, even extending the FileManager class to handle watched Directories as well.

#if DEBUG
            var packPath = Path.GetFullPath(@"C:\Users\Apache\source\repos\ChaosModVS\.effects");
#else
            var packPath = Path.Combine(GamePaths.DataPath, "ChaosMod", "Packs");
#endif
            Directory.CreateDirectory(packPath);
            cat.Catalogs.Add(new AssemblyCatalog(GetType().Assembly));
            cat.Catalogs.Add(new AssemblyCatalog(typeof(ChaosAPI).Assembly));
            cat.Catalogs.Add(new DirectoryCatalog(packPath!));
            var container = new CompositionContainer(cat);

            foreach (var assembly in container.GetAssembliesWithExports())
            {
                ResourceManager.DisembedAssets(assembly);
            }

            container.ComposeParts(this);
        }
    }

    public static class AssemblyEx
    {
        public static IEnumerable<T> GetEnumerableOfType<T>(this Assembly assembly, params object[] constructorArgs) where T : class, IComparable<T>
        {
            var objects = assembly
                .GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T)))
                .Select(type => (T) Activator.CreateInstance(type, constructorArgs))
                .ToList();
            objects.Sort();

            return objects;
        }

        public static IEnumerable<Type> GetDerivedTypes<T>(this T attribute, Assembly assembly) where T : Attribute
        {
            return assembly.GetTypes()
                .Where(type => type.GetCustomAttributes(typeof(T), true).Length > 0);
        }

        public static IEnumerable<Type> GetTypesWithAttribute<T>(this Assembly assembly)
        {
            return assembly.GetTypes()
                .Where(type => type.GetCustomAttributes(typeof(T), true).Length > 0);
        }
    }

}