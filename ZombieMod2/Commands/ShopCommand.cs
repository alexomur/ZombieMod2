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
     * First, create an item shop
     * Then an ammunition shop is created
     * Then an information shop is created
     * Behind it is a preset shop
     * And a perk shop
     
     * For Zombie
     * Information shop
     * Behind it is a preset shop
     * And a perk shop
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

            response = "";
            int i = 0;
            int j = 0;

            // If player is a human, its building its own shop with items and ammo
            if (player.Role.Type.IsHuman())
            {
                /*
                 * Can be changed to translation system
                 */
                response += "MTF shop:\n--=Item shop=--";
                for (; i < Config.ItemShop.Count; i++)
                {
                    response += $"\n[{i}] {Config.ItemShop[i].Name}\n- {Config.ItemShop[i].Description}\nCost: {Config.ItemShop[i].Cost}";
                }
                response += "\n--=Ammo shop=--";
                j = i;
                for (; i < Config.AmmoShop.Count + j; i++)
                {
                    response += $"\n[{i}] {Config.AmmoShop[i].Name}\n- {Config.AmmoShop[i].Description}\nCost: {Config.AmmoShop[i].Cost}";
                }
                response += "\n--=Info shop=--";
                j = i;
                for (; i < Config.MtfInfoShop.Count + j; i++)
                {
                    response += $"\n[{i}] {Config.MtfInfoShop[i].Name}\n- {Config.MtfInfoShop[i].Description}\nCost: {Config.MtfInfoShop[i].Cost}";
                }
                response += "\n--=Preset shop=--";
                j = i;
                for (; i < Config.MtfPresetShop.Count + j; i++)
                {
                    response += $"\n[{i}] {Config.MtfPresetShop[i].Name}\n- {Config.MtfPresetShop[i].Description}\nCost: {Config.MtfPresetShop[i].Cost}";
                }
                response += "\n--=Perk shop=--";
                j = i;
                for (; i < Config.MtfPresetShop.Count + j; i++)
                {
                    response += $"\n[{i}] {Config.MtfPresetShop[i].Name}\n- {Config.MtfPresetShop[i].Description}\nCost: {Config.MtfPresetShop[i].Cost}";
                }
            } else
            {
                response += "\n--=Info shop=--";
                j = i;
                for (; i < Config.ZombieInfoShop.Count + j; i++)
                {
                    response += $"\n[{i}] {Config.ZombieInfoShop[i].Name}\n- {Config.ZombieInfoShop[i].Description}\nCost: {Config.ZombieInfoShop[i].Cost}";
                }
                response += "\n--=Preset shop=--";
                j = i;
                for (; i < Config.ZombiePresetShop.Count + j; i++)
                {
                    response += $"\n[{i}] {Config.ZombiePresetShop[i].Name}\n- {Config.ZombiePresetShop[i].Description}\nCost: {Config.ZombiePresetShop[i].Cost}";
                }
                response += "\n--=Perk shop=--";
                j = i;
                for (; i < Config.ZombiePresetShop.Count + j; i++)
                {
                    response += $"\n[{i}] {Config.ZombiePresetShop[i].Name}\n- {Config.ZombiePresetShop[i].Description}\nCost: {Config.ZombiePresetShop[i].Cost}";
                }
            }
            return true;
        }

        public string Command { get; } = "shop";
        public string[] Aliases { get; } = { };
        public string Description { get; } = "Shows the shop";
        public bool SanitizeResponse { get; } = false;
    }
}