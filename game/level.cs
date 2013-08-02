new ScriptObject(Level1);

function block(%pos, %size) {
   TheLevel.add(new ConvexShape() {
      material = BlankWhite;
      position = %pos;
      rotation = "1 0 0 0";
      scale = %size;

      surface = "0 0 0 1 0 0 0.5";
      surface = "0 1 0 0 0 0 -0.5";
      surface = "0.707107 0 0 0.707107 0 0.5 0";
      surface = "0 0.707107 -0.707107 0 0 -0.5 0";
      surface = "0.5 0.5 -0.5 0.5 -0.5 0 0";
      surface = "0.5 -0.5 0.5 0.5 0.5 0 0";
   });
}

function Level1::onStart() {
   GameGroup.add(new SimGroup(TheLevel) {
      new LevelInfo(TheLevelInfo) {
         canvasClearColor = "0 0 0";
      };
      new GroundPlane(TheGround) {
         position = "0 0 0";
         material = BlankWhite;
      };
      new Sun(TheSun) {
         azimuth = 230;
         elevation = 45;
         color = "1 1 1";
         ambient = "0.1 0.1 0.1";
         castShadows = false;
      };
   });

   // Create four protagonists!
   knight(Juliet, "-2 2 0", Fighter);
   knight(Kilo, "2 2 0", Shooter);
   knight(Hotel, "-2 -2 0", Shooter);
   knight(November, "2 -2 0", Healer);

   // Create a bunch of enemies.
   soldier("0 0 0");
}

//-----------------------------------------------------------------------------
// A material to give the ground some colour (even if it's just white).
singleton Material(BlankWhite) {
   diffuseColor[0] = "1 1 1";
};
