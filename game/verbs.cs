new ScriptObject(Verbs);

function verb(%key, %verb) {
   VerbMap.bindCmd(keyboard, %key, "Verbs::" @ %verb @ "();", "");
}

function endVerb() {
   VerbMap.pop();
   Knights.selectMap.push();
}

function Verbs::onStart(%this) {
   GameGroup.add(Verbs);

   // Respond to keypresses.
   new ActionMap(VerbMap);

   // Add some verbs that allow the knights to perform actions.
   verb(",", "And");
   verb(".", "Test");
}

function Verbs::onEnd(%this) {
   VerbMap.delete();
}

function Verbs::and(%this) {
   // Give the user the chance to select another knight.
   endVerb();
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
   endVerb();
}
