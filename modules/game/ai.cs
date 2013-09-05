new SimSet(AI);

new ScriptObject(SoldierBrainTemplate) {
   transition[ready, playerNear] = attackWhileMoving;
};

function AI::brain(%this, %template, %obj) {
   %templateObj = %template @ BrainTemplate;
   eval("%obj.brain = new ScriptObject(\"\" : " @ %templateObj @ ");");
   %obj.brain.superclass = StateMachine;
   %obj.brain.class = %template @ Brain;
   %obj.brain.owner = %obj;
   %obj.brain.state = null;
   %obj.brain.transition[null, ready] = ready;
   %obj.brain.onEvent(ready);
}

function SoldierBrain::enterAttackWhileMoving(%this) {
   %obj = %this.owner;
   %point = std.findClosest(Cover.enemyPoints, %obj);
   %knight = std.findClosest(Knights, %obj);
   %obj.getDataBlock().takeCover(%obj, %point);
   %obj.setAimObject(%knight);
   %obj.setImageTrigger(0, true);
}

datablock TriggerData(EnemyAITrigger) {};
