new ScriptObject(Cover) {
   protagonistPoints = new SimSet();
   enemyPoints = new SimSet();
};

function Cover::onStart(%this) {
   %this.targetMap = new ActionMap();
   %this.letters = "jkhnfdgvcbtyursieowpxqlazm";
   for(%i = 0; %i < strLen(%this.letters); %i++) {
      %action = getSubStr(%this.letters, %i, 1);
      %this.targetMap.bindCmd(keyboard, %action, "Cover.target(" @ %i @ ");");
   }
}

function Cover::onEnd(%this) {
   %this.targetMap.delete();
}

function Cover::beginTarget(%this) {
   %this.targetMap.push();
   %i = 0;
   %this.protagonistPoints.sort(distanceFromKnights);
   foreach(%point in %this.protagonistPoints) {
      %point.setShapeName(" "  @ getSubStr(%this.letters, %i, 1) @ " ");
      %i++;
      if(%i == strLen(%this.letters)) break;
   }
}

function Cover::endTarget(%this) {
   %this.targetMap.pop();
   foreach(%point in %this.protagonistPoints) {
      %point.setShapeName("");
   }
}

function Cover::target(%this, %index) {
   if(%index < %this.protagonistPoints.size()) {
      Verbs.target = %this.protagonistPoints.getObject(%index);
      Verbs.onEvent(coverTargeted);
   }
}

datablock StaticShapeData(CoverPointData) {
   shapeFile = "./shapes/projectile.dae";
};

function Cover::point(%this, %pos, %team) {
   if(%team $= "") {
      %team = protagonist;
   }
   %p = new StaticShape() {
      datablock = CoverPointData;
      position = %pos;
   };
   %this.getFieldValue(%team @ Points).add(%p);
   return %p;
}
