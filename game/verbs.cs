new ScriptObject(Verbs);

function Verbs::define(%this, %key, %verb) {
   Verbs.map.bindCmd(keyboard, %key, "Verbs." @ %verb @ "();", "");
}

function Verbs::endVerb() {
   Verbs.map.pop();
   Knights.selectMap.push();
}

function Verbs::onStart(%this) {
   GameGroup.add(Verbs);

   // Respond to keypresses.
   %this.map = new ActionMap();

   // Add some verbs that allow the knights to perform actions.
   %this.define(",", "And");
   %this.define(".", "Test");
}

function Verbs::onEnd(%this) {
   %this.map.delete();
}

function Verbs::and(%this) {
   // Give the user the chance to select another knight.
   %this.endVerb();
}

function Verbs::test(%this) {
   // Do something with the selected knights.
   echo("testing");
   foreach(%knight in Knights.selected) {
      echo("   " @ %knight);
   }
   // Deselect all knights.
   Knights.deselectAll();
   // Start selection process again.
   %this.endVerb();
}
