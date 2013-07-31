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
exec("./verbs.cs");
exec("./knights.cs");
exec("./level1.cs");

//-----------------------------------------------------------------------------
// Load up our main GUI which lets us see the game.
exec("./playGui.gui");

//-----------------------------------------------------------------------------
// Create a datablock for the observer camera.
datablock CameraData(Observer) {};

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

   Knights.onEnterGame(%client);
}

//-----------------------------------------------------------------------------
// Called when the engine has been initialised.
function onStart() {
   new SimGroup(GameGroup);

   // Allow us to exit the game...
   GlobalActionMap.bind(keyboard, "escape", "quit");

   Verbs.onStart();
   Knights.onStart();
   Level1.onStart();
}

//-----------------------------------------------------------------------------
// Called when the engine is shutting down.
function onEnd() {
   Verbs.onEnd();
   Knights.onEnd();

   // Delete the objects we created.
   GameGroup.delete();
}
