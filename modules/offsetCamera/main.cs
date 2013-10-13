new ScriptObject(OffsetCamera);

datablock CameraData(OffsetCameraData) {};

function OffsetCamera::init(%this, %client, %group, %follow, %offset) {
   // Create a camera for the client.
   %c = new Camera(TheCamera) {
      datablock = OffsetCameraData;
      _offsetCameraGroup = %follow;
      _offsetCameraOffset = %offset;
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

function OffsetCameraData::onAdd(%this, %obj) {
   %obj._offsetCameraUpdate();
}

function OffsetCameraData::onRemove(%this, %obj) {
   cancel(%obj._offsetCameraSchedule);
}

function Camera::_offsetCameraUpdate(%this) {
   // Schedule the next update.
   %this._offsetCameraSchedule = %this.schedule(15 /* 60 Hz */, _offsetCameraUpdate);

   // Check that we've got objects to track.
   if(%this._offsetCameraGroup.size() == 0) {
      return;
   }

   // First get the average positions on all three axes.
   %pos = "0 0 0";
   foreach(%obj in %this._offsetCameraGroup) {
      %pos = VectorAdd(%pos, %obj.getPosition());
   }
   %pos = VectorScale(%pos, 1 / %this._offsetCameraGroup.size());

   // Apply the offset and existing rotation.
   %pos = VectorAdd(%pos, %this._offsetCameraOffset);
   %pos = %pos SPC getWords(%this.getTransform(), 3, 6);

   // Apply the updated transform.
   %this.setTransform(%pos);
}

function pitch(%amount) { $mvPitch += %amount * 0.01; }
function yaw(%amount) { $mvYaw += %amount * 0.01; }

function OffsetCamera::controls(%this, %controls) {
   %this.actions.call(%controls? "push" : "pop");
}
