AutoClickerHeroes
=================

An autoclicker designed to work with the flash game ClickerHeroes

This Autoclicker is intended for use with the game at ClickerHeroes.com. 
It was hackishly thrown together solely with the intent of autoclicking. 
It can certainly use some refactoring/reorganizing, as well as new hot features!.

Now, I have added other things, such as:

	* Every 30 minutes, it "presses" the keys: a, 8, 7, 9, 1, 2, 3, 4, 5, 6 (I hope to make this order configurable)
		* This is under the idea that in at least 30 minutes, my game hits an ancient that it fails to pass without using skills.
	* Every minute, it clicks on the "Hero Point," configurable with button called "Set Hero Click"
		* The idea here is to level up a chosen hero every minute
		* The code initializes it to a point (267, 592) which is the location on my screen for Treebeast
	* It can record one macro of clicks, which it will preserve in a file (on the desktop) and execute with F8.
		* I use this to record a macro that saves my game, so I can easily save my game without interrupting the click combo
	
Hoykeys/Controls:
	*F6 starts the clicking **where the mouse is at that point**, and any additional "checked" timers (i.e. the checkboxes at the bottom)
	*Shift+F6 starts the clicking WITHOUT locking the mouse, so the click position follows the cursor.
	*F7 stops the clicking
	*F8 plays back the recorded list of alternate points -- this is the macro I use to save my game
	
	(The rest are somewhat obsolete)
	*Shift+F8 executes the "next" click in the recorded macro (it loops around to the beginning)
	*f9 increments the index within the macro list, to skip over recorded points
	*Shift+F9 decrements the index in the macro list