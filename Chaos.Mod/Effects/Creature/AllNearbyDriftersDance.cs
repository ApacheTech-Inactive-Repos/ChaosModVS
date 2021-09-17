using System.Collections.Generic;
using System.Linq;
using Chaos.Engine.Enums;
using Chaos.Engine.Primitives;
using Chaos.Mod.Tasks;
using VintageMods.Core.Extensions;
using VintageMods.Core.Reflection;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace Chaos.Mod.Effects.Creature
{
    public class AllNearbyDriftersDance : ChaosEffect
    {
        private IEnumerable<EntityAgent> _currentDrifters;
        private EntityPlayer _player;
        private IEnumerable<EntityAgent> _prevDrifters;
        private Dictionary<EntityAgent, List<IAiTask>> _tempTasks = new();

        public override EffectType EffectType => EffectType.Creature;
        public override EffectDuration Duration => EffectDuration.Short;

        public override void OnServerStart(IServerPlayer player, ICoreServerAPI sapi)
        {
            base.OnServerStart(player, sapi);

            _player = player.Entity;
            _prevDrifters = _currentDrifters = ChaosApi.Server.Creatures
                .GetAllNearbyCreatures(player.Entity.Pos.AsBlockPos, 25)
                .Where(p => p.Code.Path.Contains("drifter"));
            foreach (var drifter in _currentDrifters) PauseAllTasks(drifter);
        }

        public override void OnServerTick(float dt)
        {
            base.OnServerTick(dt);

            _currentDrifters = ChaosApi.Server.Creatures.GetAllNearbyCreatures(_player.Pos.AsBlockPos, 25)
                .Where(p => p.Code.Path.Contains("drifter")).ToList();

            // New drifters.
            var newDrifter = _prevDrifters.Where(p => _currentDrifters.All(p2 => p2.EntityId != p.EntityId));
            foreach (var drifter in newDrifter) PauseAllTasks(drifter);

            // Drifters no longer in range.
            var outOfRange = _currentDrifters.Where(p => _prevDrifters.All(p2 => p2.EntityId != p.EntityId));
            foreach (var drifter in outOfRange) ResumeAllTasks(drifter);

            _prevDrifters = _currentDrifters;
        }

        public override void OnServerStop()
        {
            foreach (var drifter in _prevDrifters) ResumeAllTasks(drifter);
            base.OnServerStop();
        }

        private void PauseAllTasks(EntityAgent drifter)
        {
            drifter.Controls.StopAllMovement();
            var ai = drifter.GetBehavior<EntityBehaviorTaskAI>().taskManager;
            var tasks = ai.GetField<List<IAiTask>>("Tasks").Clone();
            _tempTasks.Add(drifter, tasks.Clone());
            foreach (var aiTask in tasks
                .Where(aiTask => aiTask is not null)
                .Where(aiTask => aiTask.GetType() != typeof(AiTaskDance)))
            {
                ai.StopTask(aiTask.GetType());
                ai.RemoveTask(aiTask);
            }
            ai.GetTask<AiTaskDance>().Active = true;
        }

        private void ResumeAllTasks(EntityAgent drifter)
        {
            drifter.Controls.StopAllMovement();
            var ai = drifter.GetBehavior<EntityBehaviorTaskAI>().taskManager;
            foreach (var aiTask in _tempTasks[drifter]
                .Where(aiTask => aiTask is not null)
                .Where(aiTask => aiTask.GetType() != typeof(AiTaskDance)))
                ai.AddTask(aiTask);
            _tempTasks.Remove(drifter);
            ai.GetTask<AiTaskDance>().Active = false;
            ai.StopTask(typeof(AiTaskDance));
        }
    }
}

// TODO: Only Surface Drifters and Deep Drifters, dance. It's possible the names of the cuboids have been changed for the other mobs.