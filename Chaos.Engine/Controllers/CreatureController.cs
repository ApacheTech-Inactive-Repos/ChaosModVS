using System.Collections.Generic;
using Chaos.Engine.Primitives;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace Chaos.Engine.Controllers
{
    public class CreatureController : ControllerBase<CreatureController.ServerSide, CreatureController.ClientSide>
    {
        public CreatureController(ICoreAPI api) : base(api)
        {
        }


        public class ClientSide
        {

        }


        public class ServerSide
        {
            public IEnumerable<EntityAgent> GetAllNearbyCreatures(BlockPos pos, int radius)
            {
                var list = new List<EntityAgent>();
                var entities = Sapi.World.LoadedEntities.Values;
                foreach (var entity in entities)
                {
                    if (entity is not (EntityAgent {Alive: true} agent and not EntityPlayer)) continue;
                    if (agent.Pos.AsBlockPos.InRangeHorizontally(pos.X, pos.Z, radius))
                        list.Add(agent);
                }

                return list;
            }

            public IEnumerable<EntityAgent> GetAllNearbyCorpses(BlockPos pos, int radius)
            {
                var list = new List<EntityAgent>();
                var entities = Sapi.World.LoadedEntities.Values;
                foreach (var entity in entities)
                {
                    if (entity is not (EntityAgent {Alive: false} agent and not EntityPlayer)) continue;
                    if (agent.Pos.AsBlockPos.InRangeHorizontally(pos.X, pos.Z, radius))
                        list.Add(agent);
                }

                return list;
            }
        }
    }
}