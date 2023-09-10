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
        public float ImagepixelSize { get; set; } = 0.05f;

        [Description("Delay between each pixel spawning")]
        public float ImageSpawnPixelDelay { get; set; } = 0f;

        [Description("Max distance before images are hidden from player for performance")]
        public float ImageCullingDistance { get; set; } = 40;

        [Description("How often the culling check is run")]
        public float ImageCullingDelay { get; set; } = 3f;

        [Description("How many pixels at a time will an image despawn when too far away")]
        public int ImageCullPixelAmount { get; set; } = 7;

        [Description("How many pixels at a time will an image spawn")]
        public int ImageShowPixelAmount { get; set; } = 6;

        /*public Dictionary<string, KeyValuePair<Vector3, Vector3>> SavedImageSpawns { get; set;} = new Dictionary<string, KeyValuePair<Vector3, Transform>>()
        {
            ["neko.png"] = new KeyValuePair<Vector3, Vector3>(Vector3.back, )
        };*/

    }
}