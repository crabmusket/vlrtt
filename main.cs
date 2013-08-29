//-----------------------------------------------------------------------------
// Entry point to the engine. Everything begins here.

exec("ts/shim.cs");
exec("ts/std.cs");

// Seed the random generator.
setRandomSeed(getRealTime());

// Console does something.
setLogMode(2);
// Disable script trace.
trace(false);

//-----------------------------------------------------------------------------
// Load up scripts to initialise subsystems.
include(sys);
$modulePath = "modules";

// The canvas needs to be initialized before any gui scripts are run since
// some of the controls assume that the canvas exists at load time.
createCanvas("vlrtt");

// Start rendering and stuff.
initRenderManager();
initLightingSystems("Advanced Lighting"); 
initPostEffects();

// Start audio.
sfxStartup();

//-----------------------------------------------------------------------------
// Load console.
include(console);
include(metrics);

// Load up game code.
include(game);

// Called when we connect to the local game.
function GameConnection::onConnect(%client) {
   %client.transmitDataBlocks(0);
}

// Called when all datablocks from above have been transmitted.
function GameConnection::onDataBlocksDone(%client) {
   // Start sending ghosts to the client.
   %client.activateGhosting();
   %client.onEnterGame();
}

// Create a local game server and connect to it.
new SimGroup(ServerGroup);
new GameConnection(ServerConnection);
// This calls GameConnection::onConnect.
ServerConnection.connectLocal();

// Start game-specific scripts.
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

function onDatablockObjectReceived() {}
function onGhostAlwaysObjectReceived() {}
function onGhostAlwaysStarted() {}
