using Exiled.API.Features;
using Exiled.Events.Handlers;
using System.IO;

namespace ImageAPI
{
    public sealed class Plugin : Plugin<Config>
    {
        public override string Author => "morgana";

        public override string Name => "ImageAPI";

        public override string Prefix => Name;

        public static Plugin Instance;

        private EventHandlers _handlers;

        public ImageAPI _imageApi;

        public override void OnEnabled()
        {
            Instance = this;

            RegisterEvents();

            base.OnEnabled();
            string imageAPIFolder = _imageApi.getImageFolder();
            if (!Directory.Exists(imageAPIFolder))
            {
                Log.Warn("\"" + imageAPIFolder + "\" not found! Creating...");
                Directory.CreateDirectory(imageAPIFolder);
            }
            Exiled.API.Features.Server.IsHeavilyModded = true;
            Log.Warn("Set IsHeavilyModded to true due to use of primitives, See Northwoods VSR for more detail!");
        }

        public override void OnDisabled()
        {
            UnregisterEvents();

            Instance = null;

            base.OnDisabled();
        }

        private void RegisterEvents()
        {
            _handlers = new EventHandlers();
            _imageApi = new ImageAPI();
            _imageApi.Initialise();
        }

        private void UnregisterEvents()
        {
            _handlers = null;
            _imageApi.DeInitialise();
            _imageApi = null;
        }
    }
}