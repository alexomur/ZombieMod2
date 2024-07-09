namespace ZombieMod2
{
    using Exiled.API.Enums;
    using Exiled.API.Features;
    
    public class ZombieMod2 : Plugin<Config>
    {
        public static ZombieMod2 Instance { get; private set; }

        public override PluginPriority Priority { get; } = PluginPriority.Last;
        
        public EventHandler EventHandler { get; private set; }

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