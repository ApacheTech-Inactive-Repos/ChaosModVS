﻿using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

namespace Chaos.Mod.Extensions
{
    public static class ServerPlayerEx
    {
        public static void SendMessage(this IServerPlayer player, string message)
        {
            player.SendMessage(GlobalConstants.CurrentChatGroup, message, EnumChatType.Notification);
        }
    }
}