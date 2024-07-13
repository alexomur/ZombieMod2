using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using System.Collections.Generic;
using System.Linq;
using PlayerRoles;


namespace ZombieMod2.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class BuyCommand : ICommand
    {
        private Config Config => ZombieMod2.Instance.Config;
        
        public bool Execute(ArraySegment<string> argumants, ICommandSender sender, out string response)
        {
            Int32.TryParse(argumants.ToList()[0], out int index);



            response = "Ok!";
            return true;
        }

        public string Command { get; } = "buy";
        public string[] Aliases { get; } = {};
        public string Description { get; } = "buy <id>";

        public bool SanitizeResponse { get; } = false;
    }
}