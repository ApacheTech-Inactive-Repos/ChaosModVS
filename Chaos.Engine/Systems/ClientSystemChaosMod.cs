using Vintagestory.Client.NoObf;

namespace Chaos.Engine.Systems
{
    public sealed class ClientSystemChaosMod : ClientSystem
    {
        public ClientSystemChaosMod(ClientMain game) : base(game)
        {
        }

        public override EnumClientSystemType GetSystemType()
        {
            return EnumClientSystemType.Misc;
        }

        public override string Name => "ChaosMod";
    }
}