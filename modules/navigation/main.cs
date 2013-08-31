function AIPlayer::setPathDestination(%obj, %dest) {
   %obj.clearPathDestination();
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

function AIPlayer::clearPathDestination(%obj) {
   if(isObject(%obj._navigationPath)) {
      %obj._navigationPath.delete();
      %obj._navigationPath = "";
   }
   %obj.stop();
}

function AIPlayer::followNavPath(%obj, %path) {
   %obj._navigationPathNode = 0;
   %obj._navigationPath = %path;
   %obj.moveToNextNavNode();
}

function PlayerData::onReachDestination(%this, %obj) {
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
      %obj.setMoveDestination(%dest, %slowdown);
   } else {
      %obj.clearPathDestination();
   }
}

function PlayerData::onRemove(%this, %obj) {
   if(isObject(%obj._navigationPath)) {
      %obj._navigationPath.delete();
   }
}
