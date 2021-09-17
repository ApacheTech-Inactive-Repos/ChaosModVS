using Chaos.Engine.Enums;
using Chaos.Engine.Primitives;
using JetBrains.Annotations;
using Vintagestory.API.Client;
using Vintagestory.API.Server;

namespace Chaos.Mod.Effects.Meta
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class NoChaos : ChaosEffect
    {
        public override EffectType EffectType => EffectType.Meta;
        public override EffectDuration Duration => EffectDuration.Custom;

        public override void OnServerStart(IServerPlayer player, ICoreServerAPI sapi)
        {
            base.OnServerStart(player, sapi);
            ChaosApi.Server.Meta.DisableEffects = true;
        }

        public override void OnServerStop()
        {
            ChaosApi.Server.Meta.DisableEffects = false;
            base.OnServerStop();
        }

        public override void OnClientStart(ICoreClientAPI capi)
        {
            base.OnClientStart(capi);
            ChaosApi.Client.Meta.DisableEffects = true;
        }

        public override void OnClientStop()
        {
            ChaosApi.Client.Meta.DisableEffects = false;
            base.OnClientStop();
        }
    }
}