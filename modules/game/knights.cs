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
   superclass = Character;

   shapeFile = "./shapes/soldier.dae";
   debrisShapeName = "./shapes/playerDebris.dae";
   debris = KnightDebris;

   maxDamage = 100;
   destroyedLevel = 100;

   maxForwardSpeed = 5;
   maxSideSpeed = 5;
   maxBackwardSpeed = 5;

   maxEnergy = 100;

   skill = 5;
};

function KnightBase::onAdd(%this, %obj) {
   CharacterEvents.subscribe(%obj, CharacterDeath);
   %obj.side = Knights;
   Parent::onAdd(%this, %obj);
}

function KnightBase::onReachPathDestination(%this, %obj) {
   if(%obj.isTakingCover) {
      %obj.setActionThread("hide_root");
   }
   Parent::onReachPathDestination(%this, %obj);
}

function KnightBase::goTo(%this, %obj, %dest, %slowdown, %speed) {
   if(%speed $= "") {
      %speed = 1;
   }
   if(%speed > 0.5) {
      %obj.clearAim();
      %obj.setImageTrigger(0, false);
   }
   Parent::goTo(%this, %obj, %dest, %slowdown);
   %obj.setMoveSpeed(%speed);
}

//-----------------------------------------------------------------------------

function KnightBase::attack(%this, %obj, %target) {
   %obj.setMoveSpeed(1);
   %obj.follow(%target);
   %obj.attacking = %target;
}

function KnightBase::stopAll(%this, %obj) {
   Parent::stopAll(%this, %obj);
   %obj.clearAim();
}

//-----------------------------------------------------------------------------

function Knights::onStart(%this) {
   GameGroup.add(%this);
}

function Knights::onEnd(%this) {
}

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
