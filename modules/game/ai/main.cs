new SimSet(AI);

foreach$(%type in $EnemyTypes) {
   exec("./" @ %type @ ".cs");
}

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
