using CommandSystem;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageAPI.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ImageClear : ParentCommand
    {

        public ImageClear() => LoadGeneratedCommands();

        public override string Command => "imageclear";

        public override string[] Aliases => new string[] { "clearimage", "imgclear" };

        public override string Description => "Clears all images";
        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            //Exiled.API.Features.Player player = Exiled.API.Features.Player.Get(sender);
            Plugin.Instance._imageApi.deleteAllImages();
            response = "Deleting all spawned/spawning images.";
            return true;
        }
    }
}
