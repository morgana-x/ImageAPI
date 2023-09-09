using Exiled.API.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Policy;
using UnityEngine;

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

        [Description("Size of the pixels for images")]
        public float ImagepixelSize { get; set; } = 0.1f;

        [Description("Delay between each pixel spawning")]
        public float ImageSpawnPixelDelay { get; set; } = 0.01f;

        /*public Dictionary<string, KeyValuePair<Vector3, Vector3>> SavedImageSpawns { get; set;} = new Dictionary<string, KeyValuePair<Vector3, Transform>>()
        {
            ["neko.png"] = new KeyValuePair<Vector3, Vector3>(Vector3.back, )
        };*/

    }
}