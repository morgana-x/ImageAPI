using CommandSystem;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ImageAPI.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ImageUrl : ParentCommand
    {

        public ImageUrl() => LoadGeneratedCommands();

        public override string Command => "imageurl";

        public override string[] Aliases => new string[] { "imageurl", "imgurl" };

        public override string Description => "Spawn image url test";
        public override void LoadGeneratedCommands() { }

        public void spawnImageDownloaded(string ImageFile, Vector3 Position, Transform rotationtransform = null)
        {
            Plugin.Instance._imageApi.spawnImage(ImageFile, Position, rotationTransform: rotationtransform);
            Log.Debug("spawned " + ImageFile);
        }
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Exiled.API.Features.Player player = Exiled.API.Features.Player.Get(sender);
            //string imageFile = "neko.png";
            string url = arguments.ToList()[0];
            if (url == null)
            {
                response = "Please provide a valid url";
                return false;
            }
            Plugin.Instance._imageApi.downloadImagePosition(url, player.Position + (player.CameraTransform.forward * 2), rotationTransform:player.CameraTransform, callBack: spawnImageDownloaded);
            response = "Spawning image. Please wait while it downloads if needed.";
            return true;
        }
    }
}
