new ScriptObject(Level);

new Material(TerrainMaterial) {
   mapTo = baseTerrain;
   diffuseColor[0] = "1 1 1";
};

function Level::onStart(%this) {
   // Set up basic objects.
   GameGroup.add(new SimGroup(TheLevel) {
      new LevelInfo(TheLevelInfo) {
         canvasClearColor = "0 0 0";
      };
      new TSStatic(TheGround) {
         shapeName = "./terrains/dips.dae";
         collisionType = "Visible Mesh";
         position = "0 0 0";
         allowPlayerStep = true;
      };
      new Sun(TheSun) {
         azimuth = 230;
         elevation = 45;
         color = "1 1 1";
         ambient = "0.5 0.5 0.5";
         castShadows = false;
      };
   });

   // Add navmesh for entire level.
   TheLevel.add(new NavMesh(Nav) {
      position = "0 0 0";
      scale = "5 5 10";
      alwaysRender = true;
      cellSize = 0.1;
      tileSize = 10;
   });
   Nav.build(false);

   // Create four protagonists!
   knight(Hotel,  "0 -4 10", Mage);
   knight(Juliet, "6 0 10",  Fighter);
   knight(Kilo,   "0 0 10",  Fighter);
   knight(Lionel, "-6 0 10", Fighter);
}

function SceneObject::displace(%this, %delta) {
   %trans = getWords(3, 6, %this.getTransform());
   %this.setTransform(VectorAdd(%delta, %this.getPosition()) SPC %trans);
}

//-----------------------------------------------------------------------------
// A material to give the ground some colour (even if it's just white).
singleton Material(BlankWhite) {
   diffuseColor[0] = "1 1 1";
};
