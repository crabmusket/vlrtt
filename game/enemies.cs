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
}

function Enemies::onEnd(%this) {
}
