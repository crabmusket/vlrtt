new ScriptObject(TrackingCamera);

datablock CameraData(TrackingCameraData) {};

function TrackingCamera::init(%this, %client, %group, %follow, %axes) {
   // Create a camera for the client.
   %c = new Camera(TheCamera) {
      datablock = TrackingCameraData;
      _trackingCameraGroup = %follow;
      _trackingCameraAxes = %axes;
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
      %this.actions = new ActionMap();
      %this.actions.bind(mouse, xaxis, yaw);
      %this.actions.bind(mouse, yaxis, pitch);
   }

   return %c;
}

function TrackingCameraData::onAdd(%this, %obj) {
   %obj._trackingCameraUpdate();
}

function TrackingCameraData::onRemove(%this, %obj) {
   cancel(%obj._trackingCameraSchedule);
}

function Camera::_trackingCameraUpdate(%this) {
   // Schedule the next update.
   %this._trackingCameraSchedule = %this.schedule(15 /* 60 Hz */, _trackingCameraUpdate);

   // Check that we've got objects to track.
   if(%this._trackingCameraGroup.size() == 0) {
      return;
   }

   // First get the average positions on all three axes.
   %pos = "0 0 0";
   foreach(%obj in %this._trackingCameraGroup) {
      %pos = VectorAdd(%pos, %obj.getPosition());
   }
   %pos = VectorScale(%pos, 1 / %this._trackingCameraGroup.size());

   // Now select which axes we actually want to update.
   %newPos = %this.getTransform();
   %axes = %this._trackingCameraAxes;
   if(strPos(%axes, x) != -1) { %newPos = setWord(%newPos, 0, getWord(%pos, 0)); }
   if(strPos(%axes, y) != -1) { %newPos = setWord(%newPos, 1, getWord(%pos, 1)); }
   if(strPos(%axes, z) != -1) { %newPos = setWord(%newPos, 2, getWord(%pos, 2)); }

   // Apply the updated transform.
   %this.setTransform(%newPos);
}

function pitch(%amount) { $mvPitch += %amount * 0.01; }
function yaw(%amount) { $mvYaw += %amount * 0.01; }

function TrackingCamera::controls(%this, %controls) {
   %this.actions.call(%controls? "push" : "pop");
}
