function SimSet::contains(%this, %obj) {
   return %this.getObjectIndex(%obj) != -1;
}

function include(%path) {
   exec(%path @ "/main.cs");
}
