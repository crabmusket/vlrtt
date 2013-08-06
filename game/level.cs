new ScriptObject(Level);

Level.sections = "blank towers";
Level.sectionSize = 30;
Level.sectionHeight = 20;

function Level::onStart(%this) {
   // Set up basic objects.
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

   // Random level generation parameters.
   %length = getRandom(5, 15);

   // Create level sections.
   for(%i = 0; %i < %length; %i++) {
      // Create section content.
      %sectionFunction = getWord(%this.sections, %i % getWordCount(%this.sections)) @ "Section";
      %section = %this.call(%sectionFunction, 2, 0, 0);

      // Create side walls.
      %height = getRandom(1, %i / %length * %this.sectionHeight);
      %width = 6;
      %section.add(block(-%this.sectionSize - %width/2 SPC "0 0", %width SPC %this.sectionSize SPC %height));

      // Translate the blocks and add them to the game hierarchy.
      %section.callOnChildren(displace, 0 SPC %i * %this.sectionSize SPC 0);
      TheLevel.add(%section);
   }

   // Create four protagonists!
   knight(Juliet, "-2 2 0", Fighter);
   knight(Kilo, "2 2 0", Shooter);
   knight(Hotel, "-2 -2 0", Shooter);
   knight(November, "2 -2 0", Healer);
}

function block(%pos, %size) {
   return new ConvexShape() {
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
   };
}

function Level::blankSection(%this, %soldiers, %delas, %tanks) {
   %g = new SimGroup();
   %s = %this.sectionSize;
   for(%i = 0; %i < %soldiers; %i++) {
      %pos = getRandom(-%s, %s) SPC getRandom(-%s, %s) SPC 0;
      %soldier = soldier(%pos);
      %g.add(%soldier);
   }
   return %g;
}

function Level::towersSection(%this, %soldiers, %deltas, %tanks) {
   %g = new SimGroup();
   %g.add(block("-5 0 2", "4 4 4"));
   %g.add(block( "5 0 2", "4 4 4"));
   %g.add(soldier("-5 0 4.5"));
   %g.add(soldier( "5 0 4.5"));
   return %g;
}

function SceneObject::displace(%this, %delta) {
   %this.setTransform(VectorAdd(%delta, %this.getPosition()) SPC "0 0 0 1");
}

//-----------------------------------------------------------------------------
// A material to give the ground some colour (even if it's just white).
singleton Material(BlankWhite) {
   diffuseColor[0] = "1 1 1";
};
