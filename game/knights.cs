new SimGroup(Knights);

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

function knight(%name, %pos) {
   // Create the object itself with a name, position and datablock.
   %knight = new Player(%name) {
      datablock = DefaultPlayer;
      position = %pos;
   };
   Knights.add(%knight);

   // Bind the knight's name's first letter to select it.
   %letter = getSubstr(%name, 0, 1);
   Knights.selectMap.bindCmd(keyboard, "shift" SPC %letter, "Knights.select(" @ %name @ ");", "");
}

function Knights::onStart(%this) {
   GameGroup.add(%this);

   // ActionMaps allows us to capture input.
   %this.selectMap = new ActionMap();

   // Create four protagonists!
   knight(Juliet, "-2 2 0");
   knight(Kilo, "2 2 0");
   knight(Hotel, "-2 -2 0");
   knight(November, "2 -2 0");

   // Shortcut to selecting them all.
   %this.selectMap.bindCmd(keyboard, "a", "Knights.selectAll();", "");

   %this.selected = new SimSet();
}

function Knights::onEnterGame(%this, %client) {
   // Enable knight selection.
   %this.selectMap.push();
}

function Knights::onEnd(%this) {
   Knights.selectMap.delete();
   %this.selected.delete();
}

function Knights::select(%this, %knight) {
   %this.selected.add(%knight);
   %this.endSelection();
}

function Knights::deselect(%this, %knight) {
   if(%this.selected.contains(%knight)) {
      %this.selected.remove(%knight);
   }
}

function Knights::selectAll(%this) {
   foreach(%knight in Knights) {
      %this.select(%knight);
   }
   %this.endSelection();
}

function Knights::deselectAll(%this) {
   foreach(%knight in Knights) {
      %this.deselect(%knight);
   }
}

function Knights::endSelection(%this) {
   %this.selectMap.pop();
   Verbs.map.push();
}

