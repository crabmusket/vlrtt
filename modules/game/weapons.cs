$i = -1;
datablock ShapeBaseImageData(RangedWeapon) {
   shapeFile = "./shapes/projectile.dae";
   offset = "0 0 3";
   projectileData = Bullet;

   stateName[$i++] = "ready";
   stateTransitionOnTriggerDown[$i] = "load";

   stateName[$i++] = "load";
   stateTransitionOnTriggerUp[$i] = "ready";
   stateTransitionOnTimeout[$i] = "fire";
   stateTimeoutValue[$i] = 1;

   stateName[$i++] = "fire";
   stateFire[$i] = true;
   stateScript[$i] = onFire;
   stateTransitionOnTriggerUp[$i] = "ready";
   stateTransitionOnTimeout[$i] = "ready";
   stateTimeoutValue[$i] = 0;
};

datablock ProjectileData(Bullet) {
   projectileShapeName = "./shapes/projectile.dae";
   isBallistic = false;
   speed = 10;
   damage = 20;
};

function Bullet::onCollision(%this, %obj, %col) {
   %col.damage(%this.damage);
}

singleton Material(BulletMaterial) {
   diffuseColor[0] = "1 1 1";
   mapTo = "baseProjectile";
};

function RangedWeapon::onFire(%this, %obj) {
   %db = %this.projectileData;
   %p = new Projectile() {
      datablock = %db;
      initialVelocity = VectorScale(%obj.getEyeVector(), %db.speed);
      initialPosition = VectorAdd(%obj.getPosition(), "0 0" SPC $CharacterHeight);
      sourceObject = %obj;
      sourceSlot = 0;
   };
   GameGroup.add(%p);
}

//-----------------------------------------------------------------------------

datablock ShapeBaseImageData(Wand) {
   shapeFile = "./shapes/projectile.dae";
   offset = "0 0 2";
   projectileData = Bullet;
};

function Wand::cast(%this, %obj, %target) {
   %db = %this.projectileData;
   %p = new Projectile() {
      datablock = %db;
      initialVelocity = "0 0 -5";
      initialPosition = VectorAdd(%target.getPosition(), "0 0" SPC 5);
      sourceObject = %obj;
      sourceSlot = 0;
   };
   GameGroup.add(%p);
}
