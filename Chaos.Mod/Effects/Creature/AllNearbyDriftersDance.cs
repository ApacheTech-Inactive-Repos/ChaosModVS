using System.Collections.Generic;
using System.Linq;
using Chaos.Engine.Enums;
using Chaos.Engine.Primitives;
using JetBrains.Annotations;
using VintageMods.Core.Extensions;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Chaos.Mod.Effects.Creature
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AllNearbyDriftersDance : ChaosEffect
    {
        private IEnumerable<EntityAgent> _currentDrifters;
        private IEnumerable<EntityAgent> _prevDrifters;
        private EntityPlayer _player;

        public override EffectType EffectType => EffectType.Creature;
        public override EffectDuration Duration => EffectDuration.Short;

        public override void OnServerStart(IServerPlayer player, ICoreServerAPI sapi)
        {
            base.OnServerStart(player, sapi);

            _player = player.Entity;
            _prevDrifters = _currentDrifters = ChaosApi.Creatures.Server.GetAllNearbyCreatures(player.Entity.Pos.AsBlockPos, 25)
                .Where(p => p.Code.Path.Contains("drifter"));



            var dance = new AnimationMetaData { Animation = "Dance", Code = "dance" };
            foreach (var drifter in _currentDrifters)
            {
                if (!drifter.AnimManager.IsAnimationActive("dance"))
                    drifter.StartAnimation("dance");
            }
            (Player as IServerPlayer).SendMessage("AllNearbyDriftersDance Effect Started.");
        }

        public override void OnServerTick(float dt)
        {
            base.OnServerTick(dt);

            _currentDrifters = ChaosApi.Creatures.Server.GetAllNearbyCreatures(_player.Pos.AsBlockPos, 25)
                .Where(p => p.Code.Path.Contains("drifter"));

            var result = _prevDrifters.Where(p => _currentDrifters.All(p2 => p2.EntityId != p.EntityId));

            foreach (var drifter in result)
            {
                drifter.StopAnimation("dance");
            }

            foreach (var drifter in _currentDrifters)
            {
                drifter.Controls.StopAllMovement();
            }
        }

        public override void OnServerStop()
        {
            base.OnServerStop();

            foreach (var drifter in _currentDrifters)
            {
                drifter.Controls.StopAllMovement();
            }
            (Player as IServerPlayer).SendMessage("AllNearbyDriftersDance Effect Stopped.");
        }
    }
}