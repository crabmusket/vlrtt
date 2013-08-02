new SimGroup(Knights);

//-----------------------------------------------------------------------------
// Create the player material.
singleton Material(PlayerMaterial) {
   diffuseColor[0] = "0 1 0";
   mapTo = "basePlayer";
};

//-----------------------------------------------------------------------------
// Basic protagonist datablock.
datablock PlayerData(KnightBase) {
   class = Knight;
   superclass = Character;
   shapeFile = "./player.dae";
   maxDamage = 100;
   destroyedLevel = 100;
   debris = KnightDebris;
};

datablock DebrisData(KnightDebris) {
   shapeFile = "./projectile.dae";
};

datablock PlayerData(Shooter : KnightBase) { melee = false; };
function Shooter::onAdd(%this, %obj) {
   %obj.mountImage(RangedWeapon, 0);
}

datablock PlayerData(Fighter : KnightBase) { melee = true; };
function Fighter::onAdd(%this, %obj) {
   %obj.mountImage(MeleeWeapon, 0);
}

datablock PlayerData(Healer  : KnightBase) { melee = true; };
function Healer::onAdd(%this, %obj) {
   %obj.mountImage(HealWeapon, 0);
}

function knight(%name, %pos, %role) {
   // Create the object itself with a name, position and datablock.
   %knight = new AIPlayer(%name) {
      datablock = %role $= "" ? KnightBase : %role;
      position = %pos;
   };
   Knights.add(%knight);

   // Bind the knight's name's first letter to select it.
   %action = getSubstr(%name, 0, 1);
   Knights.selectMap.bindCmd(keyboard, %action, "Knights.select(" @ %name @ ");");

   // Bind the same keypress to target the knight.
   Knights.targetMap.bindCmd(keyboard, %action, "Knights.target(" @ %name @ ");");
}

function Knight::attack(%this, %obj, %target) {
   if(%this.melee) {
      %obj.setMoveDestination(%target.getPosition());
   } else {
      %obj.setAimObject(%target);
      %obj.setImageTrigger(0, true);
   }
}

//-----------------------------------------------------------------------------

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
