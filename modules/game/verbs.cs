new ScriptObject(Verbs) {
   // Allows us to use the onEvent callback.
   class = StateMachine;

   // The null state is only necessary to we have an enterReady callback when
   // the state machine is first 'activated' (i.e. given a ready event).
   state = null;
   transition[null, ready] = ready;

   // Verbs are your only transitions out of the ready state.
   transition[ready, attack] = attackTarget;
   transition[ready, stop] = stop;
   transition[ready, move] = selectDirection;
   transition[ready, cover] = coverTarget;

   // Must target someone for these verbs.
   transition[healTarget, knightTargeted] = heal;
   transition[attackTarget, enemyTargeted] = attack;
   transition[coverTarget, coverTargeted] = cover;

   // Location targeting.
   transition[selectDirection, directionSelected] = move;

   // Catch these events from every state and return to ready.
   transition[_, finish] = ready;
   transition[_, cancel] = ready;
};

exec("./verbHelp.gui");

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
   %this.define(",", "And");
   %this.define("h", "Heal");
   %this.define("a", "Attack");
   %this.define("s", "Stop");
   %this.define("c", "Cover");
   %this.define("m", "Move");
   %this.define("r", "Retreat");

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

function Verbs::updateHelpDlg(%this) {
   %len = %this.getDynamicFieldCount();
   VerbHelpText.setText("");

   for(%i = 0; %i < %len; %i++) {
      %field = %this.getDynamicField(%i);
      %value = getField(%field, 1);
      %field = getField(%field, 0);

      if(startsWith(%field, "transition")) {
         %field = getSubStr(%field, 10);
         %field = strReplace(%field, _, "\t");
         %state = getField(%field, 0);
         %event = getField(%field, 1);

         if(%state !$= "" && %event !$= "") {
            if(%state $= %this.state) {
               VerbHelpText.addText(%this.helpText[%event], true);
            }
         }
      }
   }

   if(%this.state !$= ready) {
      VerbHelpText.addText("\nctrl c   Cancel\n", true);
   }

   VerbHelpDlg.extent =
      getWord(VerbHelpDlg.extent, 0) SPC
      14 * getRecordCount(VerbHelpText.getText());
}

function Verbs::onEvent(%this, %event) {
   Parent::onEvent(%this, %event);
   %this.updateHelpDlg();
}

function Verbs::toggleHelp(%this) {
   if(VerbHelpDlg.tween) {
      VerbHelpDlg.tween.delete();
   }
   VerbHelpDlg.deployed = !VerbHelpDlg.deployed;

   VerbHelpDlg.tween = Tweens.toOnce(200, VerbHelpDlg,
      VerbHelpDlg.deployed
         ? "position:   20 252"
         : "position: -200 252");
}

//-----------------------------------------------------------------------------

function Verbs::onFinish(%this) {
}
function Verbs::onCancel(%this) {
}

function Verbs::endVerb(%this) {
   %this.onEvent(finish);
}

//-----------------------------------------------------------------------------
// Event scripts

function Verbs::onReady(%this) {
}
function Verbs::enterReady(%this) {
   %this.map.push();
}
function Verbs::leaveReady(%this) {
   %this.map.pop();
}

//-----------------------------------------------------------------------------

function Verbs::enterAttackTarget(%this) {
   Enemies.beginTarget();
}
function Verbs::leaveAttackTarget(%this) {
   Enemies.endTarget();
}

function Verbs::enterAttack(%this) {
   Knight.getDataBlock().attack(Knight, %this.target);
   %this.target = "";
   %this.endVerb();
}

//-----------------------------------------------------------------------------

function Verbs::enterCoverTarget(%this) {
   Cover.beginTarget();
}
function Verbs::leaveCoverTarget(%this) {
   Cover.endTarget();
}

function Verbs::enterCover(%this) {
   Knight.getDataBlock().takeCover(Knight, %this.target);
   %this.target = "";
   %this.endVerb();
}

//-----------------------------------------------------------------------------

function Verbs::enterStop(%this) {
   Knight.getDataBlock().stopAll(Knight);
   %this.endVerb();
}

//-----------------------------------------------------------------------------

function Verbs::enterSelectDirection(%this) {
   %this.directionMap.push();
}
function Verbs::leaveSelectDirection(%this) {
   %this.directionMap.pop();
}

function Verbs::enterMove(%this) {
   // Get the movement direction from the letter.
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
   // Make it horizontal.
   %dir = VectorNormalize(getWords(%dir, 0, 1) SPC 0);

   // Construct a new position to move to.
   %pos = rayCircle(Knight.getPosition(), %dir, 50);
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
