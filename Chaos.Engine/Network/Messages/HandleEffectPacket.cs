using ProtoBuf;

namespace Chaos.Engine.Network.Messages
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields, SkipConstructor = true)]
    public sealed class HandleEffectPacket
    {
        public string EffectId { get; set; }

        public HandleEffectPacket(string id)
        {
            EffectId = id;
        }
    }
}