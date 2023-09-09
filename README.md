# ImageAPI
A work-in-progress Exiled plugin that allows you to spawn images in SCP SL.
+ This is not fully optimised and may lag players' games (on connection when the object is already spawned or if you spawned too many images)!!!!!!!!!!!
+ Images are spawned pixel by pixel with a certain delay to avoid freezing and crashing players' games

# Todo
+ Add culling system for each player
+ Add pixel size to config
+ Allow for images to be spawned on round start from config
+ Support for creating images with bullet holes (maybe)
# Commands
## image
+ arguments: (string) filename (including .png on end etc)
+ aliases: img
+ description: Spawns image from local folder on server where you are looking, needs to be inside of Exiled/Configs/Plugins/ImageAPI/image/
## imageurl
+ arguments: (string) url
+ aliases: imgurl
+ description: Spawns image from a url where you are looking, and caches the result in Exiled/Configs/Plugins/ImageAPI/image/
## imageclear
+ arguments: none
+ aliases: imgclear
+ description: Clears all images and stops any that are spawning

![Screenshot (4215)](https://github.com/morgana-x/ImageAPI/assets/89588301/9d47ca01-fac5-4bbb-b8cd-06ffb6292219)
