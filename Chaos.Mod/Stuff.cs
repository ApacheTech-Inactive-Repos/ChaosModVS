using System;
using VintageMods.Core.Extensions;
using VintageMods.Core.Helpers;
using VintageMods.Core.Reflection;
using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace Chaos.Mod
{
    /// <summary>
    ///     Class Stuff.
    /// </summary>
    public static class Stuff
    {
        private static float _rollAmount;
        private static float _roll;

        public static void RollClockwise(float byVal = 1)
        {
            ClampRoll(ref _rollAmount, byVal);
        }

        public static void RollAntiClockwise(float byVal = 1)
        {
            ClampRoll(ref _rollAmount, -byVal);
        }

        public static void ResetRoll()
        {
            RollClockwise(_roll);
        }

        private static float ClampRoll(ref float roll, float byVal)
        {
            return roll += byVal switch
            {
                >= 360f => roll - 360f,
                <= 0f => roll + 360f,
                _ => roll
            };
        }

        private static void TeleportToPoint(EntityPos cameraPoint, ICoreClientAPI capi)
        {
            var game = capi.AsClientMain();
            game.EntityPlayer.Pos.X = cameraPoint.X;
            game.EntityPlayer.Pos.Y = cameraPoint.Y;
            game.EntityPlayer.Pos.Z = cameraPoint.Z;
            game.SetField("mouseYaw", game.EntityPlayer.Pos.Yaw = cameraPoint.Yaw);
            game.SetField("mousePitch", game.EntityPlayer.Pos.Pitch = cameraPoint.Pitch);
            game.EntityPlayer.Pos.Roll = cameraPoint.Roll;
        }

        /// <summary>
        ///     Inverse Lerp Function (Freya Holmér)
        ///     Used to change where an interpolated value starts, and ends, along a path, between a - b.
        ///     Use Case: Changing where a colour gradient starts, giving a solid block of colour,
        ///     up to a defined point, and then gradating to another colour.
        /// </summary>
        /// <param name="a">The start of the path between point A and point B.</param>
        /// <param name="b">The end of the path between point A and point B.</param>
        /// <param name="value">The point.</param>
        /// <returns>The point along the vector, from a to b, where the interpolation begins or ends.</returns>
        private static float InverseLerp(float a, float b, float value)
        {
            return GameMath.Clamp((value - a) / (b - a), a, b);
        }

        public static void GenerateCircles(string direction, int radius, int laps)
        {
            var capi = ApiEx.Client;
            var game = ApiEx.Client.AsClientMain();
            var num = Math.Sqrt(2.0) / 2.0;
            var vec3d = new Vec3d(1.0, 0.0, 0.0);
            var array = new[]
            {
                vec3d,
                new(num, 0.0, -num),
                new(0.0, 0.0, -1.0),
                new(-num, 0.0, -num),
                new(-1.0, 0.0, 0.0),
                new(-num, 0.0, num),
                new(0.0, 0.0, 1.0),
                new(num, 0.0, num)
            };
            var array2 = new[]
            {
                vec3d,
                new(num, 0.0, num),
                new(0.0, 0.0, 1.0),
                new(-num, 0.0, num),
                new(-1.0, 0.0, 0.0),
                new(-num, 0.0, -num),
                new(0.0, 0.0, -1.0),
                new(num, 0.0, -num)
            };
            var array3 = direction.Equals("right") ? array : direction.Equals("left") ? array2 : null;
            if (array3 == null)
            {
                game.ShowChatMessage(
                    ".cam circle [radius] [circles] [right/left] - creates a circle of points around the player's current location.");
                return;
            }

            try
            {
                var pos = capi.World.Player.Entity.Pos;
                for (var i = 0; i < laps; i++)
                    foreach (var vec3d2 in array3)
                    {
                        var entityPos = new EntityPos(
                            pos.X + vec3d2.X * radius,
                            pos.Y,
                            pos.Z + vec3d2.Z * radius,
                            pos.Yaw,
                            pos.Pitch,
                            pos.Roll);
                        //AddPoint(entityPos);
                    }

                //ClosePath();
                game.ShowChatMessage(
                    "Circles have been created around your current position, consider setting a target.");
            }
            catch (Exception)
            {
                game.ShowChatMessage(
                    ".cam circle [radius] [circles] [right/left] - creates a circle of points around the player's current location.");
            }
        }
    }
}