new ScriptObject(BerserkerBrainTemplate) {
   transition[ready, playerNear] = attack;
   transition[attack, targetDeath] = attack;
};

function BerserkerBrain::enterAttack(%this) {
   %obj = %this.owner;
   %knight = std.findClosest(Knights, %obj, %obj.getAimObject());
   if(%knight) {
      %obj.goTo(%knight.position, false);
      %obj.setAimObject(%knight, "0 0" SPC $CharacterHeight);
   } else {
      %obj.stopAll();
   }
}

