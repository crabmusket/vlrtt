new ScriptObject(Convex);

function Convex::block(%this, %pos, %size) {
   %maxX =  getWord(%size, 0) / 2; %minX = -%maxX;
   %maxY =  getWord(%size, 1) / 2; %minY = -%maxY;
   %maxZ =  getWord(%size, 2);     %minZ = 0;
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

function Convex::ramp(%this, %pos, %size, %rot) {
   %maxX =  getWord(%size, 0) / 2; %minX = -%maxX;
   %maxY =  getWord(%size, 1) / 2; %minY = -%maxY;
   %maxZ =  getWord(%size, 2) / 2; %minZ = 0;
   %angle = mRadToDeg(mAtan(%maxZ, %maxY));
   return new ConvexShape() {
      material = BlankWhite;
      position = %pos;
      rotation = "1 0 0 0";
      scale = "1 1 1";

      surface = mEulerToQuat("180 0" SPC %angle) @ " 0 0 " @ %maxZ + 0.01;
      surface = "0 1 0 0 0 0 " @ %minZ;
      surface = "0.707107 0 0 0.707107 0 " @  %maxY @ " 0";
      surface = "0 0.707107 -0.707107 0 0 " @ %minY @ " 0";
      surface = "0.5 0.5 -0.5 0.5 " @ %minX @ " 0 0";
      surface = "0.5 -0.5 0.5 0.5 " @ %maxX @ " 0 0";
   };
}

function mEulerToQuat(%euler) {
   %r = mDegToRad(getWord(%euler, 1));
   %p = mDegToRad(getWord(%euler, 0));
   %y = mDegToRad(getWord(%euler, 2));
   %q0 = mCos(%r/2) * mCos(%p/2) * mCos(%y/2) + mSin(%r/2) * mSin(%p/2) * mSin(%y/2);
   %q1 = mSin(%r/2) * mCos(%p/2) * mCos(%y/2) - mCos(%r/2) * mSin(%p/2) * mSin(%y/2);
   %q2 = mCos(%r/2) * mSin(%p/2) * mCos(%y/2) + mSin(%r/2) * mCos(%p/2) * mSin(%y/2);
   %q3 = mCos(%r/2) * mCos(%p/2) * mSin(%y/2) - mSin(%r/2) * mSin(%p/2) * mCos(%y/2);
   return %q0 SPC %q1 SPC %q2 SPC %q3;
}
