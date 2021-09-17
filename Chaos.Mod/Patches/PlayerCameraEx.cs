using System;
using HarmonyLib;
using JetBrains.Annotations;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace Chaos.Mod.Patches
{
    [HarmonyPatch]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class PlayerCameraEx
    {
        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(EntityApplyGravity), "DoApply")]
        private static bool EntityApplyGravity_DoApply_Pretfix(EntityApplyGravity __instance, float dt, Entity entity,
            EntityPos pos, EntityControls controls)
        {
            if (entity is not EntityPlayer player) return true;
            if (player.Player.PlayerUID != (player.Api as ICoreClientAPI)?.World.Player.PlayerUID) return true;
            if (!player.FeetInLiquid) return true;
            pos.Motion.Y += (GlobalConstants.GravityPerSecond + Math.Max(0.0, -0.014999999664723873 * pos.Motion.Y)) *
                            (entity.FeetInLiquid ? 0.33f : 1f) * dt;
            return false;
        }
    }
}