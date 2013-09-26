new SimSet(AI);

exec("./soldier.cs");

function AI::brain(%this, %obj, %template) {
   %templateObj = %template @ BrainTemplate;
   eval("%obj.brain = new ScriptObject(\"\" : " @ %templateObj @ ");");
   %obj.brain.superclass = StateMachine;
   %obj.brain.class = %template @ Brain;
   %obj.brain.owner = %obj;
   %obj.brain.state = null;
   %obj.brain.transition[null, ready] = ready;
   %obj.brain.onEvent(ready);
}

function Enemy::onKnightEnterSection(%this, %obj, %data) {
   %knight = getWord(%data, 0);
   %trigger = getWord(%data, 1);
   if(%trigger.getGroup() == %obj.getGroup()) {
      %obj.brain.onEvent(playerNear);
   }
}

datablock TriggerData(SectionTrigger) {};

function SectionTrigger::onEnterTrigger(%this, %trigger, %enter) {
   if(!%enter.isIn(Knights)) {
      return;
   }
   KnightEvents.postEvent(KnightEnterSection, %enter SPC %trigger);
}

function SectionTrigger::onLeaveTrigger(%this, %trigger, %leave) {
   if(!Knights.contains(%leave)) {
      return;
   }
   KnightEvents.postEvent(KnightLeaveSection, %leave SPC %trigger);
}
