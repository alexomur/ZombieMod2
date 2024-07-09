using System.Collections.Generic;
using System.Linq;
using Exiled.Events.Handlers;

namespace ZombieMod2
{
    using Exiled.API.Enums;
    using Exiled.API.Features;
    
    public class ZombieMod2 : Plugin<Config>
    {
        public static ZombieMod2 Instance { get; private set; }

        public override PluginPriority Priority { get; } = PluginPriority.Last;
        
        public EventHandler EventHandler { get; private set; }

        public Dictionary<Player, float> Balances =>
            Player.List.ToDictionary(player => player, player => 0f);

        public List<Player> Players =>
            Player.List.ToList();

        public int WavesPassed = 0;
        
        public override void OnEnabled()
        {
            Instance = this;
            EventHandler = new EventHandler();
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            EventHandler = null;
            base.OnDisabled();
        }
    }
}