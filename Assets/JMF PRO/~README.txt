

====================================

JEWEL MATCH FRAMEWORK - by kurayami88

====================================


---------------------------------------------------
~ WARNING ~ WARNING ~ WARNING ~ WARNING ~ WARNING ~
---------------------------------------------------

LeanTween Newbie - Please ensure that you follow LeanTween's instructions in their ReadMe.
                   Otherwise, just put the given "Plugins" folder ( in LeanTween folder ) into
                   your main Asset folder.

JEWEL MATCH FRAMEWORK uses 2D Tookit and LeanTween. 
Though the logic system is independant of the two, all GUI and visuals are hardcoded to it.
You can easily swap out LeanTween by replacing LeanTween's coding for other Tweening script codes.

---------------------------------------------------
~ WARNING ~ WARNING ~ WARNING ~ WARNING ~ WARNING ~
---------------------------------------------------


TABLE OF CONTENTS

1. Contact Me
2. Contributors
3. Dependancies
     a. GameManager Script
4. Drag & Drop Global Variables explanation
     a. Game Manager
     b. Board Layout
     c. Winning Conditions
     d. Custom Animations
     e. Audio Player
     f. Visualize Grid
5. Customisable Scripts
6. Sample Scenes



------------------
1. Contact Me
------------------

If you feel like dropping me a buzz; need to ask questions in regards to this package; Bug report; etc...

Email :- KuraStudios@gmail.com
Facebook :- https://www.facebook.com/KuraStudios
Unity Forum Page :- http://forum.unity3d.com/threads/198177-Jewel-Match-Framework


------------------
2. Contributors
------------------

"Tauz" - forum ID name. AKA "Sebastian". Thank you for the Board Visualizer script :)
"Ville Seppanen" - Gems artwork source ( further modified & customized by kurayami88 )


------------------
3. Dependancies
------------------

3 a. GameManager Script
 
GameManager script component can be attached to any object (or an empty object).
It is the main control script that will control all other related scripts.

Just use the provided tk2dCamera prefab for a quick start! 
(not encouraged *AT ALL!* to add the 'Game Manager' script manually from scratch)

Side Note : Adding the GameManager script will add all other further dependency scripts. 
You do not have to add any other scripts manually.



--------------------------------------------
4. Drag & Drop Global Variables explanation
--------------------------------------------

4 a. Game Manger

Using Pool Manager - enable pooling of game objects ( requires Pool Manager 5 asset ! )
Board Width (int) -  the number of boxes left to right of the board
Board Height (int) -  the number of boxes up to down of the board
Size - the size of an individual board square in pixels. ( can only be a square )
Padding Percentage - the amount of padding on each board box for each piece within it so that there is a gap between each box.
Show Corners - for the on scene board visualizer to show the corners of the current board setup.
Show Grid - for the on scene board visualizer to show the grids of the current board setup.
Show Padded Tile - for the on scene board visualizer to show the padded area of the current board setup.
Show Tool Tips - for the on scene board visualizer to show text tips for the board visualizer.
Pixels Per Meter - a 2D toolkit measurement. Make sure this value is synchronize across all your 2D toolkit stuffs.
Score Txt Object - reference of a text prefab so that the score is visible.
Moves Txt Object - reference of a text prefab so that the moves counter is visible.
Combo Txt Object - reference of a text prefab so that the combo counter is visible.
Num of Active Type - the number of basic gems in this level. Currently up to max 9 types.
Eliminate Pre-Start Match - if enabled, tries to start the game without any pre-made matches due to randomization.
Piece Drop Extra Effect - if enabled, will simulate a "easeOutBack" tweening effect when the pieces stop moving.
Move Only After Settle - The player can only make the next move after the board has finished the previous move.
Move Resets Combo - When the player moves, the current combo will reset to '0'.
Delayed Gravity - toggle to enable/disable the delayed gravity when pieces drop into empty spaces.
Gravity Delay Time (float) - the delay before the piece will drop in seconds
Accelerated Velocity - pieces will fall faster if it has longer distance to travel ( more empty spaces to move through )
Display Score HUD - toggle to enable/disable the score HUD when a match is performed and destroyed
Score HUD - the score HUD prefab which contains a text field


(float value in seconds)
Pieces Drop Speed (float) - the time it takes for the gem to drop down 1 box space.
Board Refresh Speed (float) - the time it takes before each match check sequence across the entire board.
Game Update Speed (float) - the time it takes before the next update sequence
      - for the pieces to be notified of empty space and drop.
      - for detection of possible moves and suggestion.
      - to refresh the text fields > visual GUI.
No More Moves Reset Time (float) - delay in time before the board resets due to no more moves available
Suggestion Timer (float) - delay in time after the board stops moving before suggesting the next move
Gem Switch Speed (float) - the time it takes for two gems to switch positions.

Current Gravity - supports four directions (UP, DOWN, LEFT, RIGHT)
Default Back Panel - the default back panel object you wish to use

Piece Manager - the reference to the PiecesManager object
Panel Manager - the reference to the PanelsManager object

on How the PanelsManager / PiecesManager works, please view this instoduction video to JMF Pro
http://youtu.be/oPxuurE6Udg


%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%


4 b. Board Layout



Launch Window - launches the board layout window for easier board customisation.

Show / Hide Panel (x) - Shows or Hides the related Panels

Panel 1
-------
Panel edit visuals - edit the visuals for you to easier visualize the board in Board Layout script.
                     Does not affect how the game looks like.
                     *** panel type automatically syncs with the panels you defined in GameManager.


Panel 2
-------
Max generated panels - The maximum generate panels of the specific type during "Randomize"
                       *** panel type automatically syncs with the panels you defined in GameManager.

Notes Panel
-------
shows notes on how to use the Board Layout


Random on Start - tells the board to randomize on game-start following Panel 2 criteria.
                  It will ignore the preset board layout.

Board Buttons - Will correspond the BoardWidth & BoardHeight (in GameManager). Clicking on the buttons
                will cycle through each PanelType enum (located in BoardPanel script).
Board numbers - is the panel's strength indicator. Will do nothing for empty and basic panels.

Reset - will reset the entire board layout to basic panel type.
Click All - will simulate 1 cycle throughout the entire board buttons.
Randomize! - will randomize the board layout based on "Panel 2" criteria.
Reset Pieces Only! - resets the defined pieces in the board layout.



%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

4 c. Winning Conditions


Time Label - The associated prefab used to label the time remaining.
Time Text - The associated prefab used to display the time remaining.
Moves Label - The associated prefab used to label the moves remaining.
Moves Text - The associated prefab used to display the moves remaining.
Check Speed - The check speed on the winning conditions (whether Game Over or not) in seconds.

Special The Leftovers - converts remaining time and moves to specials.
Seconds per special (float) - specify how many seconds to convert to obtain a special piece.
Moves per special (int) - specify how many moves to convert to obtain a special piece.

Pop Specials Before End - the feature to trigger all remaining special pieces before the game officially ends.

Is Timer/Score/Max-Moves/Clear-Shaded Game - the objective of the current game. Can select multiple.


Is Get Types Game - the objective of the game is to get/match the required amount of a certain type of piece.
Num to Get - the criteria of how many pieces to get before you win. Follows Normal Pieces array listing order. 

Is Treasure Game - the objective of the game is to get all the treasure piece to the defined location.
Treasure label / text - the text object used to display the treasure game information
Num of Treasures - the amount of treasures user needs to collect in the game
Max on Screen - the max amount of treasures spawned at a single given time.
Chance to Spawn - the chance to spawn a treasure piece in percentage ( % ) . range from 0 - 30 %
Treasure Goal - the board locations of which the treasure pieces needs to get to in order to win the game.

GameOverMessage - The associated prefab used to display the Game Over message.



%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%


4 d. Custom Animations

Piece Destroy Effect - any Particle effect prefab which will be used to display some visuals when a piece is destroyed.
No More Moves - the prefab to display "No more moves!" when the board is reseting.

Horizontal Anim - the animation played when a match-4 Horizontal-type power is triggered
Vertical Anim - the animation played when a match-4 Vertical-type power is triggered
Star Anim - the animation played when a T-Match power is triggered
Rainbow Anim - the animation played when a match-5 power is triggered
Bomb Anim - the animation played when a match-6 power is triggered

Lock/Rock/Ice/Shaded Anim - the anims to be played when the specified panels got hit.

Converting Anim - the animation played when converting a special piece via 'Special the LeftOvers' in "winning conditions" script.



%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

4 e. Audio Player


Player - Reference to the gameObject which has the "AudioSource" component.
Enable music - enable/disable music (not dynamic - toggle does not effect while playing)
Bgm - the audioclip for your preffered music
Enable Sound FX - enable/disable sound fx (dynamic)

Game Over Sound Fx - the sound made when the round is over.
Switch Sound Fx - the sound made when a player makes a switch (even during a bad move)
Drop Sound Fx - the sound made when a piece finally stops moving after gravity.
Match Sound Fx - the sound made when a match is found and destroyed.
Special Match Sound Fx - the sound made when special merge occurs ( two power piece merge).
Bad Move Sound Fx - the sound made when a bad move is made by the player.
Arrow Sound Fx - the sound made when the horizontal/vertical/"special h/v/t" piece are triggered.
Star Sound Fx - the sound made when a star piece is triggered. ( match-T piece )
Rainbow Sound Fx - the sound made when a rainbow is triggered. ( match-5 piece )

Lock/Rock/Ice/Shaded Panel hit Fx - the sound effect played when the specified panels got hit.

Converting Special Fx - the sound effect played when converting a special piece 
                        via 'Special the LeftOvers' in "winning conditions" script.



%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

4 f. Visualize Grid


*** this script is required to display the grid on the scene while editing.
Gm - The gameManagerPanel object reference. Please drag the panel here if the default was changed.



-----------------------
5. Customisable Scripts
-----------------------

The scripts are located in "Jewel Match Framework/Scripts" folder. All are written in C#.


Area 51 - Scripts in here are off-limits unless you know what you are doing. ENTER AT OWN RISK


Customisables - Scripts that would better enhance your own style of gaming. Tips and default codes
                are already in place within the scripts. Refer to them for customisation.



------------------
6. Sample Scenes
------------------


located in the main-folder/Scenes/

Contains :-
Scene 0 - Main Menu
Scene 1 - 3 Gems 9x9
Scene 2 - 4 Gems 5x5
Scene 3 - 6 Gems 9x9
Scene 4 - 5 Gems 9x9 Random
Scene 5 - 4 Gems 9x9 Custom
Scene 6 - 5 gems 9x9 treasure

Newbie to Unity NOTE: add your scenes in "File > build settings > .." OR "CTRL-SHIFT-B"





For more info, or help, read the provided FAQ

==============
    END
==============








