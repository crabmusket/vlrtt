function SimSet::contains(%this, %obj) {
   return %this.getObjectIndex(%obj) != -1;
}

function SimSet::size(%this) {
   return %this.getCount();
}

function include(%path) {
   %prefix = $modulePath !$= "" ? $modulePath @ "/" : "";
   exec(%prefix @ %path @ "/main.cs");
}

function SimObject::can(%this, %method) {
   return %this.isMethod(%method);
}
