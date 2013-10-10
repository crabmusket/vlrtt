new ScriptObject(Combats);

$UpdateENergyPeriod = 500; // Milliseconds.

function Character::onCombatBegin(%this, %obj, %data) {
   %subject = getWord(%data, 0);
   if(%obj != %subject) {
      return;
   }

   // Remember the new enemy.
   %subject.fighting = %subject.fighting SPC %enemy;
   %enemies = getWordCount(%subject.fighting);

   // Update SPS.
   if(%enemies > 1) {
      %max = 0;
      foreach$(%enemy in %obj.fighting) {
         if(%enemy.skill > %max) {
            %max = %enemy.skill;
         }
      }
      %sps = 5 * getMax(1, %max + %enemies - %subject.skill);
   } else {
      %enemy = getWord(%data, 1);
      %sps = 5 * getMax(1, %enemy.skill - %subject.skill);
      %subject.updateEnergy = %obj.schedule(getRandom(1, $UpdateEnergyPeriod), updateEnergy);
   }
   %subject.setRechargeRate(-%sps/32);
}

function AIPlayer::updateEnergy(%obj) {
   if(%obj.fighting $= "") {
      return;
   }
   %obj.schedule($UpdateEnergyPeriod, updateEnergy);

   if(%obj.getEnergyLevel() <= 1) {
      postEvent(Combat, 
}

function AIPlayer::joinCombat(%obj, %combat) {
   %obj.getDataBlock().joinCombat(%obj, %combat);
}

function Character::joinCombat(%this, %obj, %enemy) {
   // Focus on combat!
   %obj.stopAll();
   %obj.follow(%enemy);
}

function Combats::begin(%this, %attacker, %defender, %mutual) {
   %c = new ScriptObject() {
      class = Combat;
      attacker = %attacker;
      defender = %defender;
   };
   %attacker.combat = %c;
   %defender.combat = %c;
   %attacker.joinCombat(%defender, %mutual);
   %defender.joinCombat(%attacker, %mutual);
   %c.fight();
}

function Combat::blows(%this, %attacker, %defender) {
   //echo("defender energy:" SPC %defender.getEnergyLevel());
   if(%defender.getEnergyLevel() < 1) {
      %amount = getRandom(40, 120);
      %defender.damage(%amount);
      //echo(%attacker SPC "hits" SPC %defender SPC "for" SPC %amount);
      %defender.schedule(100, adrenaline, 20);
      %attacker.schedule(100, adrenaline, 10);
   }
}

function Combat::aftermath(%this, %obj) {
   if(%obj.getDamagePercent() >= 1) {
      %this.shouldEnd = true;
   }
}

function Combat::prepare(%this, %attacker, %defender) {
}

function Combat::release(%this, %obj) {
   %obj.stopAll();
   %obj.combat = "";
   %obj.setRechargeRate(0.1);
}

function Combat::fight(%this) {
   // Calculate SPS
   %this.prepare(%this.attacker, %this.defender);
   %this.prepare(%this.defender, %this.attacker);
   // Perform blows.
   %this.blows(%this.attacker, %this.defender);
   %this.blows(%this.defender, %this.attacker);
   // Clean up.
   %this.aftermath(%this.attacker);
   %this.aftermath(%this.defender);
   if(%this.shouldEnd) {
      %this.release(%this.defender);
      %this.release(%this.attacker);
      %this.schedule(100, delete);
   } else {
      %this.fighting = %this.schedule(500, fight);
   }
}

function AIPlayer::adrenaline(%obj, %amount) {
   %obj.setEnergyLevel(%obj.getEnergyLevel() + %amount);
}
