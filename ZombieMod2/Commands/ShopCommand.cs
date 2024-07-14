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

    public static class Shop
    {
        private static Config Config => ZombieMod2.Instance.Config;

        public static List<Offer> GlobalShop = new List<Offer>() { };

        public static List<Offer> GenerateShop(Teams team)
        {
            GlobalShop = new List<Offer>() { };
            switch (team)
            {
                case Teams.MtfTeam:
                    GlobalShop.AddRange(Config.ItemShop);
                    GlobalShop.AddRange(Config.AmmoShop);
                    GlobalShop.AddRange(Config.MtfInfoShop);
                    GlobalShop.AddRange(Config.MtfPresetShop);
                    GlobalShop.AddRange(Config.MtfPerkShop);
                    break;
                case Teams.ZombieTeam:
                    GlobalShop.AddRange(Config.ZombieInfoShop);
                    GlobalShop.AddRange(Config.ZombiePresetShop);
                    GlobalShop.AddRange(Config.ZombiePerkShop);
                    break;
            }

            return GlobalShop;
        }
    }

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
         List<Offer> shop;
         if (role.GetTeam() == Team.SCPs)
         {
             shop = Shop.GenerateShop(Teams.ZombieTeam);
         }
         else
         {
             shop = Shop.GenerateShop(Teams.MtfTeam);
         }

         Type lastType = null;
         for (int i = 0; i < shop.Count; i++)
         {
             Offer offer = shop[i];
             if (offer.Product.GetType() != lastType)
                 response += $"--={offer.Product.GetType()} shop=--";
             response += $"  [{i}] {offer.Name}\n" +
                         $" Desc: {offer.Description}\n" +
                         $" Cost: {offer.Cost}\n";
             lastType = offer.Product.GetType();
         }
         return true;
     }

     public string Command { get; } = "shop";
     public string[] Aliases { get; } = { };
     public string Description { get; } = "Shows the shop";
     public bool SanitizeResponse { get; } = false;
     }
 }