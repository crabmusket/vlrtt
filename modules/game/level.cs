new ScriptObject(Level) {
   sections = "sBend walls towers";
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

      // Translate the blocks and add them to the game hierarchy.
      %section.callOnChildren(displace, 0 SPC (%i+1) * %this.sectionSize SPC 0);
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
   %backSpots = "";
   for(%i = -%s + %d; %i <= %s - %d; %i += %d) {
      for(%j = -%s + %d; %j <= %s - %d; %j += %d) {
         %pos = %i SPC %j SPC 0;
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
      %g.add(Convex.block(getField(%innerSpots, %i), %d SPC 1 SPC 2));
      %g.add(Cover.point(VectorAdd("0 -1 0.5", getField(%innerSpots, %i))));
   }

   // Enemies go along the back wall.
   for(%i = 0; %i < %soldiers; %i++) {
      %g.add(soldier(getField(%backSpots, %i)));
   }

   return %g;
}

function Level::towersSection(%this, %soldiers, %deltas, %tanks) {
   %g = new SimGroup();
   %g.add(Convex.block("-5 0 0", "4 4 4"));
   %g.add(Convex.block( "5 0 0", "4 4 4"));
   %g.add(soldier("-5 0 4.5"));
   %g.add(soldier( "5 0 4.5"));
   return %g;
}

function Level::sBendSection(%this, %soldiers, %deltas, %tanks) {
   %numCoverPoints = 4;
   %gridDivs = 4;
   %g = new SimGroup();

   // Create big blockers.
   %w = %this.sectionSize / 3;
   %g.add(Convex.block(
      -%w / 2 SPC -%w SPC 0,
      2 * %w  SPC 5   SPC 5));
   %y = getRandom(2, %w);
   %g.add(Convex.block(
      %w / 2 SPC %y SPC 0,
      2 * %w SPC 5  SPC 2));

   // Add cover in channel.
   %dx = %w / %gridDivs;
   %dy = (%y + %w - 5) / %gridDivs;
   %spots = "";
   for(%i = -%w + %dx; %i <= %w - %dx; %i += %dx) {
      for(%j = -%w + 2.5 + %dy; %j <= %y - %dy - 2.5; %j += %dy) {
         %pos = %i SPC %j SPC 0;
         %spots = %spots TAB %pos;
      }
   }

   %spots = std.shuffle(%spots, Field);
   for(%i = 0; %i < %numCoverPoints; %i++) {
      %g.add(Convex.block(getField(%spots, %i), 1 SPC %dy SPC 2));
      echo(%g.last().getPosition());
   }

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
