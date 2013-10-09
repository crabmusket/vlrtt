new ScriptObject(Combats);

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
   %sps = 5 * getMax(1, %attacker.getDataBlock().skill - %defender.getDataBlock().skill);
   %defender.setRechargeRate(-%sps/32);
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
