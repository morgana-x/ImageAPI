
using Exiled.Events.EventArgs.Server;

namespace ImageAPI
{
    public class EventHandlers
    {
        public void RoundEnded(RoundEndedEventArgs ev)
        {
            Plugin.Instance._imageApi.deleteAllImages();
        }
        public void RoundRestarting()
        {
            Plugin.Instance._imageApi.deleteAllImages();
        }
        
    }
}