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

   // Must target a knight before enterHeal callback is fired.
   transition[healTarget, knightTargeted] = heal;

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

   // Add some verbs that allow the knights to perform actions.
   %this.define(",", "And");
   %this.define(".", "Test");
   %this.define("h", "Heal");
   %this.define("backspace", "Cancel");

   // Start up the state machine.
   %this.onEvent(ready);
}

function Verbs::onEnd(%this) {
   %this.map.delete();
}

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
   Knights.selectMap.push();
}
function Verbs::leaveReady(%this) {
   Knights.selectMap.pop();
}

function Verbs::enterSelected(%this) {
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
   Knights.targetMap.push();
}
function Verbs::leaveHealTarget(%this) {
   Knights.targetMap.pop();
}

function Verbs::enterHeal(%this) {
   foreach(%knight in Knights.selected) {
      %knight.setMoveDestination(%this.target.getPosition());
   }
   %this.target = "";
   %this.endVerb();
}
