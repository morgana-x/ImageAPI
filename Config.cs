using Exiled.API.Interfaces;
using System.ComponentModel;

namespace ImageAPI
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;

        public bool Debug { get; set; } = false;

        [Description("Higher number = lower resolution images")]
        public int ImageCompressionLevel { get; set; } = 15;

        [Description("Delay between each pixel spawning")]
        public float ImageSpawnPixelDelay { get; set; } = 0.01f;
    }
}