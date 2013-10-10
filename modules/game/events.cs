function eventQueue(%namespace) {
   %manager = %namespace @ Events;
   %m = new EventManager(%manager) {
      queue = %manager;
   };
}

function event(%namespace, %action, %class) {
   %class = %class $= "" ? AIPlayer : %class;
   %manager = %namespace @ Events;
   %event = %namespace @ %action;
   eval(
%manager @ ".registerEvent(" @ %event @ ");" @
"function " @ %class @ "::on" @ %event @ "(%this, %data) {" @
   "if(%this.getDataBlock().can(on" @ %event @ ")) {" @
      "%this.getDataBlock().on" @ %event @ "(%this, %data);" @
   "}" @
"}"
   );
}

function postEvent(%namespace, %action, %data) {
   %manager = %namespace @ Events;
   %event = %namespace @ %action;
   %manager.postEvent(%event, %data);
}

//-----------------------------------------------------------------------------
// Events related to all characters.

eventQueue(Character);
event(Character, Death);

//-----------------------------------------------------------------------------
// Events related to kights' actions.

eventQueue(Knight);
event(Knight, EnterSection);
event(Knight, LeaveSection);
