using System;
using System.Collections.Generic;
using System.Linq;
using Chaos.Engine.Contracts.Controllers;
using VintageMods.Core.Extensions;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace Chaos.Engine.Controllers
{
    public class ServerEntityController : IServerEntityController, IEntityController
    {
        public IEnumerable<Entity> GetAllNearbyEntities(BlockPos pos, int radius, string[] blacklist = null)
        {
            var entities = ApiEx.Server.World.LoadedEntities.Values;

            return entities
                .Where(entity => entity is not EntityPlayer)
                .Where(entity => !entity.Code.Path.ContainsAny(blacklist ?? new string[] { }))
                .Where(entity => entity.Pos.AsBlockPos.InRangeHorizontally(pos.X, pos.Z, radius))
                .ToList();
        }
    }
}