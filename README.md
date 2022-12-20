# GirlForestGame

Steps to create room models:

1. Export the model titled RoomModel_1 in the Models folder of the project and import it into Blender. This is to get an idea of the size and scale of the room

2. Model a new room floor of any shape (It can also technically be of any size if you wanted to make extra small or extra large rooms and have whacky shapes).
These are the criteria that are needed for the room models:
        - Must have 4 exits, one on the end of each direction of the room (NSEW)
        - The Exits that are created must have 3 Parts (Same way that it is done in unity). The nesting of these parts should look like this
        
![alt text](https://drive.google.com/file/d/1IlQL2vX_tbdguPtlZc5CyK-jsS2qioJ4/view?usp=share_link)
             
The Door is the vertical piece, the Exit is the piece that is seperated and goes on the outside of the room, and the RoomEntrance is the horizontal piece that is connected to the Door piece. The RoomEntrance is the Parent of both the Door and the Exit.
        
This is what it looks like when an exit is connected to the room model

![alt text](https://drive.google.com/file/d/12maHqDgXGF6WTpfm6XT0zhhn_BCgwr0q/view?usp=share_link)
             
The exits can be any size and shape and can be molded to fit the model of the room if its a more organic model however keep the following in mind                       when creating their sizes and shapes:

  - The RoomEntrance piece is the place where the player spawns at when walking into a new room, so make sure the the origin of the RoomEntrance piece is actually inside of the room and not behind, inside, or almost inside the Door piece
  
  - The Exit piece is what the player must collide with in order to exit the room and go into the next room over. This means make sure the player is able to walk up to it and touch it. Dont put it too far away from the actual Door itself.
  
  - The Door piece is just essentially a wall that gets disabled when the room is completed so the doorway can open up to let the player walk over the Exit piece. The door can also still be any shape or size, just make sure that it perfectly blends with the walls/boundaries of the room so they player cannot glitch through the edge or something similar. Also make sure the entirety of the area is covered (if one of the faces on the outside is culled for some reason the player will go right through it).

When finished creating the room model the final layout should look something like this
![alt text](https://drive.google.com/file/d/1TEizx3sPgJMIO_HRL_e6JXtCns2pnJ5K/view?usp=share_link)
 
Each of the four entrances are perfectly parallel in their respective directions, all of them having proper object nesting and being children of the main room model as a whole.

3. The next step is to import the new model into Unity and set it up properly in the inspector.
