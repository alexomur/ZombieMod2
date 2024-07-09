namespace ZombieMod2
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;
    
    public class EventHandler
    {
        private void OnVerified(VerifiedEventArgs ev) => Log.Info($"{ev.Player} has joined the server!");
        
        public EventHandler()
        {
            Exiled.Events.Handlers.Player.Verified += OnVerified;
        }

        ~EventHandler()
        {
            Exiled.Events.Handlers.Player.Verified -= OnVerified;
        }

    }
}