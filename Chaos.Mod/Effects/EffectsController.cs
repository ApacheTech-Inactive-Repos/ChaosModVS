using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using Chaos.Engine.Contracts;
using Chaos.Engine.Network.Messages;
using Chaos.Mod.Effects.Creatures;
using Chaos.Mod.Extensions;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Chaos.Mod.Effects
{
    public class EffectsController
    {
        [Export]
        private ICoreAPI Api { get; }

        [ImportMany("ChaosEffects", typeof(IChaosEffect), AllowRecomposition = true)]
        private IEnumerable<Lazy<IChaosEffect, IChaosEffectMetadata>> Effects { get; set; }

        public EffectsController(ICoreAPI api)
        {
            Api = api;

            var cat = new AssemblyCatalog(typeof(EffectCreatureObliterateAllNearbyCreatures).Assembly);
            new CompositionContainer(cat).ComposeParts(this);
        }

        private IChaosEffect GetEffect(string id)
        {
            try
            {
                //return Effects.First();
                return Effects.First(p => p.Metadata.Id == id).Value;
            }
            catch (ArgumentNullException)
            {
                Api.World.Logger.Error($"Chaos Effect not found: { id }");
                throw;
            }
        }

        public void Execute(string id, IClientNetworkChannel channel)
        {
            var lazyEffect = Effects.FirstOrDefault(p => p.Metadata.Id == id);
            if (lazyEffect is null) return;
            lazyEffect.Value.OnClientStart(Api as ICoreClientAPI);
            channel.SendPacket(new HandleEffectPacket(id));
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