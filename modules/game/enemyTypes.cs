singleton Material(EnemyMaterial) {
   diffuseColor[0] = "1 0 0";
   mapTo = "enemyPlayer";
};

//-----------------------------------------------------------------------------
// Soldier type

function soldier(%pos) {
   %soldier = new AIPlayer() {
      datablock = Soldier;
      position = %pos;
      skin = enemy;
      rotation = "0 0 1 180";
   };
   AI.brain(%soldier, Soldier);
   Enemies.add(%soldier);
   return %soldier;
}

datablock PlayerData(Soldier : KnightBase) {
   class = Enemy;
   debrisShapeName = "./shapes/enemyDebris.dae";
   maxForwardSpeed = 5;
   maxSideSpeed = 5;
   maxBackwardSpeed = 5;
};

function Soldier::onAdd(%this, %obj) {
   %obj.mountImage(RangedWeapon, 0);
   Parent::onAdd(%this, %obj);
}

//-----------------------------------------------------------------------------
// Melee type

datablock PlayerData(Berserker : KnightBase) {
   class = Enemy;
   debrisShapeName = "./shapes/enemyDebris.dae";
   maxForwardSpeed = 7;
   maxSideSpeed = 5;
   maxBackwardSpeed = 5;
};

