singleton Material(EnemyMaterial) {
   diffuseColor[0] = "1 0 0";
   mapTo = "enemyPlayer";
};

foreach$(%type in "Soldier Berserker") {
   eval(
"function Enemies::"@%type@"(%this, %pos) {"     @
"   %obj = new AIPlayer() {"                     @
"      datablock = "@%type@";"                   @
"      position = %pos;"                         @
"      skin = enemy;"                            @
"      rotation = \"0 0 1 180\";"                @
"   };"                                          @
"   AI.brain(%obj, "@%type@");"                  @
"   Enemies.add(%obj);"                          @
"   return %obj;"                                @
"}"
   );
}

//-----------------------------------------------------------------------------
// Soldier type

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
   skill = 1;
};

