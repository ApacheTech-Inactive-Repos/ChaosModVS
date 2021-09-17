using System.Collections.Generic;
using Chaos.Engine.Contracts.Controllers;
using VintageMods.Core.Extensions;
using VintageMods.Core.Helpers;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace Chaos.Engine.Controllers
{
    public class ServerCreatureController : IServerCreatureController, ICreatureController
    {
        public IEnumerable<EntityAgent> GetAllNearbyCreatures(BlockPos pos, int radius, string[] blacklist = null)
        {
            var list = new List<EntityAgent>();
            var entities = ApiEx.Server.World.LoadedEntities.Values;
            foreach (var entity in entities)
            {
                if (entity.Code.Path.ContainsAny(blacklist ?? new string[] { })) continue;
                if (entity is not (EntityAgent {Alive: true} agent and not EntityPlayer)) continue;
                if (agent.Pos.AsBlockPos.InRangeHorizontally(pos.X, pos.Z, radius))
                    list.Add(agent);
            }

            return list;
        }

        public IEnumerable<EntityAgent> GetAllNearbyCorpses(BlockPos pos, int radius, string[] blacklist = null)
        {
            var list = new List<EntityAgent>();
            var entities = ApiEx.Server.World.LoadedEntities.Values;
            foreach (var entity in entities)
            {
                if (entity.Code.Path.ContainsAny(blacklist ?? new string[] { })) continue;
                if (entity is not (EntityAgent {Alive: false} agent and not EntityPlayer)) continue;
                if (agent.Pos.AsBlockPos.InRangeHorizontally(pos.X, pos.Z, radius))
                    list.Add(agent);
            }

            return list;
        }
    }
}