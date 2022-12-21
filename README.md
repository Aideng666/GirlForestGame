# GirlForestGame

Steps to create room models:

1. Export the model titled RoomModel_1 in the Models folder of the project and import it into Blender. This is to get an idea of the size and scale of the room


2. Model a new room floor of any shape (It can also technically be of any size if you wanted to make extra small or extra large rooms and have whacky shapes). You can also put like holes in the middle for places in the room that the player cant walk. This could also work with obstacles as well.
These are the criteria that are needed for the room models:

 - The room must have walls surrounding the outside edge of the floor as well as surrounding any holes in the middle of the floor with outward facing walls. The reason they need to be outward facing is because if the walls are culled, the player will just walk right through them in the game. The walls should be a seperate object as a child of the main model object. They can be any height, I just extruded mine up by 2 units in Blender and that seemed to be good. The reason we need the walls is strictly for colliders, when they are put into Unity, they will be invisible and only have colliders.
        
 - Must have 4 exits, one on the end of each direction of the room (NSEW).
        
 - The Exits that are created must have 3 Parts (Same way that it is done in unity).
        
![The nesting of these parts should look like this](https://github.com/Aideng666/GirlForestGame/blob/main/RoomModelStepsScreenshots/Nesting.png)
             
The Door is the vertical piece, the Exit is the piece that is seperated and goes on the outside of the room, and the RoomEntrance is the horizontal piece that is connected to the Door piece. The RoomEntrance is the Parent of both the Door and the Exit.

![This is what it looks like when an exit is connected to the room model](https://github.com/Aideng666/GirlForestGame/blob/main/RoomModelStepsScreenshots/Combined.png)
             
The exits can be any size and shape and can be molded to fit the model of the room if its a more organic model however keep the following in mind                       when creating their sizes and shapes:

  - The RoomEntrance piece is the place where the player spawns at when walking into a new room, so make sure the the origin of the RoomEntrance piece is actually inside of the room and not behind, inside, or almost inside the Door piece
  
  - The Exit piece is what the player must collide with in order to exit the room and go into the next room over. This means make sure the player is able to walk up to it and touch it. Dont put it too far away from the actual Door itself.
  
  - The Door piece is just essentially a wall that gets disabled when the room is completed so the doorway can open up to let the player walk over the Exit piece. The door can also still be any shape or size, just make sure that it perfectly blends with the walls/boundaries of the room so they player cannot glitch through the edge or something similar. Also make sure the entirety of the area is covered (if one of the faces on the outside is culled for some reason the player will go right through it).

![When finished creating the room model the final layout should look something like this](https://github.com/Aideng666/GirlForestGame/blob/main/RoomModelStepsScreenshots/Finished.png)
 
Each of the four entrances are perfectly parallel in their respective directions, all of them having proper object nesting and being children of the main room model as a whole.


3. After completing the model, you must export the finished model as an FBX. Here is what you need to export correctly:

 - Before exporting, make sure to apply transforms on all objects, and make sure to reset all of the Origin points to geometery for the Entrances, Doors, and Exits. Do not use the Apply Transform option in the export menu as it does not work I tried it.
 - Do not export the Camera or Light with the object
 - Make sure to Triangulate Faces
 - Set the forward to Y Forward and the up to Z up

You want to save the file in the Unity project in side the Assets/FBXs/Room Models folder and name the file accordingly with the naming convention RoomModel_n where n is the model number. 


4. After you have the FBX in unity in the correct folder, Simply drag the FBX into the scene to be able to view it. It will pop up as an uneditable prefab. Once it is in the scene, Drag the object from the scene into the project window inside the Assets/Models folder. A window will pop up, make sure to press Original Prefab and it will create a new model in the folder that you can edit as a new prefab. If the prefab is scaled by 100, 100, 100 and rotated -90 on the X, don't worry about it and leave it that's how it should be. I know you applied transforms before but Unity just likes to mess with you but I have already made it so it still works either way. The only thing you need to change about the transform is rotate it on the Y by 180 degrees because for some reason it rotates the room when going from blender to Unity.


5. The next step is to properly set up the model in the Unity inspector. First double click on the newly created prefab for the room model and it will open it up in prefab view. After it is open, here is everything you need to do in this section:

 - First, select EVERY object together (including all children, it wont select unless the drop downs in the hierarchy are completely open), at the bottom of the inspector window while everything is selected press Add Component and add a MeshCollider component. This will add the proper colliders for every object in the room model

 - Next, Select only the main model (the parent of the prefab) and add the RoomModel script onto it either through the inspector like above or dragging the script onto the object in the hierarchy. This script has a list for Doors and Exits. You must add each door and exit that exists in the model into those lists and they must be in this exact order. North, South, East, West. If they are in the list in any other order it will not work properly. A nice and easy way to do this part is, when you have the Room Model component open in the inspector, lock the inspector window using the little lock looking button all the way at the top right of the inspector window. When it is locked you can select all of the doors in order at the same time and then drag them all into the Doors section in the inspector and it should add all 4 of them at the same time, then all u need to do is put them in the correct order. Also do the same with the exits. Make sure to unlock the inspector after as well as it could cause confusion if you forget to.

 - Then, select all of the Exits. Once they are all selected, add the RoomExit script onto them through the inspector. You will see a variable with a drop down menu under the RoomExit script in the Exits, the variable is called Exit Dir. Change each of the exits to their respective direction(East Exit has East Dir, North has North, etc). Finally set all of the tags of each Exit to be the Exit tag.
 
 -  Finally, select every other object that is not an exit(roomModel, Walls, Entrances, Doors) and change all of their tags to the Room tag.


6. Finally, Go into Assets/Resources and choose the corresponding folder to the type of room model you are creating(If you made a Totem room model go into the TotemRooms folder, for regular fighting rooms it is the Rooms folder, etc). This is so just incase we wanna make extra models for shops and Marking rooms and stuff later for more variety. Once you are in the correct folder, right click inside the project window, go to Create, then at the top of the list click on the option for Room Object and name it by writing the type of room first then an underscore, and then the number that you used for the model in blender. For example my room above would be Room_4 because it was a regular fighting room and was the 4th fighting room model. Then select the new RoomObject you created, in the inspector you will see an option for Model. Select the model prefab you created above, to find it easily, just press the little circle with a dot in it beside the Model selector in the inspector, type in the number of the room that it was and it should pop up for you to select.


