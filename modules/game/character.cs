function Character::stopAll(%this, %obj) {
   %obj.setImageTrigger(0, false);
   %obj.clearPathDestination();
}

function Character::goTo(%this, %obj, %pos, %slowdown) {
   if(!%obj.setPathDestination(%pos, %slowdown)) {
      %obj.setMoveDestination(%pos, %slowdown);
   }
}

function Character::takeCover(%this, %obj, %cover) {
   %this.goTo(%obj, %cover.getPosition());
}

function SceneObject::damage(%this, %amount) {
   if(%this.can(getDataBlock)) {
      if(%this.getDataBlock().can(damage)) {
         %this.getDataBlock().damage(%this, %amount);
      }
   }
}

function Character::damage(%this, %obj, %amount) {
   %obj.applyDamage(%amount);
   if(%obj.getDamagePercent() >= 1) {
      %this.onDestroyed(%obj);
   } else {
      %this.onDamaged(%obj, %amount);
   }
}

function Character::onDestroyed(%this, %obj) {
   %obj.blowUp();
   %obj.startFade(200, 0, true);
   %obj.schedule(200, delete);
}

function Character::onDamaged(%this, %obj, %amount) {}
