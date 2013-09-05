new ScriptObject(Level) {
   sections = "walls";
   sectionSize = 30;
   sectionHeight = 20;
   forwards = "0 1000000 0";
   backwards = "0 0 0";
};

include(convex);

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
         ambient = "1 1 1";
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
      %section.add(Convex.block(
         -(%this.sectionSize + %width) / 2 SPC 0                 SPC 0,
         %width                            SPC %this.sectionSize SPC %height));

      // Translate the section to its actual position.
      %center = 0 SPC (%i+1) * %this.sectionSize SPC 0;
      %section.callOnChildren(displace, %center);

      // Create AI trigger.
      $Trigger::renderTriggers = true;
      %w = %this.sectionSize;
      %section.add(new Trigger() {
         datablock = EnemyAITrigger;
         position = %center;
         polyhedron =
            -%w/2 SPC %w/2 SPC 0 SPC  // Corner point
            %w SPC 0 SPC 0 SPC   // X axis
            0 SPC -%w SPC 0 SPC  // Y axis
            0 SPC 0 SPC 10;      // Z axis
      });

      // Add section to the game hierarchy.
      TheLevel.add(%section);
   }

   // Add navmesh for entire level.
   TheLevel.add(new NavMesh(Nav) {
      position = 0 SPC (%length + 0) / 2 * %this.sectionSize SPC 0;
      scale = %this.sectionSize / 20 SPC (%length + 1) * %this.sectionSize / 20 SPC 10;
   });
   Nav.build(false);

   // Create four protagonists!
   knight(Juliet, "-6 0 0", Shooter);
   knight(Kilo, "0 0 0", Fighter);
   knight(Hotel, "6 0 0", Shooter);
   knight(November, "0 -4 0", Healer);
}

// METATORQUESCRIPT aaghhghghhahhgllhahghlah
foreach$(%w in "forwards backwards") {
   eval(
"function Level::get" @ %w @ "(%this, %from) {"                       @
   "if(%from $= \"\") {"                                              @
      "%from = \"0 0 0\";"                                            @
   "}"                                                                @
   "return getWord(%from, 0) SPC getWord(%this." @ %w @ ", 1) SPC 0;" @
"}"
   );
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
   %spots = "";
   for(%i = -%s + %d; %i <= %s - %d; %i += %d) {
      for(%j = -%s + %d; %j <= %s - %d; %j += %d) {
         %pos = %i SPC %j SPC 0;
         %spots = %spots TAB %pos;
      }
   }
   %spots = std.shuffle(%spots, Field);

   // Cover goes at random points.
   for(%i = 0; %i < %numCoverPoints; %i++) {
      %g.add(Convex.block(getField(%spots, %i), %d SPC 1 SPC 1.3));
      %g.add(Cover.point(VectorAdd("0 -1 0.5", getField(%spots, %i))));
      %g.add(Cover.point(VectorAdd("0 1 0.5", getField(%spots, %i)), enemy));
   }
   %spots = std.drop(%spots, %numCoverPoints, Field);

   // Enemies go in spots that weren't used for cover.
   for(%i = 0; %i < %soldiers; %i++) {
      %g.add(soldier(getField(%spots, %i)));
   }
   %spots = std.drop(%spots, %soldiers, Field);

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
