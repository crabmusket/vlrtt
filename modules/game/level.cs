new ScriptObject(Level) {
   sections = "walls towers";
   sectionSize = 30;
   sectionHeight = 20;
   forwards = "0 1000000 0";
   backwards = "0 0 0";
};

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
   %this.forwards = 0 SPC (%length - 1) * %this.sectionSize SPC 0;

   // Create level sections.
   for(%i = 0; %i < %length; %i++) {
      // Create section content.
      %sectionFunction = getWord(%this.sections, %i % getWordCount(%this.sections)) @ "Section";
      %section = %this.call(%sectionFunction, 2, 0, 0);

      // Create side walls.
      %width = 6;
      %height = getRandom(1, %i / %length * %this.sectionHeight);
      %section.add(block(
         -(%this.sectionSize + %width) / 2 SPC 0                 SPC %height / 2,
         %width                            SPC %this.sectionSize SPC %height));

      // Translate the blocks and add them to the game hierarchy.
      %section.callOnChildren(displace, 0 SPC (%i+1) * %this.sectionSize SPC 0);
      TheLevel.add(%section);
   }

   // Add navmesh for entire level.
   TheLevel.add(new NavMesh(Nav) {
      position = 0 SPC (%length + 0) / 2 * %this.sectionSize SPC 0;
      scale = %this.sectionSize / 20 SPC (%length + 1) * %this.sectionSize / 20 SPC 10;
      alwaysRender = true;
   });
   Nav.build(false);

   // Create four protagonists!
   knight(Juliet, "-6 -4 0", Fighter);
   knight(Kilo, "-2 0 0", Shooter);
   knight(Hotel, "2 0 0", Shooter);
   knight(November, "6 -4 0", Healer);
}

function block(%pos, %size) {
   %maxX =  getWord(%size, 0) / 2;
   %maxY =  getWord(%size, 1) / 2;
   %maxZ =  getWord(%size, 2) / 2;
   %minX = -getWord(%size, 0) / 2;
   %minY = -getWord(%size, 1) / 2;
   %minZ = -getWord(%size, 2) / 2;
   return new ConvexShape() {
      material = BlankWhite;
      position = %pos;
      rotation = "1 0 0 0";
      scale = "1 1 1";

      surface = "0 0 0 1 0 0 " @ %maxZ;
      surface = "0 1 0 0 0 0 " @ %minZ;
      surface = "0.707107 0 0 0.707107 0 " @  %maxY @ " 0";
      surface = "0 0.707107 -0.707107 0 0 " @ %minY @ " 0";
      surface = "0.5 0.5 -0.5 0.5 " @ %minX @ " 0 0";
      surface = "0.5 -0.5 0.5 0.5 " @ %maxX @ " 0 0";
   };
}

function Level::blankSection(%this, %soldiers, %deltas, %tanks) {
   return new SimGroup();
}

function Level::wallsSection(%this, %soldiers, %deltas, %tanks) {
   %gridDivs = 5;
   %numCoverPoints = 8;
   %g = new SimGroup();

   // Divide the section into a grid.
   %s = %this.sectionSize / 2;
   %d = %s / %gridDivs;
   %innerSpots = "";
   %backSpots = "";
   for(%i = -%s + %d; %i <= %s - %d; %i += %d) {
      for(%j = -%s + %d; %j <= %s - %d; %j += %d) {
         %pos = %i SPC %j SPC 0.5;
         if(%j == %s - %d) {
            %backSpots = %backSpots TAB %pos;
         } else {
            %innerSpots = %innerSpots TAB %pos;
         }
      }
   }
   %innerSpots = std.shuffle(%innerSpots, Field);
   %backSpots = std.shuffle(%backSpots, Field);

   // Cover goes at random points.
   for(%i = 0; %i < %numCoverPoints; %i++) {
      %g.add(block(getField(%innerSpots, %i), %d SPC 1 SPC 2));
   }

   // Enemies go along the back wall.
   for(%i = 0; %i < %soldiers; %i++) {
      %g.add(soldier(getField(%backSpots, %i)));
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
   %trans = getWords(3, 6, %this.getTransform());
   %this.setTransform(VectorAdd(%delta, %this.getPosition()) SPC %trans);
}

//-----------------------------------------------------------------------------
// A material to give the ground some colour (even if it's just white).
singleton Material(BlankWhite) {
   diffuseColor[0] = "1 1 1";
};
