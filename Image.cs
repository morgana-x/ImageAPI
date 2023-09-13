using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.API;
using Exiled.API.Features.Toys;
using System.IO;
using UnityEngine;
using MEC;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Drawing;
using Mirror;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace ImageAPI
{
    public class SpawnedImage
    {
        public Vector3 Position { get; set; } = Vector3.zero;
        public List<Primitive> Primitives = new List<Primitive>();
        public Room room { get; set; } = null;

        public string Image { get; set; } = string.Empty;

        public string Id { get; set; } = string.Empty;

        public bool shouldCull(Player pl)
        {
            if ( (room != null)  && (pl.Zone != room.Zone) )
            {
                return true;
            }
            if (Vector3.Distance(pl.Position, Position) > Plugin.Instance.Config.ImageCullingDistance)
            {
                return true;
            }
            return false;
        }
        public SpawnedImage(Vector3 position, string image, List<Primitive> primitives)
        {
            Position = position;
            Primitives = primitives;
            Image = image;
            Id = image + Position.ToString();
        }
    }
    public class ImageAPI // You ready to see the worst code of your life!!!???
    {
        public List<Primitive> spawnedPrimitives = new List<Primitive>();
        public List<SpawnedImage> spawnedImages = new List<SpawnedImage>();
        public List<CoroutineHandle> imageSpawningCoroutines = new List<CoroutineHandle>();
        public CoroutineHandle cullingCoroutineHandle;
        public CoroutineHandle cullingDoCoroutineHandle;
        public Dictionary<Player, List<SpawnedImage>> playerVisibleImages = new Dictionary<Player, List<SpawnedImage>>();
        public Dictionary<Player, CoroutineHandle> playerCullingHandle = new Dictionary<Player, CoroutineHandle>();
        public Dictionary<Player, List<Primitive>> playerShowPrimitiveQueue = new Dictionary<Player, List<Primitive>>();
        public Dictionary<Player, List<Primitive>> playerHidePrimitiveQueue = new Dictionary<Player, List<Primitive>>();
        public string getImageFolder()
        {
            return Plugin.Instance.ConfigPath.Replace(Server.Port + ".yml", string.Empty);// + @"\image";
        }
        public string getImagePath(string file)
        {
            return getImageFolder() + file;
        }
        public void hidePrimitive(Player pl, Primitive primitive)
        {
            pl.Connection.Send(new ObjectDestroyMessage
            { netId = primitive.Base.netIdentity.netId });
        }
        public void showPrimitive(Player player, Primitive primitive)
        {
            Server.SendSpawnMessage.Invoke(null, new object[] { primitive.Base.netIdentity, player.Connection });
        }
        private IEnumerator<float> playerCullPrimitives() 
        {
            while (true)
            {
                foreach (Player pl in Player.List)
                {
                    if (!playerVisibleImages.ContainsKey(pl))
                    {
                        playerVisibleImages.Add(pl, new List<SpawnedImage>());
                    }
                    if (!playerShowPrimitiveQueue.ContainsKey(pl))
                    {
                        playerShowPrimitiveQueue.Add(pl, new List<Primitive>());
                    }
                    if (!playerHidePrimitiveQueue.ContainsKey(pl))
                    {
                        playerHidePrimitiveQueue.Add(pl, new List<Primitive>());
                    }
                    foreach (SpawnedImage b in spawnedImages)
                    {
                        if ( (( playerVisibleImages[pl].Contains(b))) && b.shouldCull(pl))
                        {
                            if ( playerVisibleImages[pl].Contains(b))
                            {
                                playerVisibleImages[pl].Remove(b);
                            }
                            playerHidePrimitiveQueue[pl].AddRange(b.Primitives);
                            playerShowPrimitiveQueue[pl] = playerShowPrimitiveQueue[pl].Except(b.Primitives).ToList();

                            Log.Debug("Culling " + b.Id + " for " + pl.DisplayNickname);
                            continue;
                        }

                        if (playerVisibleImages[pl].Contains(b))
                        {
                            continue;
                        }
                        if (b.shouldCull(pl))
                        {
                            continue;
                        }
                        //showPrimitives(pl, b.Primitives);
                  
                        playerShowPrimitiveQueue[pl].AddRange(b.Primitives);
                        playerHidePrimitiveQueue[pl] = playerHidePrimitiveQueue[pl].Except(b.Primitives).ToList();
                        Log.Debug("Showing " + b.Id + " for " + pl.DisplayNickname);
                        playerVisibleImages[pl].Add(b);
                        if (Plugin.Instance.Config.ExtremeOptimisation)
                        {
                            yield return Timing.WaitForOneFrame;
                        }
                    }
                }
                yield return Timing.WaitForSeconds(Plugin.Instance.Config.ImageCullingDelay);
            }
        }
        private IEnumerator<float> playerPrimitiveVisiblity()
        {
            while (true)
            {
                bool nothingDone = true;
                foreach (Player pl in Player.List)
                {
                    if (!playerHidePrimitiveQueue.ContainsKey(pl))
                    {
                        playerHidePrimitiveQueue.Add(pl, new List<Primitive>());
                    }
                    if (!playerShowPrimitiveQueue.ContainsKey(pl))
                    {
                        playerShowPrimitiveQueue.Add(pl, new List<Primitive>());
                    }

                    if (playerShowPrimitiveQueue[pl].Count > 0)
                    {
                        for (int i = 1; i <= Plugin.Instance.Config.ImageShowPixelAmount; i++)
                        {
                            if (i > playerShowPrimitiveQueue[pl].Count)
                                break;
                            Primitive showPrimitiveThing = playerShowPrimitiveQueue[pl].FirstOrDefault();
                            showPrimitive(pl, showPrimitiveThing);
                            playerShowPrimitiveQueue[pl].Remove(showPrimitiveThing);
                        }
                        nothingDone = false;
                        if (Plugin.Instance.Config.ExtremeOptimisation)
                        {
                            yield return Timing.WaitForOneFrame;
                        }
                    }
                    if (playerHidePrimitiveQueue[pl].Count >0)
                    {
                        for (int i = 1; i <= Plugin.Instance.Config.ImageCullPixelAmount; i++)
                        {
                            if (i > playerHidePrimitiveQueue[pl].Count)
                                break;
                            Primitive hidePrimitiveThing = playerHidePrimitiveQueue[pl].FirstOrDefault();
                            hidePrimitive(pl, hidePrimitiveThing);
                            playerHidePrimitiveQueue[pl].Remove(hidePrimitiveThing);
                        }
                        nothingDone = false;
                    }
                
                }
                if (!nothingDone)
                    yield return Timing.WaitForSeconds(Plugin.Instance.Config.ImageSpawnPixelDelay);
                else
                    yield return Timing.WaitForSeconds(Plugin.Instance.Config.ImageCullingDelay);
            }
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
        Vector2 ConstrainVerbose(int imageWidth, int imageHeight, int maxWidth, int maxHeight) //https://stackoverflow.com/questions/5222711/image-resize-in-c-sharp-algorith-to-determine-resize-dimensions-height-and-wi
        {
            // Coalculate the aspect ratios of the image and bounding box
            var maxAspect = (float)maxWidth / (float)maxHeight;
            var aspect = (float)imageWidth / (float)imageHeight;
            // Bounding box aspect is narrower
            if (maxAspect <= aspect && imageWidth > maxWidth)
            {
                // Use the width bound and calculate the height
                return new Vector2(maxWidth, Math.Min(maxHeight, maxWidth / aspect));
            }
            else if (maxAspect > aspect && imageHeight > maxHeight)
            {
                // Use the height bound and calculate the width
                return new Vector2(Math.Min(maxWidth, maxHeight * aspect), maxHeight);
            }
            else
            {
                return new Vector2(imageWidth, imageHeight);
            }
        }
    
        public void spawnImage(string file, Vector3 Location, Transform rotationTransform = null, float pixelSize = 0.1f, bool collide = false, int maxSize = 255)
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
            //var rawData = System.IO.File.ReadAllBytes(imagePath);
            //Texture2D img = new Texture2D(2, 2); // Create an empty Texture; size doesn't matter
            //img.LoadImage(rawData, false);
            
            if (img == null)
            {
                Log.Warn("Null image! Aborting spawning.");
                return;
            }


            Vector2 newSize = ConstrainVerbose(img.Width, img.Height, Plugin.Instance.Config.ImageMaxWidth, Plugin.Instance.Config.ImageMaxHeight);//ConstrainVerbose(img.Width, img.Height, Plugin.Instance.Config.ImageMaxWidth, Plugin.Instance.Config.ImageMaxHeight).ToSize();


            Log.Debug("New size: " + newSize.ToString());

            img = new Bitmap(img, (int)newSize.x, (int)newSize.y);

            for (int x = 0; x < img.Width; x++)
            //for (int x = 0; x < img.width; x++)
            {
                 for (int y = 0; y < img.Height; y++)
                //for (int y = 0; y < img.height; y++)
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

            Vector3 finishPosition = (rotationTransform.right * img.Width * pixelSize) + (rotationTransform.up * -img.Height * 0.5f * pixelSize);
            CoroutineHandle handle = Timing.RunCoroutine(spawnImagePrimitives(pixels, Location - (finishPosition/2), Rotation, pixelSize, collide));
            imageSpawningCoroutines.Add(handle);
            Log.Debug("Started coroutine");
            
        }
        private IEnumerator<float> spawnImagePrimitives(Dictionary<Vector3, UnityEngine.Color> pixels, Vector3 Location, Vector3 Rotation, float pixelSize = 0.1f, bool collide = false)
        {

            List<Primitive> primitives = new List<Primitive>();
            int skipTime = 0;
            foreach (var p in pixels)
            {
                Vector3 pos = Location + p.Key;
                Vector3 newScale = (Vector3.one * pixelSize);
                Primitive pixelPrimitive = Primitive.Create(PrimitiveType.Cube, position: pos, rotation: Rotation, scale: newScale, spawn:false);
                pixelPrimitive.Color = GetColorFromString(p.Value.r + ":" + p.Value.g + ":" + p.Value.b + ":" + 255);
                pixelPrimitive.Scale = Vector3.zero;
                pixelPrimitive.Spawn();
                pixelPrimitive.Scale = newScale;
                primitives.Add(pixelPrimitive);
                spawnedPrimitives.Add(pixelPrimitive);
                skipTime++;
                if (skipTime > Plugin.Instance.Config.ImageShowPixelAmount)
                {
                    skipTime =  0;
                    yield return Timing.WaitForSeconds(Plugin.Instance.Config.ImageSpawnPixelDelay);
                }
            }

            SpawnedImage newImage = new SpawnedImage(Location, "test", primitives);
            Room imageRoom = Room.Get(Location);
            if (imageRoom != null && imageRoom.Type != Exiled.API.Enums.RoomType.Unknown)
            {
                newImage.room = Room.Get(Location);
            }
            spawnedImages.Add(newImage);
            foreach (Player pl in Player.List)
            {
                if (!playerVisibleImages.ContainsKey(pl))
                {
                    playerVisibleImages[pl] = new List<SpawnedImage>();
                }
                if (!newImage.shouldCull(pl))
                {
                    playerVisibleImages[pl].Add(newImage);
                }
            }
            
        }

        public void deleteAllImages()
        {
            foreach (CoroutineHandle handle in imageSpawningCoroutines)
            {
                Timing.KillCoroutines(handle);
            }
            imageSpawningCoroutines.Clear();
            playerHidePrimitiveQueue.Clear();
            playerShowPrimitiveQueue.Clear();
            playerVisibleImages.Clear();
            foreach (Primitive p in spawnedPrimitives)
            {
                p.Destroy();
            }
            spawnedPrimitives.Clear();
            spawnedImages.Clear();
        }

        public void Initialise()
        {
            cullingCoroutineHandle = Timing.RunCoroutine(playerCullPrimitives());
            cullingDoCoroutineHandle = Timing.RunCoroutine(playerPrimitiveVisiblity());
            Log.Info("Initialised");
        }
        public void DeInitialise()
        {
            Timing.KillCoroutines(cullingCoroutineHandle);
            Timing.KillCoroutines(cullingDoCoroutineHandle);
            deleteAllImages();
            Log.Info("Deinitalised");
        }
        private string UrlToFileName(string dangerousURL)
        {
            return dangerousURL.Replace("http", string.Empty).Replace("www.", string.Empty).Replace(".", string.Empty).Replace("/", string.Empty).Replace(":", string.Empty).Replace(@"\", string.Empty);
        }
        public static bool IsImage(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);

            List<string> jpg = new List<string> { "FF", "D8" };
            List<string> bmp = new List<string> { "42", "4D" };
            List<string> gif = new List<string> { "47", "49", "46" };
            List<string> png = new List<string> { "89", "50", "4E", "47", "0D", "0A", "1A", "0A" };
            List<List<string>> imgTypes = new List<List<string>> { jpg, bmp, gif, png };

            List<string> bytesIterated = new List<string>();

            for (int i = 0; i < 8; i++)
            {
                string bit = stream.ReadByte().ToString("X2");
                bytesIterated.Add(bit);

                bool isImage = imgTypes.Any(img => !img.Except(bytesIterated).Any());
                if (isImage)
                {
                    return true;
                }
            }

            return false;
        }
        public async void downloadImagePosition(string url, Vector3 pos, Transform rotationTransform = null, Action<string, Vector3, Transform> callBack = null) // https://stackoverflow.com/questions/24797485/how-to-download-image-from-url :(
        {
            string fileName = UrlToFileName(url);
            string imgPath = getImagePath(fileName) + ".png";
            if (File.Exists(imgPath))
            {
                FileStream checkFile = File.OpenRead(imgPath);
                if (checkFile != null && !IsImage(checkFile))
                {
                    Log.Debug("Invalid image!");
                    return;
                }
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
                    File.WriteAllBytes(imgPath, mem.ToArray());
                    while (!File.Exists(imgPath))
                    { }
                    FileStream checkFile = File.OpenRead(imgPath);
                    if (checkFile != null && IsImage(checkFile))
                    {
                        if (callBack != null)
                            callBack(fileName + ".png", pos, rotationTransform);
                    }
                    else
                    {
                        try
                        {
                            File.Delete(imgPath);
                        }
                        catch(Exception e) 
                        {
                            Log.Debug(e.ToString());
                        }
                        Log.Debug("Invalid Image!");
                    }
                    checkFile.Dispose();
                    checkFile.Close();
                    // cant use System.Drawing.Image because SCP SL is missing the GDI something dll library that it relies on when on linux
                    //using (var downloadedImage = System.Drawing.Image.FromStream(mem))
                    //{
                    // If you want it as Png
                     //downloadedImage.Save(imgPath, ImageFormat.Png);
                   // }
                }

            }
        }
    }
}
