new ScriptObject(Cover);

datablock MissionMarkerData(CoverPointData) {
   shapeFile = "./projectile.dae";
};

function Cover::point(%this, %pos) {
   return new WayPoint() {
      datablock = CoverPointData;
      position = %pos;
   };
}

