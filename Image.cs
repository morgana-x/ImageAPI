using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.API;
using Exiled.API.Features.Toys;
using System.Drawing;
using System.IO;
using UnityEngine;
using MEC;
using System.Drawing.Imaging;
using System.Net;
using System.Security.Policy;

namespace ImageAPI
{
    public class Image
    {
        public List<Primitive> spawnedPrimitives = new List<Primitive>();
        public List<CoroutineHandle> imageSpawningCoroutines = new List<CoroutineHandle>();
        public string getImageFolder()
        {
            return Plugin.Instance.ConfigPath.Replace("7777.yml", string.Empty) + @"\image";
        }
        public string getImagePath(string file)
        {
            return getImageFolder() + @"\" + file;
        }

        public static UnityEngine.Color GetColorFromString(string colorText)
        {
            UnityEngine.Color color = new UnityEngine.Color(-1f, -1f, -1f);
            string[] charTab = colorText.Split(':');

            if (charTab.Length >= 4)
            {
                if (float.TryParse(charTab[0], out float red))
                    color.r = red / 255f;

                if (float.TryParse(charTab[1], out float green))
                    color.g = green / 255f;

                if (float.TryParse(charTab[2], out float blue))
                    color.b = blue / 255f;

                if (float.TryParse(charTab[3], out float alpha))
                    color.a = alpha;

                return color != new UnityEngine.Color(-1f, -1f, -1f) ? color : UnityEngine.Color.magenta * 3f;
            }

            if (colorText[0] != '#' && colorText.Length == 8)
                colorText = '#' + colorText;

            return ColorUtility.TryParseHtmlString(colorText, out color) ? color : UnityEngine.Color.magenta * 3f;
        }

        public void spawnImage(string file, Vector3 Location, Transform rotationTransform = null, float pixelSize = 0.1f, bool collide = false, int maxSize = 15)
        {
            Log.Debug( "Spawning " + file);
            string imageFolder = getImageFolder();
            string imagePath = getImagePath(file);
            Log.Debug(imageFolder);
            Log.Debug(imagePath);
            if (!Directory.Exists(imageFolder)) 
            {
                Directory.CreateDirectory(imageFolder);
            }
            if (!File.Exists(imagePath))
            {
                Log.Debug(imagePath + " does not exist!");
                return;
            }
            Dictionary<Vector3, UnityEngine.Color> pixels= new Dictionary<Vector3, UnityEngine.Color>();
            Bitmap img = new Bitmap(imagePath);
            if (img == null)
            {
                Log.Warn("Null image! Aborting spawning.");
                return;
            }
            int imgSize = img.Width * img.Height;
            if ((imgSize) > maxSize) // Todo: fix rescaling system
            {
                Log.Debug("Image too large (" + imgSize.ToString() + "/" + maxSize.ToString() +  "), compressing...");
                double scaleDown =  imgSize / maxSize;
                Log.Debug("Scaledown: " + scaleDown.ToString());
                Log.Debug("Image width:" + img.Width);
                int newWidth = (int)(scaleDown / img.Width); //( scaleDown / img.Width); // REMEMBER TO REVERT THIS LATER
                Log.Debug("New Image width: " + newWidth.ToString());
                
                int newHeight = (int)(scaleDown / img.Height);
                Log.Debug("New Image height: " + newWidth.ToString());
                Log.Debug("New image size: " +  (newWidth * newHeight).ToString());
                //Log.Warn(newHeight);
                //Log.Warn(newWidth.ToString() + " / " + newHeight.ToString());
                img = new Bitmap(img, newHeight, newWidth);
            }
            //img.RotateFlip(RotateFlipType.Rotate180FlipXY);
            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    System.Drawing.Color pixel = img.GetPixel(x, y);
                    UnityEngine.Color unityColor = new UnityEngine.Color( pixel.R, pixel.G, pixel.B);
                    int yoffset = img.Height - y;
                    Vector3 Offset = new Vector3(x*pixelSize, yoffset*pixelSize, 0);
                    if (rotationTransform != null)
                    {
                        Offset = (rotationTransform.right * x * pixelSize) + (rotationTransform.up * yoffset * pixelSize);
                    }
                    pixels.Add(Offset, unityColor);
                }
            }
            Log.Debug("Completed preparing pixels");
            Vector3 Rotation = Vector3.one;
            if (rotationTransform != null)
            {
                Rotation = rotationTransform.rotation.eulerAngles;
            }
            CoroutineHandle handle = Timing.RunCoroutine(spawnImagePrimitives(pixels, Location, Rotation, pixelSize, collide));
            imageSpawningCoroutines.Add(handle);
            Log.Debug("Started coroutine");
            
        }
        private IEnumerator<float> spawnImagePrimitives(Dictionary<Vector3, UnityEngine.Color> pixels, Vector3 Location, Vector3 Rotation, float pixelSize = 0.1f, bool collide = false)
        {
            //List<Primitive> primitives = new List<Primitive>();
            foreach (var p in pixels)
            {
                //Log.Warn(col.ToString());
                Vector3 pos = Location + p.Key;//new Vector3(p.Key.x * pixelSize, p.Key.y * pixelSize, 0);
                Vector3 newScale = (Vector3.one * pixelSize);
                Primitive pixelPrimitive = Primitive.Create(PrimitiveType.Cube, position: pos, rotation: Rotation, scale: newScale); // todo add rotation!
                UnityEngine.Color col = p.Value;
                pixelPrimitive.Color = GetColorFromString(col.r + ":" + col.g + ":" + col.b + ":" + 255);  //ColorUtility.ToHtmlStringRGBA(col));
                pixelPrimitive.Collidable = collide;
                //primitives.Add(pixelPrimitive);
                spawnedPrimitives.Add(pixelPrimitive);
                yield return Timing.WaitForSeconds(0.01f);
            }

        }

        public void deleteAllImages()
        {
            foreach (CoroutineHandle handle in imageSpawningCoroutines)
            {
                Timing.KillCoroutines(handle);
            }
            imageSpawningCoroutines.Clear();
            foreach (Primitive p in spawnedPrimitives)
            {
                p.Destroy();
            }
            spawnedPrimitives.Clear();
        }

        public void downloadImage(string url, Action<string> callBack = null) // https://stackoverflow.com/questions/24797485/how-to-download-image-from-url :(
        {
            string fileName = url.Replace(".", string.Empty);
            string imgPath = getImagePath(fileName) + ".png";
            if (File.Exists(imgPath))
            {
                if (callBack != null)
                    callBack(fileName + ".png");
                Log.Debug("file already exists!");
                return;
            }
            using (WebClient webClient = new WebClient())
            {
                byte[] data = webClient.DownloadData(url);

                using (MemoryStream mem = new MemoryStream(data))
                {
                    using (var downloadedImage = System.Drawing.Image.FromStream(mem))
                    {
                        // If you want it as Png
                        downloadedImage.Save(imgPath, ImageFormat.Png);
                        if (callBack != null)
                            callBack(fileName + ".png");
                    }
                }

            }
        }
        public void downloadImagePosition(string url, Vector3 pos, Transform rotationTransform = null, Action<string, Vector3, Transform> callBack = null) // https://stackoverflow.com/questions/24797485/how-to-download-image-from-url :(
        {
            string fileName = url.Replace(".", string.Empty);
            string imgPath = getImagePath(fileName) + ".png";
            if (File.Exists(imgPath))
            {
                if (callBack != null)
                    callBack(fileName + ".png", pos, rotationTransform);
                Log.Debug("file already exists!");
                return;
            }
            using (WebClient webClient = new WebClient())
            {
                byte[] data = webClient.DownloadData(url);

                using (MemoryStream mem = new MemoryStream(data))
                {
                    using (var downloadedImage = System.Drawing.Image.FromStream(mem))
                    {
                        // If you want it as Png
                        downloadedImage.Save(imgPath, ImageFormat.Png);
                        if (callBack != null)
                            callBack(fileName + ".png", pos, rotationTransform);
                    }
                }

            }
        }
    }
}
