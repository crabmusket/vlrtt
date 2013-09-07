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
   %obj.setMoveSpeed(0.5);
   %obj.setAimObject(%knight);
   %obj.setImageTrigger(0, true);
}

function SoldierBrain::leaveAttackWhileMoving(%this) {
   %obj = %this.owner;
   %obj.setMoveSpeed(1.0);
}

function Enemy::onKnightEnterSection(%this, %obj, %data) {
   %knight = getWord(%data, 0);
   %trigger = getWord(%data, 1);
   %obj.brain.onEvent(playerNear);
}

datablock TriggerData(SectionTrigger) {};

function SectionTrigger::onEnterTrigger(%this, %trigger, %enter) {
   if(!%enter.isIn(Knights)) {
      return;
   }
   KnightEvents.postEvent(KnightEnterSection, %enter SPC %trigger);
}

function SectionTrigger::onLeaveTrigger(%this, %trigger, %leave) {
   if(!%leave.isIn(Knights)) {
      return;
   }
   KnightEvents.postEvent(KnightLeaveSection, %leave SPC %trigger);
}
