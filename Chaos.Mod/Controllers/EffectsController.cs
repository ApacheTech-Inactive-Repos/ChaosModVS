using System;
using System.Collections.Generic;
using System.Linq;
using Chaos.Engine.API;
using Chaos.Engine.Contracts;
using Chaos.Engine.Effects;
using Chaos.Engine.Enums;
using Chaos.Engine.Extensions;
using Chaos.Engine.Network.Messages;
using Chaos.Engine.Primitives;
using Chaos.Mod.Extensions;
using VintageMods.Core.Extensions;
using VintageMods.Core.Helpers;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Chaos.Mod.Controllers
{
    public class EffectsController
    {
        private ICoreAPI _api;
        private ICoreClientAPI _capi;
        private ICoreServerAPI _sapi;

        private IChaosAPI ChaosApi { get; }

        private long _listenerId;

        private IEnumerable<IChaosEffect> Effects { get; }

        private Dictionary<string, IChaosEffect> RunningEffects { get; } = new();

        public EffectsController()
        {
            ChaosApi ??= new ChaosAPI();
            Effects = GetType().Assembly.GetEnumerableOfType<ChaosEffect>();
            foreach (var effect in Effects) effect.ChaosApi = ChaosApi;
        }

        public void InitialiseClient(ICoreClientAPI capi)
        {
            _api = _capi = capi;
            NetworkEx.Client.RegisterMessageType<HandleEffectPacket>();
        }

        public void InitialiseServer(ICoreServerAPI sapi)
        {
            _api = _sapi = sapi;
            NetworkEx.Server.RegisterMessageType<HandleEffectPacket>()
                .SetMessageHandler<HandleEffectPacket>((_, packet) => HandleEffect(packet));
        }

        public void StartExecuteClient(string id)
        {
            if (_api.Side.IsServer()) return;
            if (Effects.All(p => p.Id != id)) return;
            AsyncEx.Client.EnqueueAsyncTask(() =>
                HandleEffect(HandleEffectPacket.CreateSetupPacket(id)));
        }

        private void HandleEffect(HandleEffectPacket packet)
        {
            // First Use: Client Setup Packet.

            // ============================================================

            // On Client Setup
            //  - Set player for effect.
            //  - OnClientSetup()               CLIENT SETUP
            //  - Send Setup Packet.            SEND SERVER SETUP PACKET

            // On Client Start
            //  - OnClientStart()               CLIENT START
            //  - Send Start Packet.            SEND SERVER START PACKET
            //  - Running = true;
            //  - Add as running effect.
            //  - If Effect is not Instant, Set Tick Timer

            // On Client Tick
            //  - Has time elapsed?
            //  - No
            //      - OnClientTick()            CLIENT TICK
            //      - Send Tick Packet.         SEND SERVER TICK PACKET
            //  - Yes
            //      - Unregister listener
            //      - HandleEffect(stop)

            // On Client Stop
            //  - OnClientStop()                CLIENT STOP
            //  - Send Stop Packet.             SEND SERVER STOP PACKET
            //  - Running = false;
            //  - Remove as running effect.

            // On Client TakeDown
            //  - OnClientTakeDown()            CLIENT TAKEDOWN
            //  - Send Stop Packet.             SEND SERVER TAKEDOWN PACKET

            // ============================================================

            // On Server Setup
            //  - Set player for effect.
            //  - OnServerSetup()               SERVER SETUP

            // On Server Start
            //  - Notify player of Effect.
            //  - OnServerStart()               SERVER START
            //  - Running = true;
            //  - Add as running effect.

            // On Server Tick
            //  - OnServerTick()                SERVER TICK

            // On Server Stop
            //  - OnServerStop()                SERVER STOP
            //  - Running = false;
            //  - Remove as running effect.

            // On Server TakeDown
            //  - OnServerTakeDown()            SERVER TAKEDOWN

            // ============================================================

            var effect = Effects.FirstOrDefault(p => p.Id == packet.EffectId);
            if (effect == null) return;

            effect.Settings = ChaosModConfig.LoadSettings(effect);

            if (_api.Side.IsClient())
            {
                AsyncEx.Client.EnqueueAsyncTask(() =>
                {
                    switch (packet.Command)
                    {
                        // On Client Setup
                        //  - Set player for effect.
                        //  - OnClientSetup()               CLIENT SETUP
                        //  - Send Setup Packet.            SEND SERVER SETUP PACKET
                        case EffectCommand.Setup when effect.Running:
                            return;
                        case EffectCommand.Setup:
                            effect.Player = _capi.World.Player;
                            effect.OnClientSetup(_capi);
                            NetworkEx.Client.SendPacket(packet);
                            HandleEffect(HandleEffectPacket.CreateStartPacket(effect.Id));
                            break;

                        // On Client Start
                        //  - Set StartTick
                        //  - OnClientStart()               CLIENT START
                        //  - Send Start Packet.            SEND SERVER START PACKET
                        //  - Running = true;
                        //  - Add as running effect.
                        //  - If Effect is not Instant, Set Tick Timer
                        case EffectCommand.Start when effect.Running:
                            return;
                        case EffectCommand.Start:
                            effect.StartTick = _capi.InWorldEllapsedMilliseconds;
                            effect.OnClientStart(_capi);
                            NetworkEx.Client.SendPacket(HandleEffectPacket.CreateStartPacket(effect.Id));
                            if (effect.Duration is EffectDuration.Instant) return;
                            effect.Running = true;
                            RunningEffects.Add(effect.Id, effect);
                            _capi.Event.EnqueueMainThreadTask(() =>
                            {
                                _listenerId = _capi.Event.RegisterGameTickListener(
                                    dt => { HandleEffect(HandleEffectPacket.CreateTickPacket(effect.Id, dt)); },
                                    ChaosApi.GlobalConfig["EffectTickInterval"].AsInt(20));
                            }, "");
                            break;

                        // On Client Tick
                        //  - Has time elapsed?
                        //  - Yes
                        //      - Unregister listener
                        //      - HandleEffect(stop)
                        //  - No
                        //      - OnClientTick()            CLIENT TICK
                        //      - Send Tick Packet.         SEND SERVER TICK PACKET
                        case EffectCommand.Tick when !effect.Running:
                            return;
                        case EffectCommand.Tick:
                            var elapsed = (_capi.InWorldEllapsedMilliseconds - effect.StartTick) / 1000;
                            var duration = effect.Duration switch
                            {
                                EffectDuration.Instant => 0,
                                EffectDuration.Short => ChaosApi.GlobalConfig["EffectDuration"]["Short"].AsInt(30),
                                EffectDuration.Standard => ChaosApi.GlobalConfig["EffectDuration"]["Standard"].AsInt(60),
                                EffectDuration.Long => ChaosApi.GlobalConfig["EffectDuration"]["Long"].AsInt(120),
                                EffectDuration.Permanent => int.MaxValue,
                                EffectDuration.Custom => ChaosApi.GlobalConfig["CustomDurations"][effect.Pack][$"{effect.EffectType}"][effect.Id].AsInt(30),
                                _ => throw new ArgumentOutOfRangeException()
                            };
                            if (elapsed >= duration)
                            {
                                AsyncEx.Client.EnqueueMainThreadTask(() =>
                                {
                                    _capi.Event.UnregisterGameTickListener(_listenerId);
                                });
                                HandleEffect(HandleEffectPacket.CreateStopPacket(effect.Id));
                            }
                            else
                            {
                                effect.OnClientTick(packet.DeltaTime);
                                NetworkEx.Client.SendPacket(packet);
                            }
                            break;

                        // On Client Stop
                        //  - OnClientStop()                CLIENT STOP
                        //  - Send Stop Packet.             SEND SERVER STOP PACKET
                        //  - Running = false;
                        //  - Remove as running effect.
                        case EffectCommand.Stop when !effect.Running:
                            return;
                        case EffectCommand.Stop:
                            effect.OnClientStop();
                            NetworkEx.Client.SendPacket(HandleEffectPacket.CreateStopPacket(effect.Id));
                            effect.Running = false;
                            RunningEffects.Remove(effect.Id);
                            break;

                        // On Client TakeDown
                        //  - OnClientTakeDown()            CLIENT TAKEDOWN
                        //  - Send Stop Packet.             SEND SERVER TAKEDOWN PACKET
                        case EffectCommand.TakeDown when effect.Running:
                            return;
                        case EffectCommand.TakeDown:
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                });
            }

            if (_api.Side.IsServer())
            {
                AsyncEx.Server.EnqueueAsyncTask(() =>
                {
                    var player = (IServerPlayer)_sapi.World.PlayerByUid(packet.PlayerUID);
                    switch (packet.Command)
                    {
                        // On Server Setup
                        //  - Set player for effect.
                        //  - OnServerSetup()               SERVER SETUP
                        case EffectCommand.Setup when effect.Running:
                            return;
                        case EffectCommand.Setup:
                            effect.Player = _sapi.World.PlayerByUid(packet.PlayerUID);
                            break;

                        // On Server Start
                        //  - Notify player of Effect.
                        //  - OnServerStart()               SERVER START
                        //  - Running = true;
                        //  - Add as running effect.
                        case EffectCommand.Start when effect.Running:
                            return;
                        case EffectCommand.Start:
                            _sapi.SendIngameDiscovery(player, null, effect.Title());
                            _sapi.World.PlaySoundAt(new AssetLocation("sounds/effect/deepbell"), player.Entity, null, false, 32f, 0.5f);
                            effect.OnServerStart(player, _sapi);
                            if (effect.Duration is EffectDuration.Instant) return;
                            effect.Running = true;
                            RunningEffects.Add(effect.Id, effect);
                            break;

                        // On Server Tick
                        //  - OnServerTick()                SERVER TICK
                        case EffectCommand.Tick when !effect.Running:
                            return;
                        case EffectCommand.Tick:
                            effect.OnServerTick(packet.DeltaTime);
                            break;

                        // On Server Stop
                        //  - OnServerStop()                SERVER STOP
                        //  - Running = false;
                        //  - Remove as running effect.
                        case EffectCommand.Stop when !effect.Running:
                            return;
                        case EffectCommand.Stop:
                            effect.OnServerStop();
                            effect.Running = false;
                            RunningEffects.Remove(effect.Id);
                            break;

                        // On Server TakeDown
                        //  - OnServerTakeDown()            SERVER TAKEDOWN
                        case EffectCommand.TakeDown when effect.Running:
                            return;
                        case EffectCommand.TakeDown:
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                });
            }
        }
    }
}