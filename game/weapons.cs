$i = -1;
datablock ShapeBaseImageData(RangedWeapon) {
   shapeFile = "./projectile.dae";
   offset = "0 0 3";

   stateName[$i++] = "ready";
   stateTransitionTriggerDown[$i] = "load";

   stateName[$i++] = "load";
   stateTransitionOnTimeout[$i] = "fire";
   stateTimeoutValue[$i] = 1;

   stateName[$i++] = "fire";
   stateFire[$i] = true;
   stateScript[$i] = onFire;
   stateTransitionOnTimeout[$i] = "ready";
   stateTimeoutValue[$i] = 1;
};

datablock ProjectileData(Bullet) {
   shapeFile = "./projectile.dae";
};

singleton Material(BulletMaterial) {
   diffuseColor[0] = "1 1 1";
   mapTo = "baseProjectile";
};

function RangedWeapon::onFire(%this, %obj) {
   error("firing");
   %p = new Projectile() {
      datablock = Bullet;
      initialVelocity = VectorScale(%obj.getForwardVector(), 10);
      sourceObject = %obj;
   };
   GameGroup.add(%p);
}

//-----------------------------------------------------------------------------

datablock ShapeBaseImageData(MeleeWeapon) {
   shapeFile = "./projectile.dae";
   offset = "0 0 2.5";
};

function MeleeWeapon::onFire(%this, %obj) {
}

//-----------------------------------------------------------------------------

datablock ShapeBaseImageData(HealWeapon) {
   shapeFile = "./projectile.dae";
   offset = "0 0 2";
};

function HealWeapon::onFire(%this, %obj) {
}
