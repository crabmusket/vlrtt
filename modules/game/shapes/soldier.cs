singleton TSShapeConstructor(SoldierDae)
{
   baseShape = "./soldier.dae";
};

function SoldierDae::onLoad(%this)
{
   %this.addSequence("ambient", "stand_root", "0", "2", "1", "0");
   %this.addSequence("ambient", "hide_root", "3", "4", "1", "0");
   %this.addSequence("ambient", "down_root", "5", "6", "1", "0");
}
