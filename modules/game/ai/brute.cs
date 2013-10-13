new ScriptObject(BruteBrainTemplate) {
   transition[ready, playerNear] = attack;
   transition[attack, targetDeath] = attack;
};

function BruteBrain::enterAttack(%this) {
   %obj = %this.owner;
   %knight = std.findClosest(Knights, %obj, %obj.getAimObject());
   if(%knight) {
      %obj.follow(%knight);
      %obj.attacking = %knight;
   } else {
      %obj.stopAll();
   }
}

