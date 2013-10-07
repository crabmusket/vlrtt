new ScriptObject(BerserkerBrainTemplate) {
   transition[ready, playerNear] = attack;
   transition[attack, targetDeath] = attack;
};

function BerserkerBrain::enterAttack(%this) {
   %obj = %this.owner;
   %knight = std.findClosest(Knights, %obj, %obj.getAimObject());
   if(%knight) {
      %obj.follow(%knight);
   } else {
      %obj.stopAll();
   }
}

