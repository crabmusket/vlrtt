new SimGroup(Knights);

//-----------------------------------------------------------------------------
// Create the player material.
singleton Material(PlayerMaterial) {
   diffuseColor[0] = "0 1 0";
   mapTo = "basePlayer";
};

//-----------------------------------------------------------------------------
// Create the player material.
datablock PlayerData(Knight) {
   shapeFile = "./player.dae";
   class = Character;
   maxDamage = 100;
   destroyedLevel = 100;
};

function knight(%name, %pos) {
   // Create the object itself with a name, position and datablock.
   %knight = new AIPlayer(%name) {
      datablock = Knight;
      position = %pos;
   };
   Knights.add(%knight);

   // Bind the knight's name's first letter to select it.
   %action = getSubstr(%name, 0, 1);
   Knights.selectMap.bindCmd(keyboard, %action, "Knights.select(" @ %name @ ");");

   // Bind the same keypress to target the knight.
   Knights.targetMap.bindCmd(keyboard, %action, "Knights.target(" @ %name @ ");");
}

function Knights::onStart(%this) {
   GameGroup.add(%this);

   // ActionMaps allows us to capture input.
   %this.selectMap = new ActionMap();
   %this.targetMap = new ActionMap();

   // Shortcut to selecting them all.
   %this.selectMap.bindCmd(keyboard, "a", "Knights.selectAll();", "");

   // Selected knights.
   %this.selected = new SimSet();
}

function Knights::onEnd(%this) {
   Knights.selectMap.delete();
   %this.selected.delete();
}

function Knights::select(%this, %knight) {
   %this.selected.add(%knight);
   Verbs.onEvent(knightSelected);
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
   Verbs.onEvent(knightSelected);
}

function Knights::deselectAll(%this) {
   foreach(%knight in Knights) {
      %this.deselect(%knight);
   }
}

function Knights::target(%this, %knight) {
   Verbs.target = %knight;
   Verbs.onEvent(knightTargeted);
}
