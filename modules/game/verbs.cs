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
   transition[selected, cover] = coverTarget;

   // Must target someone for these verbs.
   transition[healTarget, knightTargeted] = heal;
   transition[attackTarget, enemyTargeted] = attack;
   transition[coverTarget, coverTargeted] = cover;

   // Catch these events from every state and return to ready.
   transition[_, finish] = ready;
   transition[_, cancel] = ready;
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
   %this.define("c", "Cover");
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
   BottomPrintText.event = BottomPrintText.schedule(1000, setText, "");
   Knights.deselectAll();
}
function Verbs::onCancel(%this) {
   BottomPrintText.setText("");
   Knights.deselectAll();
}

function Verbs::endVerb(%this) {
   %this.onEvent(finish);
}

//-----------------------------------------------------------------------------
// Event scripts

function Verbs::onReady(%this) {
   BottomPrintText.setText("   ");
}
function Verbs::enterReady(%this) {
   Knights.beginSelect();
}
function Verbs::leaveReady(%this) {
   Knights.endSelect();
}

function Verbs::enterSelected(%this) {
   if(BottomPrintText.event) {
      cancel(BottomPrintText.event);
      BottomPrintText.event = "";
      BottomPrintText.setText("   ");
   }
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
   BottomPrintText.addText(" heal", true);
   Knights.beginTarget();
}
function Verbs::leaveHealTarget(%this) {
   Knights.endTarget();
}

function Verbs::enterHeal(%this) {
   BottomPrintText.addText(" " @ %this.target.name, true);
   foreach(%knight in Knights.selected) {
      %knight.getDataBlock().heal(%knight, %this.target);
   }
   %this.target = "";
   %this.endVerb();
}

//-----------------------------------------------------------------------------

function Verbs::enterAttackTarget(%this) {
   BottomPrintText.addText(" attack", true);
   Enemies.beginTarget();
}
function Verbs::leaveAttackTarget(%this) {
   Enemies.endTarget();
}

function Verbs::enterAttack(%this) {
   BottomPrintText.addText(" the enemy", true);
   foreach(%knight in Knights.selected) {
      %knight.getDataBlock().attack(%knight, %this.target);
   }
   %this.target = "";
   %this.endVerb();
}

//-----------------------------------------------------------------------------

function Verbs::enterCoverTarget(%this) {
   BottomPrintText.addText(" take cove", true);
   Cover.beginTarget();
}
function Verbs::leaveCoverTarget(%this) {
   Cover.endTarget();
}

function Verbs::enterCover(%this) {
   BottomPrintText.addText(" there!", true);
   foreach(%knight in Knights.selected) {
      %knight.getDataBlock().takeCover(%knight, %this.target);
   }
   %this.target = "";
   %this.endVerb();
}

//-----------------------------------------------------------------------------

function Verbs::enterStop(%this) {
   BottomPrintText.addText(" stop!", true);
   foreach(%knight in Knights.selected) {
      %knight.getDataBlock().stopAll(%knight);
   }
   %this.endVerb();
}

//-----------------------------------------------------------------------------

function Verbs::enterMoveForwards(%this) {
   BottomPrintText.addText(" move up!", true);
   foreach(%knight in Knights.selected) {
      %knight.getDataBlock().goTo(%knight, Level.getForwards(%knight.getPosition()));
   }
   %this.endVerb();
}

function Verbs::enterRetreat(%this) {
   BottomPrintText.addText(" retreat!", true);
   foreach(%knight in Knights.selected) {
      %knight.getDataBlock().goTo(%knight, Level.getBackwards(%knight.getPosition()));
   }
   %this.endVerb();
}
