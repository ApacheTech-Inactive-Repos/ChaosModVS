using System.ComponentModel.Composition;
using Chaos.Engine.Contracts;
using JetBrains.Annotations;
using VintageMods.Core.Helpers;
using VintageMods.Core.IO.Extensions;
using Vintagestory.API.Datastructures;

namespace Chaos.Engine.API
{
    [Export]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers | ImplicitUseTargetFlags.WithInheritors)]
    public class ChaosAPI : IChaosAPI
    {
        public JsonObject GlobalConfig { get; } = ApiEx.Universal.GetModFile("global-config.json").AsRawJsonObject();

        public IChaosClientAPI Client { get; } = new ChaosClientAPI();
        public IChaosServerAPI Server { get; } = new ChaosServerAPI();
    }
}