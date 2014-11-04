using UnityEngine;
using System.Collections;

/// <summary>
/// JMF Relay static class. 
/// WARNING~! Do not call JMFRelay.onXXX(); explicitly... it is not meant to be called!
/// *** already called by fixed coding positions in GameManager. ***
/// </summary>


public static class JMFRelay {

	static GameManager gm {get{return JMFUtils.gm;}} // getter methods for gameManager reference
	static WinningConditions wc {get{return JMFUtils.wc;}} // getter methods for WinningConditions reference


	public static void onPreGameStart (){ // called before GameManager does anything... NOTHING IS SET UP YET
		// -----------------------------------
		// your own stuff here...
		//
		// WARNING : board HAS NOT been set up yet... you can do final board modifications here... 
		// stuff like abilities that modify the current GameManager set up before board inits
		// e.g., board size, board width/height etc...
		// -----------------------------------
	}

	// when the board has been finalized, and are being initiated ( GUI output of pieces and panels )
	public static void onGameStart (){
		// init the board objects
		for(int x = 0; x < gm.boardWidth; x++){
			for(int y = 0; y < gm.boardHeight; y++){
				gm.board[x,y].init();
			}
		}
		// -----------------------------------
		// your own stuff here...
		// -----------------------------------
	}

	public static void onPlayerMove() { // called when player makes a move
		// custom piece / panels onPlayerMove function call
		for (int x = 0; x < gm.boardWidth ; x++){
			for (int y = 0; y < gm.boardHeight ; y++) {
				if(gm.board[x,y].isFilled){
					gm.board[x,y].piece.pd.onPlayerMove(gm.board[x,y]);
				}
				gm.board[x,y].panel.pnd.onPlayerMove(gm.board[x,y].panel);
			}
		}

		// -----------------------------------
		// your own stuff here...
		// -----------------------------------
	}

	// called when all pieces stop moving and suggestion is being calculated
	public static void onBoardStabilize (){
		onComboEnd(); // end the combo when board stabilizes

		// custom piece / panels onBoardStabilize function call
		for (int x = 0; x < gm.boardWidth ; x++){
			for (int y = 0; y < gm.boardHeight ; y++) {
				if(gm.board[x,y].isFilled){
					gm.board[x,y].piece.pd.onBoardStabilize(gm.board[x,y]);
				}
				gm.board[x,y].panel.pnd.onBoardStabilize(gm.board[x,y].panel);
			}
		}

		// -----------------------------------
		// your own stuff here...
		// -----------------------------------
	}

	public static void onCombo(){ // called directly after combo+1, but before GUI output 
		// -----------------------------------
		// your own stuff here...
		// -----------------------------------
	}
	public static void onComboEnd(){
		gm.currentCombo = 0; // reset combo counter...

		// -----------------------------------
		// your own stuff here...
		// -----------------------------------
	}

	public static void onNoMoreMoves(){ // called before board reset happens
		// -----------------------------------
		// your own stuff here...
		// -----------------------------------
	}

	public static void onBoardReset(){ // called after board reset happens
		// -----------------------------------
		// your own stuff here...
		// -----------------------------------
	}

	public static void onPieceClick(int x, int y){
		// -----------------------------------
		// your own stuff here...
		// x / y is the board position of which the piece located was clicked.
		// e.g., JMFUtils.gm.board[x,y] ....
		// -----------------------------------
	}

	// the "RAW" score given for destroyed pieces / matches of an individual box
	// the score HAS NOT been multiplied by combo bonus yet~!
	public static int onScoreIssue(int scoreGain){
		int modifiedGains = scoreGain;
		// -----------------------------------
		// your own stuff here...
		// -----------------------------------
		// modifiedGains = something else?? ;

		return modifiedGains;
	}
}
