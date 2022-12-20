# GirlForestGame

Steps to create room models:

1. Export the model titled RoomModel_1 in the Models folder of the project and import it into Blender. This is to get an idea of the size and scale of the room

2. Model a new room floor of any shape (It can also technically be of any size if you wanted to make extra small or extra large rooms and have whacky shapes).
These are the criteria that are needed for the room models:

        - The room must have walls surrounding the outside edge of the floor. The walls should be a seperate object as a child of the main model object. They can be any height, i just extruded mine up by 2 units in Blender and that seemed to be good. The reason we need the walls is strictly for colliders, when they are put into Unity, they will be invisible and only have colliders.
        
        - Must have 4 exits, one on the end of each direction of the room (NSEW).
        
        - The Exits that are created must have 3 Parts (Same way that it is done in unity).
        
![The nesting of these parts should look like this](https://drive.google.com/file/d/1IlQL2vX_tbdguPtlZc5CyK-jsS2qioJ4/view?usp=share_link)
             
The Door is the vertical piece, the Exit is the piece that is seperated and goes on the outside of the room, and the RoomEntrance is the horizontal piece that is connected to the Door piece. The RoomEntrance is the Parent of both the Door and the Exit.

![This is what it looks like when an exit is connected to the room model](https://drive.google.com/file/d/12maHqDgXGF6WTpfm6XT0zhhn_BCgwr0q/view?usp=share_link)
             
The exits can be any size and shape and can be molded to fit the model of the room if its a more organic model however keep the following in mind                       when creating their sizes and shapes:

  - The RoomEntrance piece is the place where the player spawns at when walking into a new room, so make sure the the origin of the RoomEntrance piece is actually inside of the room and not behind, inside, or almost inside the Door piece
  
  - The Exit piece is what the player must collide with in order to exit the room and go into the next room over. This means make sure the player is able to walk up to it and touch it. Dont put it too far away from the actual Door itself.
  
  - The Door piece is just essentially a wall that gets disabled when the room is completed so the doorway can open up to let the player walk over the Exit piece. The door can also still be any shape or size, just make sure that it perfectly blends with the walls/boundaries of the room so they player cannot glitch through the edge or something similar. Also make sure the entirety of the area is covered (if one of the faces on the outside is culled for some reason the player will go right through it).

When finished creating the room model the final layout should look something like this
![alt text](https://drive.google.com/file/d/1HaNQrE9h5T2zai3b1SWluK_RtrqExnAh/view?usp=share_link)
 
Each of the four entrances are perfectly parallel in their respective directions, all of them having proper object nesting and being children of the main room model as a whole.

3. After completing the model, you must export the finished model as an FBX. Here is what you need to export correctly:

 - Do not export the Camera or Light with the object
 - Make sure to Triangulate Faces
 - Set the forward to Y Forward and the up to Z up
 - Make sure it's being exported as FBX

You want to save the file in the Unity project in side the Assets/FBXs/Room Models folder and name the file accordingly with the naming convention RoomModel_n where n is the model number. 


4. After you have the FBX in unity in the correct folder, Simply drag the FBX into the scene to be able to view it. It will pop up as an uneditable prefab. Once it is in the scene, Drag the object from the scene into the project window inside the Assets/Models folder. A window will pop up, make sure to press Original Prefab and it will create a new model in the folder that you can edit as a new prefab.


5. The next step is to properly set up the model in the Unity inspector. First double click on the newly created prefab for the room model and it will open it up in prefab view. After it is open, here is everything you need to do in this section:

 - First, select EVERY object together (including all children, it wont select unless the drop downs in the hierarchy are completely open), at the bottom of the inspector window while everything is selected press Add Component and add a MeshCollider component. This will add the proper colliders for every object in the room model

 - Next, Select only the main model (the parent of the prefab) and add the RoomModel script onto it either through the inspector like above or dragging the script onto the object in the hierarchy. This script has a list for Doors and Exits. You must add each door and exit that exits under the model into those lists and they must be in this exact order. North, South, East, West. If they are in the list in any other order it will not work properly.

 - 


