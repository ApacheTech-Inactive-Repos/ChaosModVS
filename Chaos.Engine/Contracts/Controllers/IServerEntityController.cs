using System.Collections.Generic;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace Chaos.Engine.Contracts.Controllers
{
    public interface IServerEntityController
    {
        IEnumerable<Entity> GetAllNearbyEntities(BlockPos pos, int radius, string[] blacklist = null);
    }
}