new SimSet(Enemies);

exec("./ai.cs");

datablock PlayerData(Soldier : KnightBase) {
   melee = false;
   class = Enemy;
   debrisShapeName = "./shapes/enemyDebris.dae";
   maxForwardSpeed = 5;
   maxSideSpeed = 5;
   maxBackwardSpeed = 5;
};

singleton Material(EnemyMaterial) {
   diffuseColor[0] = "1 0 0";
   mapTo = "enemyPlayer";
};

function Enemy::onReachPathDestination(%this, %obj) {
   %obj.brain.onEvent(onReachPathDestination);
   Parent::onReachPathDestination(%this, %obj);
}

function soldier(%pos) {
   %soldier = new AIPlayer() {
      datablock = Soldier;
      position = %pos;
      skin = enemy;
      rotation = "0 0 1 180";
   };
   AI.brain(Soldier, %soldier);
   Enemies.add(%soldier);
   return %soldier;
}

function Enemies::onStart(%this) {
   GameGroup.add(Enemies);

   // Create a keymap that accepts every key.
   %this.targetMap = new ActionMap();
   %this.letters = "jkhnfdgvcbtyursieowpxqlazm";
   for(%i = 0; %i < strLen(%this.letters); %i++) {
      %action = getSubStr(%this.letters, %i, 1);
      %this.targetMap.bindCmd(keyboard, %action, "Enemies.target(" @ %i @ ");");
   }
}

function Enemies::onEnd(%this) {
   %this.targetMap.delete();
}

function Enemies::beginTarget(%this) {
   %this.targetMap.push();
   %i = 0;
   Enemies.sort(distanceFromKnights);
   foreach(%enemy in Enemies) {
      %enemy.setShapeName(" "  @ getSubStr(%this.letters, %i, 1) @ " ");
      %i++;
      if(%i == strLen(%this.letters)) break;
   }
}

function Enemies::endTarget(%this) {
   %this.targetMap.pop();
   foreach(%enemy in Enemies) {
      %enemy.setShapeName("");
   }
}

function Enemies::target(%this, %index) {
   if(%index < %this.size()) {
      Verbs.target = %this.getObject(%index);
      Verbs.onEvent(enemyTargeted);
   }
}
