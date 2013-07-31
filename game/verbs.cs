new ScriptObject(Verbs) {
   class = StateMachine;

   state = null;
   transition[null, ready] = ready;

   transition[ready, knightSelected] = selected;

   transition[selected, and] = ready;
   transition[selected, test] = test;

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

function Verbs::enterTest(%this) {
   // Do something with the selected knights.
   echo("testing");
   foreach(%knight in Knights.selected) {
      echo("   " @ %knight);
   }
   // Start selection process again.
   %this.endVerb();
}
