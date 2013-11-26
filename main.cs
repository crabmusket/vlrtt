//-----------------------------------------------------------------------------
// Entry point to the engine. Everything begins here.

// Make TorqueScript a bit more familiar to users of other languages.
exec("ts/shim.cs");
// Some useful functions.
exec("ts/std.cs");

// Seed the random generator.
setRandomSeed(getRealTime());

// Console does something.
setLogMode(2);
// Disable script trace.
trace(false);

//-----------------------------------------------------------------------------
// Load up scripts to initialise subsystems.
exec("sys/main.cs");

// The canvas needs to be initialized before any gui scripts are run since
// some of the controls assume that the canvas exists at load time.
createCanvas("vlrtt");

// Start rendering and stuff.
initRenderManager();
initLightingSystems("Advanced Lighting"); 
initPostEffects();

// Start audio.
sfxStartup();

// Provide stubs so we don't get console spam.
function onDatablockObjectReceived() {}
function onGhostAlwaysObjectReceived() {}
function onGhostAlwaysStarted() {}
function updateTSShapeLoadProgress() {}

// Convenience function: execute the main.cs file of some directory in modules/.
function execModule(%name) {
   exec("modules/" @ %name @ "/main.cs");
}

//-----------------------------------------------------------------------------
// Load console.

execModule("console");
execModule("metrics");

// Load up game code.
execModule("game");

// Called when we connect to the local game.
function GameConnection::onConnect(%client) {
   %client.transmitDataBlocks(0);
}

// Called when all datablocks from above have been transmitted.
function GameConnection::onDataBlocksDone(%client) {
   // Start sending ghosts to the client.
   %client.activateGhosting();
   // Enter game callback.
   %client.onEnterGame();
}

// Create a local game server and connect to it.
new SimGroup(ServerGroup);
new GameConnection(ServerConnection);
// This calls GameConnection::onConnect.
ServerConnection.connectLocal();

// Start game-specific scripts. Defined in modules/game/main.cs
onStart();

//-----------------------------------------------------------------------------
// Called when the engine is shutting down.
function onExit() {
   // Clean up game objects and so on.
   onEnd();

   // Delete the connection if it's still there.
   ServerConnection.delete();
   ServerGroup.delete();

   // Delete all the datablocks.
   deleteDataBlocks();
}
