using Exiled.API.Interfaces;
using System.ComponentModel;

namespace ImageAPI
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;

        public bool Debug { get; set; } = false;

        [Description("Higher number = lower resolution images")]
        public int ImageCompressionLevel = 15;
    }
}