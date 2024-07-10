using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using PlayerRoles;
using UnityEngine;
using Player = Exiled.API.Features.Player;

namespace ZombieMod2
{
    using Exiled.API.Interfaces;

    public class ItemOffer
    {
        public string Name { get; set; }
        public ItemType ItemType { get; set; }
        public float Cost { get; set; }
        
        public ItemOffer(string name, ItemType itemType, float cost)
        {
            this.Name = name;
            this.ItemType = itemType;
            this.Cost = cost;
        }
        public ItemOffer() {}
    }
    
    public class AmmoBox
    {
        public ushort Nato9 { get; set; }
        public ushort Nato556 { get; set; }
        public ushort Nato762 { get; set; }
        public ushort Ammo44Cal { get; set; }
        public ushort Ammo12Gauge { get; set; }

        public AmmoBox(ushort nato9 = 0, ushort nato556 = 0, ushort nato762 = 0, ushort ammo44Cal = 0, ushort ammo12Gauge = 0)
        {
            this.Nato9 = nato9;
            this.Nato556 = nato556;
            this.Nato762 = nato762;
            this.Ammo44Cal = ammo44Cal;
            this.Ammo12Gauge = ammo12Gauge;
        }
        public AmmoBox() {}

        public Dictionary<AmmoType, ushort> ToDictionary()
        {
            return new Dictionary<AmmoType, ushort>()
            {
                { AmmoType.Nato9, this.Nato9 },
                { AmmoType.Nato556, this.Nato556 },
                { AmmoType.Nato762, this.Nato762 },
                { AmmoType.Ammo44Cal, this.Ammo44Cal },
                { AmmoType.Ammo12Gauge, this.Ammo12Gauge }
            };
        }
    }

    public class AmmoOffer
    {
        public string Name { get; set; }
        public AmmoBox AmmoBox { get; set; }
        public float Cost { get; set; }

        public AmmoOffer(string name, AmmoBox ammoBox, float cost)
        {
            this.Name = name;
            this.AmmoBox = ammoBox;
            this.Cost = cost;
        }
        public AmmoOffer() {}
    }
    
    public class Perk
    {
        public string Name { get; set; }
        public List<Effect> Effects { get; set; }

        public Perk(string name, List<Effect> effects)
        {
            this.Name = name;
            this.Effects = effects;
        }
        public Perk() {}

        public void AddPerk(Player player)
        {
            player.SyncEffects(this.Effects);
        }
    }

    public class PerkOffer : Perk
    {
        public float Cost { get; set; }

        public PerkOffer(string name, List<Effect> effects, float cost) : base(name, effects)
        {
            this.Cost = cost;
        }
        public PerkOffer() {}
    }

    public class Preset
    {
        public string Name { get; set; }
        public List<ItemType> ItemTypes { get; set; }
        public List<Perk> Perks { get; set; }
        public AmmoBox AmmoBox { get; set; }
        public RoleTypeId RoleTypeId { get; set; }
        public float MaxHp { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Preset name</param>
        /// <param name="itemTypes">Items of the form ItemType</param>
        /// <param name="perks">Effect lists of the form Perk</param>
        /// <param name="ammoBox">Ammunition sets of all calibers</param>
        /// <param name="roleTypeId">MTF preset must be any Human role, Zombie preset must be any SCP role (Zombie-Human will break the money system)</param>
        /// <param name="maxHp">Optional parameter; This parameter for Zombie will increase depending on ZombieHpImprovementFactor and ZombieHpImprovementCount according to the formula kx+b every wave; Default is 100</param>
        public Preset(string name, List<ItemType> itemTypes, List<Perk> perks, AmmoBox ammoBox, RoleTypeId roleTypeId, float maxHp = 100)
        {
            this.Name = name;
            this.ItemTypes = itemTypes;
            this.Perks = perks;
            this.AmmoBox = ammoBox;
            this.RoleTypeId = roleTypeId;
            this.MaxHp = maxHp;
        }
        public Preset() {}

        /// <summary>
        /// Use if you need to completely clear the player and give a new preset
        /// </summary>
        public void SpawnPreset(Player player, RoomType roomType = RoomType.Unknown)
        {
            player.Role.Set(RoleTypeId);
            player.ClearInventory();
            if (roomType != RoomType.Unknown)
            {
                Room room = Room.List.FirstOrDefault(r => r.Type == roomType);
                if (room != null)
                    player.Position = room.Position + Vector3.up;
                else
                    Log.Warn($"Room {roomType} not found!");
            }

            foreach (var item in ItemTypes)
            {
                // Check if the player's inventory is full
                if (player.Inventory.UserInventory.Items.Count < 8)
                {
                    player.AddItem(item);
                }
                else
                {
                    // Create and drop the item at the player's position if the inventory is full
                    Item droppedItem = Item.Create(item);
                    droppedItem.CreatePickup(player.Position);
                }
            }
            foreach (var perk in Perks)
            {
                perk.AddPerk(player);
            }
            foreach (var ammo in AmmoBox.ToDictionary())
            {
                player.AddAmmo(ammo.Key, ammo.Value);
            }

            player.MaxHealth = MaxHp;
            player.Health = player.MaxHealth;
        }

        /// <summary>
        /// Adding new items, perks and ammo to player
        /// Will change the Role, but won't delete an inventory and won't change the location
        /// </summary>
        public void AddPreset(Player player)
        {
            if (player.Role != this.RoleTypeId)
                player.Role.Set(this.RoleTypeId, RoleSpawnFlags.None);
            foreach (var item in ItemTypes)
            {
                // Check if the player's inventory is full
                if (player.Inventory.UserInventory.Items.Count < 8)
                {
                    player.AddItem(item);
                }
                else
                {
                    // Create and drop the item at the player's position if the inventory is full
                    Item droppedItem = Item.Create(item);
                    droppedItem.CreatePickup(player.Position);
                }
            }
            foreach (var perk in Perks)
            {
                perk.AddPerk(player);
            }
            foreach (var ammo in AmmoBox.ToDictionary())
            {
                player.AddAmmo(ammo.Key, ammo.Value);
            }

            if (MaxHp > player.MaxHealth)
            {
                player.MaxHealth = MaxHp;
            }
            player.Health = player.MaxHealth;
        }
    }

    public class PresetOffer : Preset
    {
        public float Cost { get; set; }

        public PresetOffer(string name, List<ItemType> itemTypes, List<Perk> perks, AmmoBox ammoBox, RoleTypeId roleTypeId, float cost, float maxHp = 100) : base(name, itemTypes, perks, ammoBox, roleTypeId, maxHp)
        {
            this.Cost = cost;
        }
    }

    public class InfoOffer
    {
        public string Name { get; set; }
        public string BroadcastText { get; set; }
        public float Cost { get; set; }

        public InfoOffer(string name, string broadcastText, float cost)
        {
            this.Name = name;
            this.BroadcastText = broadcastText;
            this.Cost = cost;
        }
        public InfoOffer() {}
    }
    
    public sealed class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        
        [Description("MTF start preset")]
        public Preset StartMtfPreset { get; set; } = new Preset
        (
            name:"Standart MTF Preset",
            itemTypes:new List<ItemType>()
            {
                ItemType.GunCOM15,
                ItemType.Medkit,
                ItemType.GrenadeHE,
                ItemType.KeycardMTFOperative
            },
            perks:new List<Perk>()
            {
                new Perk
                (
                    name:"Temp Immortality",
                    effects: new List<Effect>()
                    {
                        new Effect
                        (
                            EffectType.MovementBoost,
                            duration: 10,
                            intensity: 40
                        ),
                        new Effect
                        (
                            EffectType.DamageReduction,
                            duration: 10,
                            intensity: 200
                        ),
                    }
                )
            },
            ammoBox:new AmmoBox
            (
                nato9:60
            ),
            roleTypeId:RoleTypeId.NtfPrivate
        );

        [Description("Zombie start preset")]
        public Preset StartZombiePreset { get; set; } = new Preset
        (
            name: "Standart Zombie Preset",
            itemTypes: new List<ItemType>() {},
            perks: new List<Perk>()
            {
                new Perk
                (
                    name:"Temp Immortality",
                    effects: new List<Effect>()
                    {
                        new Effect
                        (
                            EffectType.MovementBoost,
                            duration: 10,
                            intensity: 40
                        ),
                        new Effect
                        (
                            EffectType.DamageReduction,
                            duration: 10,
                            intensity: 200
                        ),
                    }
                )
            },
            ammoBox: new AmmoBox(),
            roleTypeId:RoleTypeId.Scp0492,
            maxHp:200
        );

        [Description("How much will a MTF player get from killing a zombie")]
        public float StartMtfMoneyPerKill { get; set; } = 100;

        [Description("How much will a Zombie player get from killing a MTF")]
        public float StartZombieMoneyPerKill { get; set; } = 500;
        
        [Description("How much will the amount of money earned increase with each wave\nFactor - for set the coefficient\nCount - for precise setting\nBoth options work simultaneously (by formula kx+b)")]
        public float MoneyImprovementFactor { get; set; } = 1.5f;
        public float MoneyImprovementCount { get; set; } = 100f;
        
        [Description("How much will a player lose from killing an ally")]
        public float PenaltyForTeamkill { get; set; } = 500;

        [Description("How much will all the players get for survived wave (both MTF and Zombies)")]
        public float MoneyPerWave { get; set; } = 50;

        [Description("How much HP will zombie get every wave\nFactor - for set the coefficient\nCount - for precise setting\nBoth options work simultaneously (by formula kx+b)")]
        public float ZombieHpImprovementFactor { get; set; } = 1f;
        public float ZombieHpImprovementCount { get; set; } = 100f;

        [Description("ItemTypes in shop and their cost\nYou can add your own offers (for example, create Shotgun offer)")]
        public List<ItemOffer> ItemShop { get; set; } = new List<ItemOffer>()
        {
            new ItemOffer(
                name: "Epsilon-11 Rifle",
                itemType: ItemType.GunE11SR,
                cost: 1000
                ),
            new ItemOffer(
                name: "Medkit",
                itemType: ItemType.Medkit,
                cost: 300
                )
        };

        [Description("Information about Jackpot in shop and their cost\nEveryone will know the paid info\nYou can NOT add your own offers\n0 - to disable offer\njackpot_mode must be true")]
        public List<InfoOffer> MtfInfoShop { get; set; } = new List<InfoOffer>()
        {
            new InfoOffer(
                name:"Jackpot's Appereance",
                broadcastText:"Jackpot is look like %ItemType%",
                cost:150
                ),
            new InfoOffer(
                name:"Jackpot's Zone",
                broadcastText:"Jackpot is in the %Zone%",
                cost:500
                ),
            new InfoOffer(
                name:"Jackpot's Room",
                broadcastText:"Jackpot is in the %Room%",
                cost:5000
                )
        };

        [Description("Information about Jackpot in shop and their cost\nEveryone will know the paid info\nYou can NOT add your own offers\n0 - to disable offer\njackpot_mode must be true")]
        public List<InfoOffer> ZombieInfoShop { get; set; } = new List<InfoOffer>()
        {
            new InfoOffer(
                name:"Appereance",
                broadcastText:"Jackpot is look like %ItemType%",
                cost:150
                ),
            new InfoOffer(
                name:"Zone",
                broadcastText:"Jackpot is in the %Zone%",
                cost:500
                ),
            new InfoOffer(
                name:"Room",
                broadcastText:"Jackpot is in the %Room%",
                cost:5000
                )
        };

        [Description("Any AmmoType in shop\nYou can add your own offers (for example, create 12g Big)")]
        public List<AmmoOffer> AmmoShop { get; set; } = new List<AmmoOffer>()
        {
            new AmmoOffer(
                name:"9x19 Tiny",
                ammoBox:new AmmoBox(nato9:60),
                cost:50
                ),
            new AmmoOffer(
                name:"9x19 Big",
                ammoBox:new AmmoBox(nato9:200),
                cost:150
                ),
            new AmmoOffer(
                name:"5.56x45 Tiny",
                ammoBox:new AmmoBox(nato556:60),
                cost:100
                ),
            new AmmoOffer(
                name:"5.56x45 Big",
                ammoBox:new AmmoBox(nato556:200),
                cost:300
                ),
        };
        
        [Description("Preset store with ready-made equipment\nNew items will be added to player's inventory,\nthose items that do not fit into the inventory will be dropped away\nYou can add your own offers (for example, create Machinegunner)")]
        public List<PresetOffer> PresetShop { get; set; } = new List<PresetOffer>()
        {
            new PresetOffer
            (
                name:"Bomber",
                itemTypes: new List<ItemType>()
                {
                    ItemType.ArmorCombat,
                    ItemType.GunCrossvec,
                    ItemType.GrenadeHE,
                    ItemType.GrenadeHE,
                    ItemType.GrenadeHE,
                    ItemType.Medkit
                },
                perks: new List<Perk>() {},
                new AmmoBox(nato9:120),
                cost:10000,
                roleTypeId:RoleTypeId.NtfPrivate
            ),
            new PresetOffer
            (
                name:"Medic",
                itemTypes: new List<ItemType>()
                {
                    ItemType.ArmorCombat,
                    ItemType.GunCrossvec,
                    ItemType.Adrenaline,
                    ItemType.GrenadeFlash,
                    ItemType.Medkit,
                    ItemType.Medkit
                },
                perks: new List<Perk>() {},
                new AmmoBox(nato9:120),
                cost:7500,
                roleTypeId:RoleTypeId.NtfSergeant
            )
        };
        
        
        [Description("EffectTypes in the Perk shop for MTF\nYou can add your own offers (for example, create Heavy guy using DamageReduction and Disabled/other)\nEvery perk will be deleted after death")]
        public List<PerkOffer> MtfPerkShop { get; set; } = new List<PerkOffer>()
        {
            new PerkOffer
            (
                name:"Close Runner",
                effects: new List<Effect>()
                {
                    new Effect(EffectType.Invigorated, duration:5, intensity:1),
                    new Effect(EffectType.MovementBoost, duration:10, intensity:80),
                },
                cost:1000
            )
        };

        [Description("EffectTypes in the Perk shop for Zombies\nYou can add your own offers (for example, create Heavy guy using DamageReduction and Disabled/other)\nEvery perk will be deleted after death")]
        public List<PerkOffer> ZombiePerkShop { get; set; } = new List<PerkOffer>()
        {
            new PerkOffer 
            (
                name:"Slippery",
                effects: new List<Effect>()
                {
                    new Effect(EffectType.Ghostly, duration:5, intensity:1)
                },
                cost:50
            )
        };

        [Description("The ratio of the number of MTF and Zombie at spawn at START of the game (MTF/Zombie)\n1 - for spawn only 1 zombie\n0 - for spawn only 1 MTF")]
        public float StartRatioSpawn { get; set; } = 0.8f;
        [Description("The ratio of the number of MTF and Zombie at spawn in the MIDDLE of the game (MTF/Zombie)\n1 - for spawn 0 zombie\n0 - for spawn 0 MTF")]
        public float MiddleRatioSpawn { get; set; } = 0.5f;

        [Description("With Infection mode enabled, every killed MTF will instantly become a zombie")]
        public bool InfectionMode { get; set; } = true;

        [Description("Duration of the preparation stage before the first wave (secs)")]
        public int StartWavePause { get; set; } = 30;
        [Description("Duration of a wave (secs)")]
        public int WaveDuration { get; set; } = 180;
        [Description("Pause between waves for shopping and movement (secs)")]
        public int WavePause { get; set; } = 60;
        
        [Description("MTF spawn room and weighted probability of this room")]
        public Dictionary<RoomType, int> MtfSpawns { get; set; } = new Dictionary<RoomType, int>()
        {
            { RoomType.Hcz106, 2 },
            { RoomType.Hcz079, 1 }
        };

        [Description("Zombies spawn room and weighted probability of this room")]
        public Dictionary<RoomType, int> ZombieSpawns { get; set; } = new Dictionary<RoomType, int>()
        {
            {RoomType.EzCafeteria, 1},
            {RoomType.Lcz173, 1}
        };

        [Description(
        "\n\nJackpot mode adds an item(s) (Jackpot) to a specific room,\n" +
        "which the MTF will have to find and take to one of the specified rooms (Exctracts)\n" +
        "This will be considered a victory for MTF\n" +
        "If Jackpot mode is false, the game will continue until the entire MTF dies")]
        public bool JackpotMode { get; set; } = true;
        
        [Description("ItemTypes to be used as Jackpot and weighted probability of this ItemType\nEach ItemType can be used several times")]
        public Dictionary<ItemType, int> JackpotAppearences { get; set; } = new Dictionary<ItemType, int>()
        {
            { ItemType.Coin, 1 },
            { ItemType.KeycardO5, 3 },
            { ItemType.GunCom45, 1 }
        };
        
        [Description("RoomTypes to be used as Jackpot spawns and their weighted probability\nEach spawn can be used several times")]
        public Dictionary<RoomType, int> JackpotSpawns { get; set; } = new Dictionary<RoomType, int>()
        {
            { RoomType.Hcz049, 1},
            { RoomType.Lcz914, 1},
            { RoomType.EzIntercom, 1}
        };

        [Description("Number of Jackpots")] 
        public int JackpotCount { get; set; } = 1;

        [Description("How much will all MTFs receive for completing the Jackpot")] 
        public int MoneyPerJackpot { get; set; } = 3000;

        [Description("Extracts and their weighted probability\nEach extract can be used several times")]
        public Dictionary<RoomType, int> ExtractsSpawns { get; set; } = new Dictionary<RoomType, int>()
        {
            { RoomType.Surface, 1 },
            { RoomType.LczToilets, 1 }
        };

        [Description("Number of Extracts")] 
        public int ExtractsCount { get; set; } = 2;
    }
}