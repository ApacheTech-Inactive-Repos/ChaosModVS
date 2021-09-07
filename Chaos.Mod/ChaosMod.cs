using System.IO;
using Chaos.Engine.API;
using Chaos.Engine.Contracts;
using Chaos.Engine.Network.Messages;
using Chaos.Mod.Effects;
using VintageMods.Core.Attributes;
using VintageMods.Core.ModSystems;
using VintageMods.Core.Network.Messages;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

[assembly: ModDomain("chaosmod", "ChaosMod")]

namespace Chaos.Mod
{
    internal class ChaosMod : UniversalModSystem
    {
        private IChaosAPI _chaosApi;
        private EffectsController _effects;

        public ChaosMod() : base("chaosmod")
        {
        }

        public override double ExecuteOrder()
        {
            return 0.0;
        }

        public override void Start(ICoreAPI api)
        {
            base.Start(api);
            _chaosApi = new ChaosAPI(Api);
            _effects = new EffectsController(Api);
        }

        public override void StartClientSide(ICoreClientAPI capi)
        {
            ClientNetworkChannel
                .RegisterMessageType<HandleEffectPacket>();

            Capi.Input.RegisterHotKey("mef", "MEF Packet Test", GlKeys.AltRight);
            Capi.Input.SetHotKeyHandler("mef", _ =>
            {
                ClientNetworkChannel.SendPacket(new CompositionDataPacket
                {
                    Data = File.ReadAllBytes(@"C:\Users\Apache\source\repos\ChaosModVS\Chaos.PayloadTest\bin\Debug\netstandard2.0\Chaos.PayloadTest.dll")
                });
                return true;
            });

            Capi.Input.RegisterHotKey("chaos-boom", "Boom!", GlKeys.ControlRight);
            Capi.Input.SetHotKeyHandler("chaos-boom", _ =>
            {
                const string id = "Creature.ObliterateAllNearbyCreatures";
                _effects.Execute(id, ClientNetworkChannel);
                return true;
            });
        }

        public override void StartServerSide(ICoreServerAPI sapi)
        {
            ServerNetworkChannel
                .RegisterMessageType<HandleEffectPacket>()
                .SetMessageHandler<HandleEffectPacket>(_effects.HandleEffect);
        }
    }
}