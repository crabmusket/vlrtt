new ScriptObject(Level);

function Level::onStart(%this) {
   // Set up basic objects.
   GameGroup.add(new SimGroup(TheLevel) {
      new LevelInfo(TheLevelInfo) {
         canvasClearColor = "0 0 0";
      };

      new Sun(TheSun) {
         azimuth = 230;
         elevation = 45;
         color = "1 1 1";
         ambient = "0.5 0.5 0.5";
         castShadows = false;
      };

      new TSStatic(TheGround) {
         shapeName = "./terrains/saddle.dae";
         collisionType = "Visible Mesh";
         position = "0 0 0";
         allowPlayerStep = true;
      };

      new fxShapeReplicator(TheRocks) {
         position = "0 0 0";
         scale = "50 50 10";
         shapeFile = "./terrains/rock.dae";
         outerRadiusX = 50;
         outerRadiusY = 50;
         shapeCount = 100;
         shapeRetries = 1;
         allowOnStatics = true;
         interactions = false;
         shapeRotateMax = "0 0 180";
         seed = getRealTime();
      };

      new fxShapeReplicator(DeadTrees) {
         position = "0 0 0";
         scale = "50 50 10";
         shapeFile = "./shapes/deadpine.dae";
         shapeCount = 30;
         shapeRetries = 1;
         allowOnStatics = true;
         interactions = false;
         shapeRotateMax = "0 0 180";
         zOffset = -0.5;
         seed = getRealTime();
         outerRadiusX = 50;
         outerRadiusY = 50;
         innerRadiusX = 10;
         innerRadiusY = 10;
      };
   });

   // Add navmesh for entire level.
   TheLevel.add(new NavMesh(Nav) {
      position = "0 0 0";
      scale = "5 5 10";
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
// Lots of white materials :P.

singleton Material(BlankWhite) {
   diffuseColor[0] = "1 1 1";
};

singleton Material(TerrainMat) {
   mapTo = baseTerrain;
   diffuseColor[0] = "1 1 1";
};

singleton Material(RockMaterial) {
   mapTo = baseRock;
   diffuseColor[0] = "1 1 1";
   emissive[0] = true;
};

singleton Material(RuinMaterial) {
   mapTo = baseRuin;
   diffuseColor[0] = "1 1 1";
};

singleton Material(TreeMaterial) {
   mapTo = baseTree;
   diffuseColor[0] = "1 1 1";
};
