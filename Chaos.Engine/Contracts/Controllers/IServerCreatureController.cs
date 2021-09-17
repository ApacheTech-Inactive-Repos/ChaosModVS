using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace Chaos.Engine.Contracts.Controllers
{
    public interface IServerCreatureController
    {
        IEnumerable<EntityAgent> GetAllNearbyCreatures(BlockPos pos, int radius, string[] blacklist = null);
        IEnumerable<EntityAgent> GetAllNearbyCorpses(BlockPos pos, int radius, string[] blacklist = null);
    }
}