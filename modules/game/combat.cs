new ScriptObject(Combats);

function Combats::begin(%this, %attacker, %defender, %mutual) {
   //%c = new ScriptObjct() {
   //   class = Combat;
   //   participants = %attacker SPC %defender;
   //};
   %attacker.joinCombat(%defender, %mutual);
   %defender.joinCombat(%attacker, %mutual);
}

