using JetBrains.Annotations;
using ProtoBuf;
using VintageMods.Core.Extensions;

namespace Chaos.Engine.Network.Messages
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields, SkipConstructor = true)]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class HandleEffectPacket
    {
        private HandleEffectPacket(string id, EffectCommand command)
        {
            (EffectId, Command) = (id, command);
        }

        public string EffectId { get; set; }

        public EffectCommand Command { get; set; }

        public float DeltaTime { get; set; }

        public string PlayerUID { get; set; } = ApiEx.Client.World.Player.PlayerUID;

        public void Send()
        {
            NetworkEx.Client.SendPacket(this);
        }

        public static HandleEffectPacket CreateSetupPacket(string id)
        {
            return new(id, EffectCommand.Setup);
        }

        public static HandleEffectPacket CreateStartPacket(string id)
        {
            return new(id, EffectCommand.Start);
        }
        public static HandleEffectPacket CreateTickPacket(string id, float dt)
        {
            return new(id, EffectCommand.Tick) { DeltaTime = dt };
        }

        public static HandleEffectPacket CreateStopPacket(string id)
        {
            return new(id, EffectCommand.Stop);
        }

        public static HandleEffectPacket CreateTakeDownPacket(string id)
        {
            return new(id, EffectCommand.TakeDown);
        }
    }
}