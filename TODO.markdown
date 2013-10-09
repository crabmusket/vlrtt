# vlrtt TODO

## Gameplay

Need to make some gameplay :P.

### Less shooting

 * Focus on melee combat
 * A melee engagement should be based on stamina/skill
 * High skill makes enemy's stamina decrease faster
 * When a character's stamina is gone, they take a blow
 * Blows do _lots_ of damage
 * After taking a blow a character regains some stamina (adrenaline)

Stamina is on the order of 0-100 for normal combatants.
Skill is on the order of 0-10.
In single combat, loss of stamina per second `sps = 5 * max(1, skill of enemy - skill)`.

For example, fresh opponents of stamina 100 and equally-matched skill will take 10 seconds to come to blows.
In the case that two characters reach zero stamina at the same moment, a random character takes a blow,
or they may both take a light blow.
If one character had a skill of 10 and the other of 0, the combat would be over in 2 seconds,
and the more-skilled character would have lost 10 stamina.

### Mage

 * Healing spell
 * Magic missile that reduces stamina (and health?)
 * Possible: missile takes time to recharge... how long?
 * Possible: missile prevents healing for some time

### Enemies

 * Normal guys with melee combat
 * Big guys with low skill but high stamina

### Final boss

 * Unique spells
 * Unique-ish environment

## Environment

 * Less waist-high-walls because less shooting
 * Arena-based?
 * Build custom arenas in Blender
 * Arrange arenas in random linear order
 * State machine to lay arenas out with rules

## Commands

 * Help/heal: mage will cast healing spell, fighters will do what?
