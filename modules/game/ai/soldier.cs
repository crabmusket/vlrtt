new ScriptObject(SoldierBrainTemplate) {
   transition[ready, playerNear] = attack;
   transition[attack, targetDeath] = attack;
};

function SoldierBrain::enterAttack(%this) {
   %obj = %this.owner;
   %point = std.findClosest(Cover.enemyPoints, %obj);
   %knight = std.findClosest(Knights, %obj, %obj.getAimObject());
   if(%knight) {
      %obj.getDataBlock().takeCover(%obj, %point);
      %obj.setMoveSpeed(0.5);
      %obj.setAimObject(%knight, "0 0" SPC $CharacterHeight);
      //%obj.setImageTrigger(0, true);
   } else {
      %obj.stopAll();
   }
}

function SoldierBrain::leaveAttack(%this) {
   %obj = %this.owner;
   %obj.setMoveSpeed(1.0);
}
