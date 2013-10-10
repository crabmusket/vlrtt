new ScriptObject(Combats);

$UpdateEnergyPeriod = 500; // Milliseconds.

function Character::onCombatBegin(%this, %obj, %data) {
   if(%obj != getWord(%data, 0)) {
      return;
   }
   %enemy = getWord(%data, 1);

   // Remember the new enemy.
   if(%obj.fighting) {
      %obj.fighting = %obj.fighting SPC %enemy;
   } else {
      %obj.fighting = %enemy;
   }
   %enemies = getWordCount(%obj.fighting);

   // Update SPS.
   if(%enemies > 1) {
      %max = 0;
      foreach$(%enemy in %obj.fighting) {
         if(%enemy.skill > %max) {
            %max = %enemy.skill;
         }
      }
      %sps = 5 * (getMax(1, %max - %obj.skill) + %enemies);
   } else {
      %sps = 5 * getMax(1, %enemy.skill - %obj.skill);
      %obj.updateEnergyLevel = %obj.schedule(getRandom(1, $UpdateEnergyPeriod), updateEnergy);
   }
   %obj.setRechargeRate(-%sps/32);

   // Stop what you're doing!
   %obj.stopAll();
   %obj.setAimObject(%enemy);
}

function AIPlayer::updateEnergy(%obj) {
   if(%obj.fighting $= "") {
      return;
   }
   %obj.updateEnergyLevel = %obj.schedule($UpdateEnergyPeriod, updateEnergy);

   if(%obj.getEnergyLevel() <= 1) {
      postEvent(Combat, antExhausted /* lol */, %obj);
   }
}

function Character::onCombatantExhausted(%this, %obj, %enemy) {
   if(%enemy.isAWordIn(%obj.fighting)) {
      // Strike a blow!
      %enemy.damage(getRandom(40, 120));
      %obj.adrenaline(10);
      %enemy.adrenaline(20);
   }
}

function Character::onCharacterDeath(%this, %obj, %character) {
   if(%obj.fighting) {
      %obj.removeEnemy(%character);
   }
}

function AIPlayer::removeEnemy(%obj, %enemy) {
   // Remove enemy from list.
   %i = 0;
   foreach$(%c in %obj.fighting) {
      if(%c == %character) {
         %obj.fighting = removeWord(%obj.fighting, %i);
         break;
      }
      %i++;
   }

   // No enemies remaining?
   if(!%obj.fighting) {
      cancel(%obj.updateEnergyLevel);
      %obj.updateEnergyLevel = "";
      %obj.setRechargeRate(10/32);
      %obj.stopAll();
   }
}

function AIPlayer::adrenaline(%obj, %amount) {
   %obj.setEnergyLevel(%obj.getEnergyLevel() + %amount);
}
