
using Exiled.Events.EventArgs.Server;

namespace ImageAPI
{
    public class EventHandlers
    {
        public void RoundEnded(RoundEndedEventArgs ev)
        {
            Plugin.Instance._imageApi.DeInitialise();
        }
        public void RoundRestarting()
        {
            Plugin.Instance._imageApi.DeInitialise();
        }
        public void WaitingForPlayers()
        {
            Plugin.Instance._imageApi.DeInitialise();
            Plugin.Instance._imageApi.Initialise();
        }
        
    }
}