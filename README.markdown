# vlrtt

A real-time tactical action game with vim-like controls.

## Controls

You control your squad by pressing letters to form commands.
A command usually takes the form of three keystrokes - selecting a character, a verb, and a target.
At any time you can press `ctrl c` to cancel your current order.
And you can always quit the game by pressing `esc`.

### Selection

Your four protagonists are named Juliet, Kilo, Hotel and Lionel.
To select one, press the key coresponding to the first letter of their name.
You can select multiple characters by typing a comma and then another letter.
You can select all your characters by typing a lowercase `a`.

### Verbs

Once you've selected the characters you want to command, you can give them an order:

 * `a` tells them to attack an enemy. Some characters have ranged attacks, others melee.
 * `c` means take cover at a specific location
 * `M` means move forwards (they'll just run towards the end of the level)
 * `R` means retreat (they'll run towards the beginning of the level)
 * `h` means help or heal

Note that if a command takes a capital letter, you actually need to hold down `shift` to issue it (caps lock doesn't work!).

### Targeting

Some verbs, like attacking and healing, require a target.
If this happens, after you press the key for a verb, valid targets will be assigned to letter keys.
The closest target is usually `j`, then `k` and so on.
The letters will appear on-screen overlaid on top of the entities they go with.
