//-----------------------------------------------------------------------------
// Copyright (c) 2012 GarageGames, LLC
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//-----------------------------------------------------------------------------

exec("./tsshim.cs");

//-----------------------------------------------------------------------------
// Load up our main GUI which lets us see the game.
exec("./playGui.gui");

//-----------------------------------------------------------------------------
// Create a datablock for the observer camera.
datablock CameraData(Observer) {};

//-----------------------------------------------------------------------------
// And a material to give the ground some colour (even if it's just white).
singleton Material(BlankWhite) {
   diffuseColor[0] = "1 1 1";
};

//-----------------------------------------------------------------------------
// Create the player material.
singleton Material(PlayerMaterial) {
   diffuseColor[0] = "1 0 0";
   mapTo = "PlayerTexture";
};

//-----------------------------------------------------------------------------
// Create the player material.
datablock PlayerData(DefaultPlayer) {
   shapeFile = "./player.dts";
};

//-----------------------------------------------------------------------------
// Called when all datablocks have been transmitted.
function GameConnection::onEnterGame(%client) {
   // Create a camera for the client.
   new Camera(TheCamera) {
      datablock = Observer;
   };
   TheCamera.setTransform("0 -7 4 1 0 0 0");

   // Cameras are not ghosted (sent across the network) by default; we need to
   // do it manually for the client that owns the camera or things will go south
   // quickly.
   TheCamera.scopeToClient(%client);

   // And let the client control the camera.
   %client.setControlObject(TheCamera);

   // Add the camera to the group of game objects so that it's cleaned up when
   // we close the game.
   GameGroup.add(TheCamera);

   // Activate HUD which allows us to see the game. This should technically be
   // a commandToClient, but since the client and server are on the same
   // machine...
   Canvas.setContent(PlayGui);
   activateDirectInput();

   // Activate the toon-edge PostFX.
   OutlineFx.enable();

   // Enable knight selection.
   KnightSelectMap.push();
}

//-----------------------------------------------------------------------------
// Function to spawn and setup controls for a knight.
function knight(%name, %pos) {
   // Create the object itself with a name, position and datablock.
   %knight = new Player(%name) {
      datablock = DefaultPlayer;
      position = %pos;
   };
   Knights.add(%knight);

   // Bind the knight's name's first letter to select it.
   %letter = getSubstr(%name, 0, 1);
   KnightSelectMap.bindCmd(keyboard, "shift" SPC %letter, "selectKnight(" @ %name @ ");", "");
}

//-----------------------------------------------------------------------------
// Select a knight.
function selectKnight(%knight) {
   SelectedKnights.add(%knight);
   endSelection();
}

function deselectKnight(%knight) {
   if(SelectedKnights.contains(%knight)) {
      SelectedKnights.remove(%knight);
   }
}

function selectAllKnights() {
   foreach(%knight in Knights) {
      selectKnight(%knight);
   }
   endSelection();
}

function deselectAllKnights() {
   foreach(%knight in Knights) {
      deselectKnight(%knight);
   }
}

function endSelection() {
   KnightSelectMap.pop();
   VerbMap.push();
}

//-----------------------------------------------------------------------------
// Define a verb.
function verb(%key, %verb) {
   VerbMap.bindCmd(keyboard, %key, "Verbs::" @ %verb @ "();", "");
}

function endVerb() {
   VerbMap.pop();
   KnightSelectMap.push();
}

//-----------------------------------------------------------------------------
// Verbs.
function Verbs::and() {
   // Give the user the chance to select another knight.
   endVerb();
}

function Verbs::test() {
   // Do something with the selected knights.
   echo(SelectedKnights.getCount());
   // Deselect all knights.
   deselectAllKnights();
   // Start selection process again.
   endVerb();
}

//-----------------------------------------------------------------------------
// Called when the engine has been initialised.
function onStart() {
   // Create objects!
   new SimGroup(GameGroup) {
      new LevelInfo(TheLevelInfo) {
         canvasClearColor = "0 0 0";
      };
      new GroundPlane(TheGround) {
         position = "0 0 0";
         material = BlankWhite;
      };
      new Sun(TheSun) {
         azimuth = 230;
         elevation = 45;
         color = "1 1 1";
         ambient = "0.1 0.1 0.1";
         castShadows = false;
      };
      new SimGroup(Knights);
      new SimSet(SelectedKnights);
   };

   // ActionMaps allows us to capture input.
   new ActionMap(KnightSelectMap);
   new ActionMap(VerbMap);

   // Create four protagonists!
   knight(Juliet, "-2 2 0");
   knight(Kilo, "2 2 0");
   knight(Hotel, "-2 -2 0");
   knight(November, "2 -2 0");

   KnightSelectMap.bind(keyboard, "a", "selectAllKnights");

   // Add some verbs that allow the knights to perform actions.
   verb(",", "And");
   verb(".", "Test");

   // Allow us to exit the game...
   GlobalActionMap.bind(keyboard, "escape", "quit");
}

//-----------------------------------------------------------------------------
// Called when the engine is shutting down.
function onEnd() {
   // Delete the objects we created.
   GameGroup.delete();
}
