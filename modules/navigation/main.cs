function AIPlayer::setPathDestination(%obj, %dest) {
   %path = new NavPath() {
      from = %obj.getPosition();
      to = %dest;
      mesh = Nav;
   };
   if(%path.getCount() > 0) {
      %obj.followNavPath(%path);
      return true;
   } else {
      %path.delete();
      return false;
   }
}

function AIPlayer::followNavPath(%obj, %path) {
   %obj._navigationPathNode = 0;
   %obj._navigationPath = %path;
   %obj.moveToNextNavNode();
}

function AIPlayer::onReachDestination(%obj) {
   echo(%obj.name SPC "reached");
   if(isObject(%obj._navigationPath)) {
      %obj.moveToNextNavNode();
   }
}

function AIPlayer::moveToNextNavNode(%obj) {
   %len = %obj._navigationPath.getCount() - 1;
   if(%obj._navigationPathNode < %len) {
      %obj._navigationPathNode++;
      %dest = %obj._navigationPath.getNode(%obj._navigationPathNode);
      %slowdown = %obj._navigationPathNode == %len;
      echo(%obj.getName() SPC "moving to" SPC %obj._navigationPathNode @ ":" SPC %dest);
      %obj.setMoveDestination(%dest, %slowdown);
   } else {
      %obj.stop();
      %obj._navigationPath.delete();
      %obj._navigationPath = "";
   }
}

function PlayerData::onRemove(%this, %obj) {
   if(isObject(%obj._navigationPath)) {
      %obj._navigationPath.delete();
   }
}
