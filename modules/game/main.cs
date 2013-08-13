//-----------------------------------------------------------------------------
// Game mainfile. Defines what happens when the root main.cs has set everything
// up properly.

// Load scripts.
include(stateMachine);
include(trackingCamera);
include(bottomPrint);
exec("./verbs.cs");
exec("./character.cs");
exec("./knights.cs");
exec("./enemies.cs");
exec("./weapons.cs");
exec("./level.cs");
exec("./playGui.gui");

$forwards = "0 100000 0";

//-----------------------------------------------------------------------------
// Called when all datablocks have been transmitted.
function GameConnection::onEnterGame(%client) {
   // Give the player a controllable camera for now.
   %c = TrackingCamera.init(%client, GameGroup, Knights, y);
   %c.setTransform(Level.sectionSize*.75 SPC 0 SPC Level.sectionHeight / 2 SPC "0.345653 0.254298 -0.903248 1.36705");

   setFOV(50);
   TrackingCamera.controls(true);

   // Activate HUD which allows us to see the game. This should technically be
   // a commandToClient, but since the client and server are on the same
   // machine...
   Canvas.setContent(PlayGui);
   activateDirectInput();

   // Activate the toon-edge PostFX.
   OutlineFx.enable();
}

//-----------------------------------------------------------------------------
// Called when the engine has been initialised.
function onStart() {
   new SimGroup(GameGroup);

   // Allow us to exit the game...
   GlobalActionMap.bind(keyboard, "escape", "quit");
   GlobalActionMap.bind(keyboard, "alt f4", "quit");

   // Gameplay modules.
   Knights.onStart();
   Enemies.onStart();
   Level.onStart();

   // UI modules.
   Verbs.onStart();
   BottomPrint.onStart();
}

//-----------------------------------------------------------------------------
// Called when the engine is shutting down.
function onEnd() {
   Verbs.onEnd();
   Knights.onEnd();
   Enemies.onEnd();

   // Delete the objects we created.
   GameGroup.delete();
}
