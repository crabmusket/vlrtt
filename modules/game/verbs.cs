exec("./verbHelp.gui");

new ScriptObject(Verbs) {
   // Allows us to use the onEvent callback.
   class = StateMachine;

   // The null state is only necessary to we have an enterReady callback when
   // the state machine is first 'activated' (i.e. given a ready event).
   state = null;
   transition[null, ready] = ready;

   // Catch these events from every state and return to ready.
   transition[_, finish] = ready;
   transition[_, cancel] = ready;

   // Verbs are your only transitions out of the ready state.
   transition[ready, attack] = attackTarget;
   transition[ready, stop] = stop;
   transition[ready, move] = selectDirection;
   transition[ready, cover] = coverTarget;

   // Must target someone for these verbs.
   transition[attackTarget, enemyTargeted] = attack;
   transition[coverTarget, coverTargeted] = cover;

   // Location targeting.
   transition[selectDirection, directionSelected] = move;
};

// Utility function for defining verbs.
function Verbs::define(%this, %key, %verb) {
   %this.map.bindCmd(keyboard, %key, "Verbs.onEvent(" @ %verb @ ");", "");
   %this.helpText[%verb] = %key @ "   " @ %verb @ "\n";
}

function Verbs::onStart(%this) {
   GameGroup.add(Verbs);
   PlayGui.add(VerbHelpDlg);

   // Respond to keypresses.
   %this.map = new ActionMap();
   %this.globalMap = new ActionMap();
   %this.directionMap = new ActionMap();

   // Add some verbs that allow the knights to perform actions.
   %this.define("a", "Attack");
   %this.define("s", "Stop");
   %this.define("c", "Cover");
   %this.define("m", "Move");

   // Keyboard actions that should be available in any state.
   %this.globalMap.bindCmd(keyboard, "ctrl c", "Verbs.onEvent(cancel);");
   %this.globalMap.bindCmd(keyboard, "?",      "Verbs.toggleHelp();");
   %this.globalMap.push();

   // Direction selection actions.
   foreach$(%d in "w e d c s z a q") {
      %this.directionMap.bindCmd(keyboard, %d,
         "Verbs.direction ="@%d@"; Verbs.onEvent(directionSelected);");
   }
   %this.helpText[directionSelected] =
      "w      Away\n"     @
      "a      Left\n"     @
      "s      Towards\n"  @
      "d      Right\n\n"  @
      "qezc   Others";

   %this.helpText[coverTargeted] =
   %this.helpText[enemyTargeted] =
      "j   Closest\n"      @
      "k   Next closest\n" @
      "    etc...\n";

   // Start up the state machine.
   %this.onEvent(ready);
}

function Verbs::onEnd(%this) {
   %this.map.delete();
   %this.globalMap.delete();
   %this.directionMap.delete();
}

//-----------------------------------------------------------------------------

function Verbs::onEvent(%this, %event) {
   // Call the regular parent function first, so that the state has changed
   // before we try to get the help text for it.
   Parent::onEvent(%this, %event);
   %this.updateHelpDlg();
}

function Verbs::updateHelpDlg(%this) {
   VerbHelpText.setText("");

   // We're going to iterate over every dynamic field on the state machine. These
   // fields include all the state transitions, the ones we're interested in.
   // What we want to find is whether, for each transition from our current state,
   // there is help text for that transition. If there is, we add it to the help
   // gui.
   %len = %this.getDynamicFieldCount();
   for(%i = 0; %i < %len; %i++) {
      // Dynamic fields are structured as "key" TAB "value".
      %field = %this.getDynamicField(%i);
      %value = getField(%field, 1);
      %field = getField(%field, 0);

      // We only care about transition fields, which will have the form
      // "transition" @ "state" @ "_" @ "event".
      if(startsWith(%field, "transition")) {
         // Chop the "transition" off.
         %field = getSubStr(%field, 10);
         // Split on tabs!
         %field = strReplace(%field, _, "\t");
         // Now we can easily extract the state and event using fields.
         %state = getField(%field, 0);
         %event = getField(%field, 1);

         // Only consider transitions out of our current state.
         if(%state !$= "" && %event !$= "") {
            if(%state $= %this.state) {
               VerbHelpText.addText(%this.helpText[%event], true);
            }
         }
      }
   }

   // Special case: you can always cancel.
   if(%this.state !$= ready) {
      VerbHelpText.addText("\nctrl c   Cancel\n", true);
   }

   // Resize the dialog so it looks nice.
   VerbHelpDlg.extent =
      getWord(VerbHelpDlg.extent, 0) SPC
      14 * getRecordCount(VerbHelpText.getText());
}

function Verbs::toggleHelp(%this) {
   // If there's a currently-running tweening effect, we need to kill it before
   // going back in the other direction.
   if(VerbHelpDlg.tween) {
      VerbHelpDlg.tween.delete();
   }

   // Toggle deployed state which tells us which direction to tween in.
   VerbHelpDlg.deployed = !VerbHelpDlg.deployed;

   // And start the tween!
   VerbHelpDlg.tween = Tweens.toOnce(200, VerbHelpDlg,
      VerbHelpDlg.deployed
         ? "position:   20 252"
         : "position: -200 252");
}

function Verbs::endVerb(%this) { %this.onEvent(finish); }

//-----------------------------------------------------------------------------
// Event scripts

function Verbs::enterReady(%this) { %this.map.push(); }
function Verbs::leaveReady(%this) { %this.map.pop(); }

function Verbs::enterAttackTarget(%this) { Enemies.beginTarget(); }
function Verbs::leaveAttackTarget(%this) { Enemies.endTarget(); }

function Verbs::enterAttack(%this) {
   Knight.getDataBlock().attack(Knight, %this.target);
   %this.target = "";
   %this.endVerb();
}

function Verbs::enterCoverTarget(%this) { Cover.beginTarget(); }
function Verbs::leaveCoverTarget(%this) { Cover.endTarget(); }

function Verbs::enterCover(%this) {
   Knight.getDataBlock().takeCover(Knight, %this.target);
   %this.target = "";
   %this.endVerb();
}

function Verbs::enterStop(%this) {
   Knight.getDataBlock().stopAll(Knight);
   %this.endVerb();
}

function Verbs::enterSelectDirection(%this) { %this.directionMap.push(); }
function Verbs::leaveSelectDirection(%this) { %this.directionMap.pop(); }

function Verbs::enterMove(%this) {
   // Get the movement direction from the letter. Basically, combine the different
   // cardinal directions in different ways to construct 8 possible movement
   // vectors.  We don't worry about normalising thenvector here...
   switch$(%this.direction) {
      case w: %dir = TheCamera.getForwardVector();
      case a: %dir = VectorScale(TheCamera.getRightVector(), -1);
      case s: %dir = VectorScale(TheCamera.getForwardVector(), -1);
      case d: %dir = TheCamera.getRightVector();

      case q: %dir = VectorAdd(VectorScale(TheCamera.getRightVector(), -1), TheCamera.getForwardVector());
      case e: %dir = VectorAdd(TheCamera.getRightVector(), TheCamera.getForwardVector());
      case c: %dir = VectorAdd(TheCamera.getRightVector(), VectorScale(TheCamera.getForwardVector(), -1));
      case z: %dir = VectorScale(VectorAdd(TheCamera.getRightVector(), TheCamera.getForwardVector()), -1);
   }

   // ...because it's normalised here, at the same time as becoming horizontal.
   %dir = VectorNormalize(getWords(%dir, 0, 1) SPC 0);

   // Construct a new position to move to by colliding a ray with the edge of the
   // arena circle. So glad I decided to go with the circular arena!
   %pos = rayCircle(Knight.getPosition(), %dir, 50);
   // Bring the position in from the edge of the circle a bit.
   %pos = VectorSub(%pos, VectorScale(%dir, 2));
   Knight.goTo(%pos, true, 0.5);
   %this.endVerb();
}

function rayCircle(%pos, %ray, %radius) {
   // Ray/circle intersection: http://stackoverflow.com/a/1549997/945863
   // Assume circle is centered at 0, 0, 1.
   %pos = getWords(%pos, 0, 1) SPC 1;
   %Dx = getWord(%pos, 0);
   %Dy = getWord(%pos, 1);
   %a = mPow(VectorLen(%ray), 2);
   %b = 2 * %Dx * getword(%ray, 0) + 2 * %Dy * getWord(%ray, 1);
   %c = mPow(%Dx, 2) + mPow(%Dy, 2) - mPow(%radius, 2);
   %sol = mSolveQuadratic(%a, %b, %c);
   // The positive solution is the one we want.
   %x = getMax(getWord(%sol, 1), getWord(%sol, 2));
   return VectorAdd(%pos, VectorScale(%ray, %x));
}
