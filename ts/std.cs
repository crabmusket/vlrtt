new ScriptObject(std);

function std::shuffle(%this, %data, %delim) {
   if(%delim $= "") {
      %delim = Word;
   }

   %getCount = get @ %delim @ Count;
   %get = get @ %delim;
   %set = set @ %delim;

   %len = call(%getCount, %data);
   for(%i = %len - 1; %i > 0; %i--) {
      %j = getRandom(0, %i);
      %saved = call(%get, %data, %j);
      %data = call(%set, %data, %j, call(%get, %data, %i));
      %data = call(%set, %data, %i, %saved);
   }

   return %data;
}

function std::drop(%this, %data, %count, %delim) {
   if(%delim $= "") {
      %delim = Word;
   }

   %getCount = get @ %delim @ Count;
   %get = get @ %delim @ s;

   return call(%get, %data, %count, call(%getCount, %data) - 1);
}

function std::take(%this, %data, %count, %delim) {
   if(%delim $= "") {
      %delim = Word;
   }

   %getCount = get @ %delim @ Count;
   %get = get @ %delim @ s;

   return call(%get, %data, 0, %count - 1);
}

function std::findClosest(%this, %set, %obj, %excl) {
   %pos = %obj.getPosition();
   %closest = "";
   %dist = 100000;
   foreach(%point in %set) {
      if(%point.isAWordIn(%excl)) {
         continue;
      }
      %d = VectorLen(VectorSub(%pos, %point.getPosition()));
      if(%d < %dist) {
         %dist = %d;
         %closest = %point;
      }
   }
   return %closest;
}

function SimObject::isAWordIn(%obj, %string) {
   foreach$(%w in %string) {
      if(%w $= %obj) {
         return true;
      }
   }
   return false;
}
