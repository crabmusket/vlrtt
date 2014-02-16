function StateMachine::onEvent(%this, %event) {
   if(%event $= "event") {
      return;
   }

   // See if there's a transition callback associated with this event.
   %script = "on" @ %event;
   if(%this.isMethod(%script)) {
      %this.call(%script);
   }

   // Figure out the new state to transition to.
   %newState = %this.transition[%this.state, %event];

   // Check for a wildcard transition out of this state.
   if(%newState $= "") {
      %newState = %this.transition[%this.state, _];
   }

   // If it doesn't exist, see if there's a wildcard transition for this event.
   if(%newState $= "") {
      %newState = %this.transition[_, %event];
   }

   // Apply the state change.
   if(%newState !$= "") {
      // Callback for leaving the current state.
      %script = "leave" @ %this.state;
      if(%this.isMethod(%script)) {
         %this.call(%script);
      }

      // Change the state!
      %this.state = %newState;

      // Callback upon entering the new state.
      %script = "enter" @ %this.state;
      if(%this.isMethod(%script)) {
         %this.call(%script);
      }
   }

   return %this;
}
