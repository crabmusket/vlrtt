new ScriptObject(FlyCamera);

//-----------------------------------------------------------------------------
// Create a datablock for the observer camera.
datablock CameraData(Observer) {};

function FlyCamera::init(%this, %client, %group) {
   // Create a camera for the client.
   %c = new Camera() {
      datablock = Observer;
   };

   // Add to SimGroup if one is given.
   if(isObject(%group)) {
      %group.add(%c);
   }

   // Cameras are not ghosted (sent across the network) by default; we need to
   // do it manually for the client that owns the camera or things will go south
   // quickly.
   %c.scopeToClient(%client);

   // And let the client control the camera.
   %client.setControlObject(%c);

   // If there's no input, capture some!
   if(!isObject(%this.map)) {
      %this.map = new ActionMap();
      %this.map.bindCmd(mouse, button0, "$mvForwardAction = 1;", "$mvForwardAction = 0;");
      %this.map.bindCmd(mouse, button1, "$mvBackwardAction = 1;", "$mvBackwardAction = 0;");
      %this.map.bind(mouse, xaxis, yaw);
      %this.map.bind(mouse, yaxis, pitch);
   }

   return %c;
}

function pitch(%amount) { $mvPitch += %amount * 0.01; }
function yaw(%amount) { $mvYaw += %amount * 0.01; }

function FlyCamera::controls(%this, %controls) {
   %this.map.call(%controls? "push" : "pop");
}
