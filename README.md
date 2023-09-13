# ImageAPI  ![Downloads](https://img.shields.io/github/downloads/morgana-x/ImageAPI/total)
A work-in-progress Exiled plugin that allows you to spawn images in SCP SL.
## This is not fully optimised (aka poorly made typically at 2am), consider if this is actually needed on your server, unless you want to muck around on a quiet one or something
# Features
+ Image downscaling
+ Downloading from URLs
+ Culling (Hide images from players when too far away)
+ Spawning and despawning images pixel by pixel (or a few pixels at a time)
+ Rotation support
+ Unoptimised code :trolling:

# Todo
+ Allow for images to be spawned on round start from config
+ Support for creating images with bullet holes (maybe)
+ Fix falling into ground glitch and serverside collider.... again (Just make the base's spawnedPrimitive public silly exiled :( )
+ linux support
+ pixel neighbour checks so less pixels are spawned 
# Commands
## image
+ arguments: (string) filename (including .png on end etc)
+ aliases: img
+ description: Spawns image from local folder on server where you are looking, needs to be inside of Exiled/Configs/Plugins/ImageAPI/
## imageurl
+ arguments: (string) url
+ aliases: imgurl
+ description: Spawns image from a url where you are looking, and caches the result in Exiled/Configs/Plugins/ImageAPI/
## imageclear
+ arguments: none
+ aliases: imgclear
+ description: Clears all images and stops any that are spawning

![Screenshot (4215)](https://github.com/morgana-x/ImageAPI/assets/89588301/9d47ca01-fac5-4bbb-b8cd-06ffb6292219)
![Screenshot (4217)](https://github.com/morgana-x/ImageAPI/assets/89588301/b5e86399-9b5a-486f-b7bb-6300b6203c58)
![Screenshot (4218)](https://github.com/morgana-x/ImageAPI/assets/89588301/94233fd0-be3b-453c-9eb7-67cc213d2ebe)
