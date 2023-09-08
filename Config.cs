using Exiled.API.Interfaces;
using System.ComponentModel;

namespace ImageAPI
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;

        public bool Debug { get; set; } = false;

        [Description("Max width for images (Highly reccomended to stay in the double digits)")]
        public int ImageMaxWidth { get; set; } = 50;
        [Description("Max height for images (Highly reccomended to stay in the double digits)")]
        public int ImageMaxHeight { get; set; } = 40;

        [Description("Delay between each pixel spawning")]
        public float ImageSpawnPixelDelay { get; set; } = 0.01f;
    }
}