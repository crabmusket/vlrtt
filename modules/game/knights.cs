new SimGroup(Knights);

exec("./shapes/soldier.cs");

//-----------------------------------------------------------------------------
// Create the player material.
singleton Material(PlayerMaterial) {
   diffuseColor[0] = "0 1 0";
   mapTo = "basePlayer";
};

datablock DebrisData(KnightDebris) {
   numBounces = 1;
   velocity = 3;
   velocityVariance = 1;
};

//-----------------------------------------------------------------------------
// Basic protagonist datablock.
datablock PlayerData(KnightBase) {
   class = Knight;
   superclass = Character;
   shapeFile = "./shapes/soldier.dae";
   maxDamage = 100;
   destroyedLevel = 100;
   debrisShapeName = "./shapes/playerDebris.dae";
   debris = KnightDebris;
   maxForwardSpeed = 5;
   maxSideSpeed = 5;
   maxBackwardSpeed = 5;
};

function Knight::onAdd(%this, %obj) {
   CharacterEvents.subscribe(%obj, CharacterDeath);
   Parent::onAdd(%this, %obj);
}

function Knight::onReachPathDestination(%this, %obj) {
   if(%obj.isTakingCover) {
      %obj.setActionThread("hide_root");
   }
   Parent::onReachPathDestination(%this, %obj);
}

function Knight::goTo(%this, %obj, %dest, %slowdown, %speed) {
   if(%speed $= "") {
      %speed = 0.5;
   }
   if(%speed > 0.5) {
      %obj.clearAim();
      %obj.setImageTrigger(0, false);
   }
   Parent::goTo(%this, %obj, %dest, %slowdown);
   %obj.setMoveSpeed(%speed);
}

datablock PlayerData(Shooter : KnightBase) {};
function Shooter::onAdd(%this, %obj) {
   %obj.mountImage(RangedWeapon, 0);
   Parent::onAdd(%this, %obj);
}

function Shooter::onCharacterDeath(%this, %obj, %dead) {
   if(%dead $= %obj.getAimObject()) {
      %obj.clearAim();
      %obj.setImageTrigger(0, false);
   }
}

datablock PlayerData(Fighter : KnightBase) {};
function Fighter::onAdd(%this, %obj) {
   %obj.mountImage(MeleeWeapon, 0);
   Parent::onAdd(%this, %obj);
}

datablock PlayerData(Healer  : KnightBase) {};
function Healer::onAdd(%this, %obj) {
   %obj.mountImage(HealWeapon, 0);
   Parent::onAdd(%this, %obj);
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

   return %knight;
}

//-----------------------------------------------------------------------------

function Shooter::attack(%this, %obj, %target) {
   if(%obj.getMoveSpeed() > 0.5) {
      %obj.setMoveSpeed(0.5);
   }
   %obj.setAimObject(%target, "0 0" SPC $CharacterHeight);
   %obj.setImageTrigger(0, true);
}

function Fighter::attack(%this, %obj, %target) {
   %obj.follow(%target);
}

function Fighter::onCollision(%this, %obj, %col) {
   if(Enemies.contains(%col)) {
      %col.damage(40);
      %sep = VectorSub(%col.getPosition(), %obj.getPosition());
      %col.applyImpulse(%col.getPosition(), VectorScale(%sep, 50));
   }
}

function Healer::attack(%this, %obj, %target) {}

function Knight::heal(%this, %obj, %target) {
   %obj.goTo(%target.getPosition(), true, 1.0);
   %obj.setImageTrigger(0, false);
}

function Knight::stopAll(%this, %obj) {
   Parent::stopAll(%this, %obj);
   %obj.setAimLocation(Level.forwards);
   %obj.schedule(200, clearAim);
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

//-----------------------------------------------------------------------------

function Knights::nameKnights(%this, %name) {
   foreach(%knight in Knights) {
      if(%name) {
         %knight.setShapeName(" " @ getSubstr(%knight.getName(), 0, 1) @ " ");
      } else {
         %knight.setShapeName("");
      }
   }
}

function Knights::beginSelect(%this) {
   %this.selectMap.push();
   %this.nameKnights(true);
}

function Knights::beginTarget(%this) {
   %this.targetMap.push();
   %this.nameKnights(true);
}

function Knights::endSelect(%this) {
   %this.selectMap.pop();
   %this.nameKnights(false);
}

function Knights::endTarget(%this) {
   %this.targetMap.pop();
   %this.nameKnights(false);
}

function Knights::select(%this, %knight, %noevent) {
   %this.selected.add(%knight);
   %knight.mountImage(Selectron, 1);
   if(%noevent $= "") {
      Verbs.onEvent(knightSelected);
   }
}

function Knights::deselect(%this, %knight) {
   %knight.unmountImage(1);
   if(%this.selected.contains(%knight)) {
      %this.selected.remove(%knight);
   }
}

function Knights::selectAll(%this) {
   foreach(%knight in Knights) {
      %this.select(%knight, true);
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

//-----------------------------------------------------------------------------

datablock ShapeBaseImageData(Selectron) {
   shapeFile = "./shapes/selectron.dae";
   offset = "0 0 0.25";
};

//-----------------------------------------------------------------------------

function distanceFromKnights(%a, %b) {
   %distA = 10000;
   %distB = 10000;
   foreach(%knight in Knights.selected) {
      %dist = VectorLen(VectorSub(%a.getPosition(), %knight.getPosition()));
      %distA = %dist < %distA? %dist : %distA;
      %dist = VectorLen(VectorSub(%b.getPosition(), %knight.getPosition()));
      %distB = %dist < %distB? %dist : %distB;
   }
   if(%distA < %distB) return -1;
   if(%distA > %distB) return 1;
   return 0;
}
