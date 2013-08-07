function SimSet::contains(%this, %obj) {
   return %this.getObjectIndex(%obj) != -1;
}

function SimSet::size(%this) {
   return %this.getCount();
}

function include(%path) {
   exec(%path @ "/main.cs");
}

function SimObject::can(%this, %method) {
   return %this.isMethod(%method);
}
