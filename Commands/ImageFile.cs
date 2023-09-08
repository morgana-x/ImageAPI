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
    public class ImageFile : ParentCommand
    {

        public ImageFile() => LoadGeneratedCommands();

        public override string Command => "image";

        public override string[] Aliases => new string[] { "image", "img", "imgfile" };

        public override string Description => "Spawn image using a filename";
        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Exiled.API.Features.Player player = Exiled.API.Features.Player.Get(sender);
            string imageFile = arguments.ToList()[0];
            Log.Debug(imageFile);
            if (imageFile == null) 
            {
                response = "Please provide an image file name.\nYour images are located in " + Plugin.Instance._imageApi.getImageFolder();
                return false;
            }
            Plugin.Instance._imageApi.spawnImage(imageFile, player.Position + (player.CameraTransform.forward * 2),  rotationTransform:player.CameraTransform);
            Log.Debug("Spawned image \"" + imageFile + "\" for " + player.DisplayNickname);
            response = "Spawning image";
            return true;
        }
    }
}
