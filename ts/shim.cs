function SimSet::contains(%this, %obj) {
   return %this.getObjectIndex(%obj) != -1;
}

function SimObject::isIn(%this, %set) {
   return %set.contains(%this);
}

function SimSet::size(%this) {
   return %this.getCount();
}

function SimSet::last(%this) {
   return %this.size() ? %this.getObject(%this.size() - 1) : "";
}

function SimSet::first(%this) {
   return %this.size() ? %this.getObject(0) : "";
}

function include(%path) {
   %prefix = $modulePath !$= "" ? $modulePath @ "/" : "";
   exec(%prefix @ %path @ "/main.cs");
}

function SimObject::can(%this, %method) {
   return %this.isMethod(%method);
}
