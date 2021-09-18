using Chaos.Engine.Effects.Enums;
using JetBrains.Annotations;
using ProtoBuf;
using VintageMods.Core.Helpers;

namespace Chaos.Engine.Network.Messages
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields, SkipConstructor = true)]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class ChaosEffectPacket
    {
        private ChaosEffectPacket(string id, EffectStage command)
        {
            (EffectId, Command) = (id, command);
        }

        public string EffectId { get; set; }

        public EffectStage Command { get; set; }

        public float DeltaTime { get; set; }

        public string PlayerUID { get; set; } = ApiEx.Client.World.Player.PlayerUID;

        public void Send()
        {
            NetworkEx.Client.SendPacket(this);
        }

        public static ChaosEffectPacket CreateSetupPacket(string id)
        {
            return new(id, EffectStage.Setup);
        }

        public static ChaosEffectPacket CreateStartPacket(string id)
        {
            return new(id, EffectStage.Start);
        }

        public static ChaosEffectPacket CreateTickPacket(string id, float dt)
        {
            return new(id, EffectStage.Tick) {DeltaTime = dt};
        }

        public static ChaosEffectPacket CreateStopPacket(string id)
        {
            return new(id, EffectStage.Stop);
        }

        public static ChaosEffectPacket CreateTakeDownPacket(string id)
        {
            return new(id, EffectStage.TakeDown);
        }
    }
}