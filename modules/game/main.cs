//-----------------------------------------------------------------------------
// Game mainfile. Defines what happens when the root main.cs has set everything
// up properly.

// Module dependencies.
include(stateMachine);
include(trackingCamera);
include(flyCamera);
include(bottomPrint);
include(navigation);
include(level);

// Scripts that make up this module.
exec("./events.cs");
exec("./verbs.cs");
exec("./character.cs");
exec("./knights.cs");
exec("./enemies.cs");
exec("./weapons.cs");
exec("./cover.cs");
exec("./playGui.gui");
exec("./combat.cs");

//-----------------------------------------------------------------------------
// Called when all datablocks have been transmitted.
function GameConnection::onEnterGame(%client) {
   // Select camera.
   if($flyCamera) {
      %c = FlyCamera.init(%client, GameGroup);
      %c.setTransform("0 0 10 0 0 1 0");
      FlyCamera.controls(true);
      setFOV(50);
   } else {
      %c = TrackingCamera.init(%client, GameGroup, Knights, y);
      %c.setTransform(30*.75 SPC 0 SPC 30 / 2 SPC
         "0.255082 0.205918 -0.944739 1.41418");
      TrackingCamera.controls(true);
      setFOV(50);
   }

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
   Cover.onStart();

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
