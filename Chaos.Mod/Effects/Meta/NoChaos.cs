using Chaos.Engine.Enums;
using Chaos.Engine.Primitives;
using Vintagestory.API.Client;
using Vintagestory.API.Server;

namespace Chaos.Mod.Effects.Meta
{
    public class NoChaos : ChaosEffect
    {
        public override EffectType EffectType => EffectType.Meta;
        public override EffectDuration Duration => EffectDuration.Long;

        public override void OnServerStart(IServerPlayer player, ICoreServerAPI sapi)
        {
            base.OnServerStart(player, sapi);
            ChaosApi.Meta.Server.DisableEffects = true;
        }

        public override void OnServerStop()
        {
            ChaosApi.Meta.Server.DisableEffects = false;
            base.OnServerStop();
        }

        public override void OnClientStart(ICoreClientAPI capi)
        {
            base.OnClientStart(capi);
            ChaosApi.Meta.Client.DisableEffects = true;
        }

        public override void OnClientStop()
        {
            ChaosApi.Meta.Client.DisableEffects = false;
            base.OnClientStop();
        }
    }
}