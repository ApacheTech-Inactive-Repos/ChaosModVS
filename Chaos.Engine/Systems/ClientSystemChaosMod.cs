using Chaos.Engine.Contracts;
using JetBrains.Annotations;
using Vintagestory.API.Client;
using Vintagestory.Client.NoObf;

namespace Chaos.Engine.Systems
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class ClientSystemChaosMod : ClientSystem, IHasClientAccess
    {
        public ClientSystemChaosMod(ClientMain game) : base(game)
        {
            Capi = (ICoreClientAPI) game.Api;
            Capi.RegisterCommand("chaos", "", "", (_, _) =>
            {
                Capi.ShowChatMessage("Client System Loaded.");
            });
        }

        public override string Name => "chaos";

        public ICoreClientAPI Capi { get; }

        public override bool CaptureAllInputs()
        {
            return false;
        }

        public override EnumClientSystemType GetSystemType()
        {
            return EnumClientSystemType.Misc;
        }
    }
}