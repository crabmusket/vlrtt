new ScriptObject(Verbs) {
   // Allows us to use the onEvent callback.
   class = StateMachine;

   // The null state is only necessary to we have an enterReady callback when
   // the state machine is first 'activated' (i.e. given a ready event).
   state = null;
   transition[null, ready] = ready;

   // Top-level commands. Usually involves selecting a knight.
   transition[ready, knightSelected] = selected;

   // This is where most of the verbs live - after selecting a character.
   transition[selected, and] = ready;
   transition[selected, test] = test;
   transition[selected, heal] = healTarget;
   transition[selected, attack] = attackTarget;
   transition[selected, stop] = stop;
   transition[selected, move] = moveForwards;
   transition[selected, retreat] = retreat;

   // Must target someone for these verbs.
   transition[healTarget, knightTargeted] = heal;
   transition[attackTarget, enemyTargeted] = attack;

   // Catch these events from every state and return to ready.
   transition["*", finish] = ready;
   transition["*", cancel] = ready;
};

function Verbs::define(%this, %key, %verb) {
   Verbs.map.bindCmd(keyboard, %key, "Verbs.onEvent(" @ %verb @ ");", "");
}

function Verbs::onStart(%this) {
   GameGroup.add(Verbs);

   // Respond to keypresses.
   %this.map = new ActionMap();
   %this.globalMap = new ActionMap();

   // Add some verbs that allow the knights to perform actions.
   %this.define(",", "And");
   %this.define(".", "Test");
   %this.define("h", "Heal");
   %this.define("a", "Attack");
   %this.define("s", "Stop");
   %this.define("shift m", "Move");
   %this.define("shift r", "Retreat");

   %this.globalMap.bindCmd(keyboard, "ctrl c", "Verbs.onEvent(cancel);");
   %this.globalMap.push();

   // Start up the state machine.
   %this.onEvent(ready);
}

function Verbs::onEnd(%this) {
   %this.map.delete();
   %this.globalMap.delete();
}

//-----------------------------------------------------------------------------

datablock ShapeBaseImageData(Selectron) {
   shapeFile = "./selectron.dae";
   offset = "0 0 0.25";
};

//-----------------------------------------------------------------------------

function Verbs::onFinish(%this) {
   Knights.deselectAll();
}
function Verbs::onCancel(%this) {
   Knights.deselectAll();
}

function Verbs::endVerb(%this) {
   %this.onEvent(finish);
}

//-----------------------------------------------------------------------------
// Event scripts

function Verbs::enterReady(%this) {
   if(!Knights.selected.size()) {
      BottomPrintText.setText("");
   }
   Knights.beginSelect();
}
function Verbs::leaveReady(%this) {
   Knights.endSelect();
}

function Verbs::enterSelected(%this) {
   if(Knights.selected.size() == Knights.size()) {
      BottomPrintText.addText(" Everyone,", true);
   } else {
      BottomPrintText.addText(" " @ Knights.selected.last().name @ ",", true);
   }
   %this.map.push();
}
function Verbs::leaveSelected(%this) {
   %this.map.pop();
}

//-----------------------------------------------------------------------------

function Verbs::enterTest(%this) {
   // Do something with the selected knights.
   echo("testing");
   foreach(%knight in Knights.selected) {
      echo("   " @ %knight);
   }
   // Start selection process again.
   %this.endVerb();
}

//-----------------------------------------------------------------------------

function Verbs::enterHealTarget(%this) {
   Knights.beginTarget();
}
function Verbs::leaveHealTarget(%this) {
   Knights.endTarget();
}

function Verbs::enterHeal(%this) {
   foreach(%knight in Knights.selected) {
      %knight.setMoveDestination(%this.target.getPosition());
   }
   %this.target = "";
   %this.endVerb();
}

//-----------------------------------------------------------------------------

function Verbs::enterAttackTarget(%this) {
   Enemies.beginTarget();
}
function Verbs::leaveAttackTarget(%this) {
   Enemies.endTarget();
}

function Verbs::enterAttack(%this) {
   foreach(%knight in Knights.selected) {
      %knight.getDataBlock().attack(%knight, %this.target);
   }
   %this.target = "";
   %this.endVerb();
}

//-----------------------------------------------------------------------------

function Verbs::enterStop(%this) {
   foreach(%knight in Knights.selected) {
      %knight.setImageTrigger(0, 0);
      %knight.stop();
      %knight.setAimLocation($forwards);
      %knight.schedule(200, clearAim);
   }
   %this.endVerb();
}

//-----------------------------------------------------------------------------

function Verbs::enterMoveForwards(%this) {
   foreach(%knight in Knights.selected) {
      %knight.setMoveDestination($forwards);
   }
   %this.endVerb();
}

function Verbs::enterRetreat(%this) {
   foreach(%knight in Knights.selected) {
      %knight.setMoveDestination(VectorScale($forwards, -1));
   }
   %this.endVerb();
}