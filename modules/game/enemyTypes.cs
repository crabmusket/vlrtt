singleton Material(EnemyMaterial) {
   diffuseColor[0] = "1 0 0";
   mapTo = "enemyPlayer";
};

foreach$(%type in $EnemyTypes) {
   eval(
"function Enemies::"@%type@"(%this, %pos) {"     @
"   %obj = new AIPlayer() {"                     @
"      datablock = "@%type@";"                   @
"      skin = enemy;"                            @
"      position = %pos;"                         @
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

   skill = 2;
};

function Soldier::onAdd(%this, %obj) {
   Parent::onAdd(%this, %obj);
}

//-----------------------------------------------------------------------------
// Big slow type.

datablock PlayerData(Brute : KnightBase) {
   class = Enemy;
   debrisShapeName = "./shapes/enemyDebris.dae";

   maxForwardSpeed = 2;
   maxSideSpeed = 2;
   maxBackwardSpeed = 2;

   skill = 1;
};

function Brute::onAdd(%this, %obj) {
   %obj.scale = "2 1.5 1.5";
   Parent::onAdd(%this, %obj);
}
