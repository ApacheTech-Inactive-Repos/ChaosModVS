using Chaos.Engine.Contracts;
using JetBrains.Annotations;
using VintageMods.Core.Extensions;
using Vintagestory.API.Server;
using Vintagestory.Server;

namespace Chaos.Engine.Systems
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class ServerSystemChaosMod : ServerSystem, IHasServerAccess
    {
        public ServerSystemChaosMod(ServerMain server) : base(server)
        {
            Sapi = (ICoreServerAPI)server.Api;
            Sapi.RegisterCommand("chaos", "", "", (player, _, _) =>
            {
                player.SendMessage("Server System Loaded.");
            });
        }

        public ICoreServerAPI Sapi { get; }
    }
}