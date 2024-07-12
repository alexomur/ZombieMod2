using System;
using System.Collections.Generic;
using CommandSystem;
using Exiled.API.Features;
using PlayerRoles;
using RemoteAdmin;
using Utils.NonAllocLINQ;

namespace ZombieMod2.Commands
{
    /*
     * I wanted to make a very beautiful system here, which will be from
     * one function to generate a List of all possible purchases to ensure that the Indexes in both stores are the same
     * But I changed my mind
     * 
     * Now this is just shitty code
     * In the ShopCommand and BuyCommand commands in the Execute assembly
     * the set of possible purchases must be
     * the same: according to the principle below
     
     * For MTF
     * First, create an item store
     * Then an information store is created
     * Then an ammunition store is created
     * Behind it is a preset store
     * And a perk store
     
     * For Zombie
     */
    
    [CommandHandler(typeof(ClientCommandHandler))]
    public class ShopCommand : ICommand
    {
        private Config Config => ZombieMod2.Instance.Config;
        
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player;
            RoleTypeId role;
            if (sender is PlayerCommandSender playerCommandSender)
            {
                player = Player.Get(playerCommandSender.ReferenceHub);
                if (player != null)
                {
                    role = player.Role;
                }
                else
                {
                    response = $"Sender ({sender.LogName}) is not found!";
                    Log.Warn($"Sender ({sender.LogName}) is not found! Command: {this.Command}");
                    return false;
                }
            }
            else
            {
                response = $"Command ({this.Command}) can only be executed by a player. Sender: {sender.LogName}";
                return false;
            }

            int i = 0;
            if (player.Role.Type.IsHuman())
            {
                response = "MTF shop:";
                for (; i < Config.ItemShop.Count; i++)
                {
                    response += $"[{i}] {Config.ItemShop[i].Name}";
                }
            }
            
        }

        public string Command { get; } = "shop";
        public string[] Aliases { get; } = { };
        public string Description { get; } = "Shows the shop";
        public bool SanitizeResponse { get; } = false;
    }
}