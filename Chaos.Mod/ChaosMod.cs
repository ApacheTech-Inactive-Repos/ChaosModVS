using Chaos.Engine.Systems;
using Chaos.Mod.Controllers;
using VintageMods.Core.Attributes;
using VintageMods.Core.ModSystems;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

[assembly: ModDomain("chaosmod", "ChaosMod")]

namespace Chaos.Mod
{
    internal class ChaosMod : UniversalInternalMod<ServerSystemChaosMod, ClientSystemChaosMod>
    {
        public ChaosMod() : base("chaosmod")
        {
        }

        public EffectsController Effects { get; private set; }

        public override double ExecuteOrder()
        {
            return 0.0;
        }

        // Perform actions on both Server and Client.
        public override void Start(ICoreAPI api)
        {
            base.Start(api);
            Effects ??= new EffectsController(Api);
        }

        // Perform actions on Client.
        public override void StartClientSide(ICoreClientAPI capi)
        {
            Effects.Client.Initialise(ClientChannel);
            RegisterHotkeys();
        }

        // Perform actions on Server.
        public override void StartServerSide(ICoreServerAPI sapi)
        {
            Effects.Server.Initialise(ServerChannel);
        }

        private void RegisterHotkeys()
        {
            Capi.Input.RegisterHotKey("chaos-boom", "Boom!", GlKeys.ControlRight);
            Capi.Input.SetHotKeyHandler("chaos-boom", _ =>
            {
                Effects.Client.StartExecute("ObliterateAllNearbyAnimals");
                return true;
            });
        }
    }
}