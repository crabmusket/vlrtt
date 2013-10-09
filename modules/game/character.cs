$CharacterHeight = 1.7;

function Character::onAdd(%this, %obj) {
   %obj.setActionThread("stand_root");
}

function Character::stopAll(%this, %obj) {
   %obj.setImageTrigger(0, false);
   %obj.clearPathDestination();
   if(%obj.follow) {
      cancel(%obj.follow);
      %obj.follow = "";
   }
}

function AIPlayer::stopAll(%this) {
   %this.getDataBlock().stopAll(%this);
}

function AIPlayer::goTo(%obj, %pos, %slowdown) {
   %obj.getDataBlock().goTo(%obj, %pos, %slowdown);
}

function Character::goTo(%this, %obj, %pos, %slowdown) {
   %obj.isTakingCover = false;
   %obj.setActionThread("stand_root");
   // Beginning the movement immediately cancels the animation, so delay it.
   %obj.schedule(100, delayedGoTo, %pos, %slowdown);
}

function AIPlayer::delayedGoTo(%this, %pos, %slowdown) {
   if(!%this.setPathDestination(%pos, %slowdown)) {
      %this.setMoveDestination(%pos, %slowdown);
   }
}

function AIPlayer::follow(%obj, %follow) {
   if(isObject(%follow)) {
      %obj.setAimObject(%follow, "0 0" SPC $CharacterHeight);
      %obj.goTo(%follow.position, false);
      %obj.follow = %obj.schedule(500, follow, %follow);
   } else {
      %obj.follow = "";
   }
}

function Character::takeCover(%this, %obj, %cover) {
   %this.goTo(%obj, %cover.getPosition(), false);
   %obj.isTakingCover = true;
}

function Character::onReachPathDestination(%this, %obj) {}

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
   CharacterEvents.postEvent("CharacterDeath", %obj);
}

function Character::onDamaged(%this, %obj, %amount) {}

function Character::onCollision(%this, %obj, %col) {
   if(%obj.combat $= "" && %col.combat $= "" && %obj.attacking == %col) {
      Combats.begin(%obj, %col, %col.attacking == %obj);
   }
}

function AIPlayer::joinCombat(%obj, %combat) {
   %obj.getDataBlock().joinCombat(%obj, %combat);
}

function Character::joinCombat(%this, %obj, %enemy) {
   // Focus on combat!
   %obj.stopAll();
   %obj.follow(%enemy);
}
