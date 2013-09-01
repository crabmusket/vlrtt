new SimSet(Cover);

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
   Cover.sort(distanceFromKnights);
   foreach(%point in Cover) {
      %point.setShapeName(" "  @ getSubStr(%this.letters, %i, 1) @ " ");
      %i++;
      if(%i == strLen(%this.letters)) break;
   }
}

function Cover::endTarget(%this) {
   %this.targetMap.pop();
   foreach(%point in Cover) {
      %point.setShapeName("");
   }
}

function Cover::target(%this, %index) {
   if(%index < %this.size()) {
      Verbs.target = %this.getObject(%index);
      Verbs.onEvent(coverTargeted);
   }
}

datablock StaticShapeData(CoverPointData) {
   shapeFile = "./shapes/projectile.dae";
};

function Cover::point(%this, %pos) {
   %p = new StaticShape() {
      datablock = CoverPointData;
      position = %pos;
   };
   Cover.add(%p);
   return %p;
}

