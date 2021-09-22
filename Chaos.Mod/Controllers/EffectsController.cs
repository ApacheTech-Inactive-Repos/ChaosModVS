using System;
using System.Collections.Generic;
using System.Linq;
using Chaos.Engine.Effects.Contracts;
using Chaos.Engine.Effects.Enums;
using Chaos.Engine.Effects.Extensions;
using Chaos.Engine.Effects.Primitives;
using Chaos.Engine.Network.Messages;
using VintageMods.Core.Extensions;
using VintageMods.Core.Helpers;
using VintageMods.Core.IO.Extensions;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;

namespace Chaos.Mod.Controllers
{
    public class EffectsController
    {
        private ICoreAPI _api;
        private ICoreClientAPI _capi;

        private long _listenerId;
        private ICoreServerAPI _sapi;

        public EffectsController()
        {
            Effects = GetType().Assembly.GetEnumerableOfType<ChaosEffect>();
            foreach (var effect in Effects) effect.GlobalConfig = GlobalConfig;
        }

        private JsonObject GlobalConfig { get; } = ApiEx.Universal.GetModFile("global-config.json").AsRawJsonObject();

        private IEnumerable<IChaosEffect> Effects { get; }

        private Dictionary<string, IChaosEffect> RunningEffects { get; } = new();

        public void InitialiseClient(ICoreClientAPI capi)
        {
            _api = _capi = capi;
            NetworkEx.Client.RegisterMessageType<ChaosEffectPacket>();
        }

        public void InitialiseServer(ICoreServerAPI sapi)
        {
            _api = _sapi = sapi;
            NetworkEx.Server.RegisterMessageType<ChaosEffectPacket>()
                .SetMessageHandler<ChaosEffectPacket>((_, packet) => HandleEffect(packet));
        }

        public void StartExecuteClient(string id)
        {
            if (_api.Side.IsServer()) return;
            if (Effects.All(p => p.Id != id)) return;
            AsyncEx.Client.EnqueueAsyncTask(() =>
                HandleEffect(ChaosEffectPacket.CreateSetupPacket(id)));
        }

        private void HandleEffect(ChaosEffectPacket packet)
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
            //  - Is Effect Instant?
            //  - Yes
            //      - OnClientTakeDown()        CLIENT TAKEDOWN
            //      - Send TakeDown Packet.     SEND SERVER TAKEDOWN PACKET
            //  - No
            //      - Set Tick Timer

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

            if (_api.Side.IsClient())
                AsyncEx.Client.EnqueueAsyncTask(() =>
                {
                    switch (packet.Command)
                    {
                        // On Client Setup
                        //  - Set player for effect.
                        //  - OnClientSetup()               CLIENT SETUP
                        //  - Send Setup Packet.            SEND SERVER SETUP PACKET
                        case EffectStage.Setup when effect.Running:
                            return;
                        case EffectStage.Setup:
                            effect.SetPlayer(_capi.World.Player);
                            effect.OnClientSetup(_capi);
                            NetworkEx.Client.SendPacket(packet);
                            HandleEffect(ChaosEffectPacket.CreateStartPacket(effect.Id));
                            break;

                        // On Client Start
                        //  - Set StartTick
                        //  - OnClientStart()               CLIENT START
                        //  - Send Start Packet.            SEND SERVER START PACKET
                        //  - Running = true;
                        //  - Add as running effect.
                        //  - Is Effect Instant?
                        //  - Yes
                        //      - OnClientTakeDown()        CLIENT TAKEDOWN
                        //      - Send TakeDown Packet.     SEND SERVER TAKEDOWN PACKET
                        //  - No
                        //      - Set Tick Timer
                        case EffectStage.Start when effect.Running:
                            return;
                        case EffectStage.Start:
                            effect.StartTick = _capi.InWorldEllapsedMilliseconds;
                            effect.OnClientStart(_capi);
                            NetworkEx.Client.SendPacket(packet);
                            if (effect.Duration is EffectDuration.Instant)
                            {
                                effect.OnClientTakeDown(_capi);
                                NetworkEx.Client.SendPacket(ChaosEffectPacket.CreateTakeDownPacket(effect.Id));
                                return;
                            }
                            effect.Running = true;
                            RunningEffects.Add(effect.Id, effect);
                            _capi.Event.EnqueueMainThreadTask(() =>
                            {
                                _listenerId = _capi.Event.RegisterGameTickListener(
                                    dt => { HandleEffect(ChaosEffectPacket.CreateTickPacket(effect.Id, dt)); },
                                    GlobalConfig["EffectTickInterval"].AsInt(20));
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
                        case EffectStage.Tick when !effect.Running:
                            return;
                        case EffectStage.Tick:
                            var elapsed = (_capi.InWorldEllapsedMilliseconds - effect.StartTick) / 1000;
                            var duration = effect.Duration switch
                            {
                                EffectDuration.Instant => 0,
                                EffectDuration.Short => GlobalConfig["EffectDuration"]["Short"].AsInt(30),
                                EffectDuration.Standard => GlobalConfig["EffectDuration"]["Standard"]
                                    .AsInt(60),
                                EffectDuration.Long => GlobalConfig["EffectDuration"]["Long"].AsInt(120),
                                EffectDuration.Permanent => int.MaxValue,
                                EffectDuration.Custom => GlobalConfig["CustomDurations"][effect.Pack][
                                    $"{effect.EffectType}"][effect.Id].AsInt(30),
                                _ => throw new ArgumentOutOfRangeException()
                            };
                            if (elapsed >= duration)
                            {
                                AsyncEx.Client.EnqueueMainThreadTask(() =>
                                {
                                    _capi.Event.UnregisterGameTickListener(_listenerId);
                                });
                                HandleEffect(ChaosEffectPacket.CreateStopPacket(effect.Id));
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
                        case EffectStage.Stop when !effect.Running:
                            return;
                        case EffectStage.Stop:
                            effect.OnClientStop();
                            NetworkEx.Client.SendPacket(packet);
                            effect.Running = false;
                            RunningEffects.Remove(effect.Id);
                            HandleEffect(ChaosEffectPacket.CreateTakeDownPacket(effect.Id));
                            break;

                        // On Client TakeDown
                        //  - OnClientTakeDown()            CLIENT TAKEDOWN
                        //  - Send TakeDown Packet.         SEND SERVER TAKEDOWN PACKET
                        case EffectStage.TakeDown when effect.Running:
                            return;
                        case EffectStage.TakeDown:
                            effect.OnClientTakeDown(_capi);
                            NetworkEx.Client.SendPacket(packet);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                });

            if (_api.Side.IsServer())
                AsyncEx.Server.EnqueueAsyncTask(() =>
                {
                    var player = (IServerPlayer) _sapi.World.PlayerByUid(packet.PlayerUID);
                    switch (packet.Command)
                    {
                        // On Server Setup
                        //  - Set player for effect.
                        //  - OnServerSetup()               SERVER SETUP
                        case EffectStage.Setup when effect.Running:
                            return;
                        case EffectStage.Setup:
                            effect.SetPlayer(_sapi.World.PlayerByUid(packet.PlayerUID));
                            effect.OnServerSetup(player, _sapi);
                            break;

                        // On Server Start
                        //  - Notify player of Effect.
                        //  - OnServerStart()               SERVER START
                        //  - Running = true;
                        //  - Add as running effect.
                        case EffectStage.Start when effect.Running:
                            return;
                        case EffectStage.Start:
                            _sapi.SendIngameDiscovery(player, null, effect.Title());
                            _sapi.World.PlaySoundAt(new AssetLocation("sounds/effect/deepbell"), player.Entity, null, false, 32f, 0.5f);
                            effect.OnServerStart(player, _sapi);
                            if (effect.Duration is EffectDuration.Instant) return;
                            effect.Running = true;
                            RunningEffects.Add(effect.Id, effect);
                            break;

                        // On Server Tick
                        //  - OnServerTick()                SERVER TICK
                        case EffectStage.Tick when !effect.Running:
                            return;
                        case EffectStage.Tick:
                            effect.OnServerTick(packet.DeltaTime);
                            break;

                        // On Server Stop
                        //  - OnServerStop()                SERVER STOP
                        //  - Running = false;
                        //  - Remove as running effect.
                        case EffectStage.Stop when !effect.Running:
                            return;
                        case EffectStage.Stop:
                            effect.OnServerStop();
                            effect.Running = false;
                            RunningEffects.Remove(effect.Id);
                            break;

                        // On Server TakeDown
                        //  - OnServerTakeDown()            SERVER TAKEDOWN
                        case EffectStage.TakeDown when effect.Running:
                            return;
                        case EffectStage.TakeDown:
                            effect.OnServerTakeDown(player, _sapi);
                            _sapi.World.PlaySoundAt(new AssetLocation("sounds/effect/deepbell"), player.Entity, null, false, 32f, 0.5f);
                            player.SendMessage("Effect Ended.");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                });
        }
    }
}