//-----------------------------------------------------------------------------
// Game mainfile. Defines what happens when the root main.cs has set everything
// up properly.

// Module dependencies.
execModule("twillex");
execModule("stateMachine");
execModule("offsetCamera");
execModule("bottomPrint");
execModule("navigation");
execModule("level");

// Scripts that make up this module.
exec("./events.cs");
exec("./verbs.cs");
exec("./character.cs");
exec("./knights.cs");
exec("./enemies.cs");
exec("./cover.cs");
exec("./combat.cs");

// GUIs.
exec("./playGui.gui");

//-----------------------------------------------------------------------------
// Called when all datablocks have been transmitted.
function GameConnection::onEnterGame(%client) {
   // Create a camera for the client to view the game.
   %c = OffsetCamera.init(%client, GameGroup, Knights, "20 -3 10");
   %c.setTransform("0 0 0" SPC "0.255082 0.205918 -0.944739 1.41418");
   OffsetCamera.controls(true);
   setFOV(50);

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

   // Global tweening engine.
   Twillex::create(Tweens);
   Tweens.startUpdates();

   // Gameplay modules.
   Knights.onStart();
   Enemies.onStart();
   Level.onStart();
   Cover.onStart();

   // UI modules.
   Verbs.onStart();
}

//-----------------------------------------------------------------------------
// Called when the engine is shutting down.
function onEnd() {
   Verbs.onEnd();
   Knights.onEnd();
   Enemies.onEnd();

   Tweens.delete();

   // Delete the objects we created.
   GameGroup.delete();
}
