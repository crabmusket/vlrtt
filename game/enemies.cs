new SimGroup(Enemies);

datablock PlayerData(Soldier : Knight) {};

singleton Material(EnemyMaterial) {
   diffuseColor[0] = "1 0 0";
   mapTo = "enemyPlayer";
};

function soldier(%pos) {
   %soldier = new AIPlayer() {
      datablock = Soldier;
      position = %pos;
      skin = enemy;
   };
   Enemies.add(%soldier);
}

function Enemies::onStart(%this) {
   GameGroup.add(Enemies);

   // Create a keymap that accepts every key.
   %this.targetMap = new ActionMap();
   %this.letters = "jkhnfdgvcbtyursieowpxqlazm";
   for(%i = 0; %i < strLen(%this.letters); %i++) {
      %action = getSubStr(%this.letters, %i, 1);
      %this.targetMap.bindCmd(keyboard, %action, "Enemies.target(" @ %action @ ");");
   }
}

function Enemies::onEnd(%this) {
   %this.targetMap.delete();
}

function Enemies::target(%this, %char) {
   %index = strPos(%this.letters, %char);
   if(%index < %this.size()) {
      Verbs.target = %this.getObject(%index);
      Verbs.onEvent(enemyTargeted);
   }
}
