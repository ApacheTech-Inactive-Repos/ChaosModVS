using System;
using System.Linq;
using Chaos.Engine.Systems;
using Chaos.Mod.Controllers;
using VintageMods.Core.Attributes;
using VintageMods.Core.IO.Enum;
using VintageMods.Core.Maths;
using VintageMods.Core.ModSystems;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.Client.NoObf;
using Vintagestory.GameContent;
using Vintagestory.Server;

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
            Files.RegisterFile("global-config.json", FileScope.Global);
            Effects ??= new EffectsController(Api);
        }
        
        // Perform actions on Client.
        public override void StartClientSide(ICoreClientAPI capi)
        {
            Effects.InitialiseClient(ClientChannel);
            RegisterHotkeys();

            Capi.Input.RegisterHotKey("chaos-boom", "Boom!", GlKeys.ControlRight);
            Capi.Input.SetHotKeyHandler("chaos-boom", _ =>
            {
                Effects.StartExecuteClient("NightVision");
                return true;
            });
        }


        // Perform actions on Server.
        public override void StartServerSide(ICoreServerAPI sapi)
        {
            Effects.InitialiseServer(ServerChannel);
            sapi.RegisterCommand("storm", "", "", (player, _, _) =>
            {
                var currentTime = sapi.World.Calendar.TotalDays;
                var system = sapi.ModLoader.GetModSystem<SystemTemporalStability>();
                system.StormData.nextStormTotalDays = currentTime;
            });
        }
        
        #region Sandbox Methods

        private void RegisterHotkeys()
        {
            Capi.Input.RegisterHotKey("roll-cw", "Roll Clockwise", GlKeys.Keypad6);
            Capi.Input.SetHotKeyHandler("roll-cw", _ =>
            {
                //PlayerCameraEx.RollClockwise();
                
                return true;
            });

            Capi.Input.RegisterHotKey("roll-acw", "Roll Anti-Clockwise", GlKeys.Keypad4);
            Capi.Input.SetHotKeyHandler("roll-acw", _ =>
            {
                //PlayerCameraEx.RollAntiClockwise();
                AddForceTo(Capi.World.Player);
                return true;
            });

            Capi.Input.RegisterHotKey("roll-reset", "Reset Roll", GlKeys.Keypad5);
            Capi.Input.SetHotKeyHandler("roll-reset", _ =>
            {
                //PlayerCameraEx.ResetRoll();
                RevertGfxToDefault();
                return true;
            });
        }

        private void RevertGfxToDefault()
        {
            // TODO: Ready to ship to Shaders controller.
            ClientSettings.BrightnessLevel = 1f;
            ClientSettings.AmbientBloomLevel = 20;
            ClientSettings.GammaLevel = 2.2f;
            ClientSettings.ExtraGammaLevel = 1f;
            ClientSettings.Bloom = false;
            Capi.Shader.ReloadShaders();
        }

        private void AddForceTo(IPlayer player)
        {
            var vecAhead = player.Entity.Pos.Motion.Ahead(-1, 0, player.Entity.BodyYaw);
            player.Entity.Pos.Motion.X = vecAhead.X * 0.01;
            player.Entity.Pos.Motion.Z = vecAhead.Z * 0.01;
        }

        #endregion
    }

    public static class Stuff
    {


    }
}