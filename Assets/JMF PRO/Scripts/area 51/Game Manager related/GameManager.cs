using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary> ##################################
/// 
/// NOTICE :
/// This script is the Mother of all script~!
/// Everything that happens during the game will be controlled in this script.
/// (with public references from support scripts too ofcourse.)
/// 
/// DO NOT TOUCH UNLESS REQUIRED
/// 
/// </summary> ##################################



// ---
// global access board checking enums
// ---

public enum Check{ UP, DOWN, LEFT, RIGHT}; // for scenario check of match directions
public enum Gravity{ UP, DOWN, LEFT, RIGHT};

// special pieces
public enum PowerType{ NONE,POWH ,POWV, POWT, POW5, POW6};

[RequireComponent(typeof(CustomAnimations), typeof(BoardLayout), typeof(WinningConditions))]
[RequireComponent(typeof(AudioPlayer), typeof(VisualizedGrid) )]
public class GameManager : MonoBehaviour {
	
	// ===========================
	// GLOBAL VARIABLES
	// ===========================

	public bool usingPoolManager = false;
	public int boardWidth=4;
	public int boardHeight=4;
	public float size = 4; // the size we want the board to be
	[Range(0.0f,100.0f)] public float paddingPercentage = 20f; // the percentage of padding user wants
	[HideInInspector] public float boxPadding = 0; // the padding in each box **updated during "Awake()"
	public bool showCorners = false;
    public bool showGrid = false;
	public bool showPaddedTile = false;
	public bool showToolTips = false;
	public TextMesh scoreTxtObject; // reference to the text score counter
	public TextMesh movesTxtObject; // reference to the text moves counter
	public GameObject comboTxtObject; // reference to the text combo combo
	[Range(1,9)] public int NumOfActiveType = 3; // remember not to exceed the normalPieces array~!
	public bool eliminatePreStartMatch = false;
	public bool pieceDropExtraEffect = true;
	public bool moveOnlyAfterSettle = false; // must the player wait for board to settle before next move?
	public bool movingResetsCombo = true;// player moving will reset the combo?
	public bool delayedGravity = true; // delay before a piece drops when there's an empty space
	public float gravityDelayTime = 0.3f; // the delay in float seconds
	public bool acceleratedVelocity = true; // drop pieces fall faster if it need to cover more distance
	public bool displayScoreHUD = false;
	public GameObject scoreHUD;
	
	public float piecesDropSpeed = 0.25f;
	public float boardRefreshSpeed = 0.2f;
	public float gameUpdateSpeed = 0.2f;
	public float noMoreMoveResetTime = 2f;
	public float suggestionTimer = 5f;
	public float gemSwitchSpeed = 0.2f;
	public Gravity currentGravity = Gravity.DOWN; // initial gravity of the game
	
	// pieces & panels prefabs
	public GameObject defaultBackPanel;
	public GameObject pieceManager;
	public GameObject panelManager;
	[HideInInspector] public PieceDefinition[] pieceTypes;
	[HideInInspector] public PanelDefinition[] panelTypes;
	
	public Board[,] board; // the board array
		
	// scoring stuff
	[HideInInspector] public long score = 0;
	[HideInInspector] public int currentCombo = 0;
	[HideInInspector] public int maxCombo = 0;
	ComboPopUp comboScript;
	[HideInInspector] public int moves = 0;
	[HideInInspector] public bool isGameOver = false;
	[HideInInspector] public int[] matchCount = new int[9];
	
	// suggestion variables
	[HideInInspector] public bool checkedPossibleMove = false;
	[HideInInspector] public bool isCheckingPossibleMoves = false;
	int suggestionID = 0;
	Vector3 pieceOriginalSize;
	Vector3 newSize;
	GameObject suggestedPiece;
	[HideInInspector] public bool canMove = true; // switch to determine if player can make the next move
	
	// other helper scripts
	[HideInInspector] public AudioPlayer audioScript;
	[HideInInspector] public CustomAnimations animScript;
	
	
	// ================================================
	// ENGINE FUNCTIONS
	// ================================================
	
	// >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
	// Misc. functions
	// >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
	
	// start game preparation
	void initializeGame() {

		boxPadding = size*(paddingPercentage/100); // set the padding value

		pieceTypes = pieceManager.GetComponents<PieceDefinition>();
		panelTypes = panelManager.GetComponents<PanelDefinition>();


		// support sub-scripts initialization
		audioScript = GetComponent<AudioPlayer>();
		animScript = GetComponent<CustomAnimations>();
		
		if(comboTxtObject != null){
			comboScript = comboTxtObject.GetComponent<ComboPopUp>(); // find and assign the combo script
		}
		
		// creates a 2D board
		board = new Board[boardWidth,boardHeight];
		
		//
		// loop to create the board with blocks
		//
		
		// for the board width size
		for( int x = 0; x < boardWidth; x++) {
			// for the board height size
			for( int y = 0; y < boardHeight; y++) {
				// create board centralized to the game object in unity
				Vector3 pos = new Vector3( x - (boardWidth/2.0f) + 0.5f, y -(boardHeight/2.0f) + 0.5f, 0);
				board[x,y] = new Board(this, new int[2]{x,y}, pos*size ) ;
				//place a cube here to start with...
				board[x,y].createObject(pieceTypes[0], ranType());
			}
		}
	}
	
	void preGameSetup(){
		// call the board panels preGameSetup...
		GetComponent<BoardLayout>().setupGamePanels();

		// redesign the board until there's no starting match
		if(eliminatePreStartMatch && NumOfActiveType >= 2 ){
			int count = 0;
			for( int x = 0; x < boardWidth; x++) { // iterate through each board block
				for( int y = 0; y < boardHeight; y++) {
					count++;
					if( findPrematches(x,y) ){ // find any match and change its type
						x = 0; y = -1; // restart the loop
					}
					if(count > 9999){ // if cannot solve by this num of tries, break!
						Debug.LogError("failed to eliminate pre-start match...");
						break;
					}
				}
			}
		}

		// call the board piece preGameSetup...
		GetComponent<BoardLayout>().setupGamePieces();
	}
	
	// function to eliminate pre-start matches
	bool findPrematches(int xPos, int yPos) {
		// variables to keep track of the match potentials
		int matchingRows = 0;
		int matchingCols = 0;
		
		if( !(board[xPos,yPos].isFilled && board[xPos,yPos].canBeMatched()) ){
			return false; // no match can be made from here... quit~
		}
		int mType = board[xPos,yPos].piece.slotNum; // identifier of the current block type
		// check rows
		for (int x = (xPos+1) ; x < boardWidth ; x++) { //check the right side of the cube 
			if ( board[x,yPos].canBeMatched() && board[x,yPos].piece.slotNum == mType) {
				matchingRows++; // increase linked counter
			} else {
				break; // exit loop as no more match this side...
			}
		}
		if ( matchingRows > 1 ){ // if a row is matching
			if(! (xPos+2 >= boardWidth || !board[xPos+2,yPos].panel.pnd.hasStartingPiece) ){
				board[xPos+2,yPos].createObject(pieceTypes[0], ranType()); // assign a new type
			} else if(! (xPos+1 >= boardWidth || !board[xPos+1,yPos].panel.pnd.hasStartingPiece) ){
				board[xPos+1,yPos].createObject(pieceTypes[0], ranType()); // assign a new type
			} else {
				board[xPos,yPos].createObject(pieceTypes[0], ranType()); // assign a new type
			}
			return true;
		}
		
		//  check columns
		for (int y = (yPos+1) ; y < boardHeight ; y++) { //check the top side of the cube 
			if ( board[xPos,y].canBeMatched() && board[xPos,y].piece.slotNum == mType) {
				matchingCols++; // increase linked counter
			} else {
				break; // exit loop as no more match this side...
			}
		}
		if ( matchingCols > 1 ) { // if a column is matching
			if(! (yPos+2 >= boardHeight || !board[xPos,yPos+2].panel.pnd.hasStartingPiece) ){
				board[xPos,yPos+2].createObject(pieceTypes[0], ranType()); // assign a new type
			} else if(! (yPos+1 >= boardHeight || !board[xPos,yPos+1].panel.pnd.hasStartingPiece) ){
				board[xPos,yPos+1].createObject(pieceTypes[0], ranType()); // assign a new type
			} else {
				board[xPos,yPos].createObject(pieceTypes[0], ranType()); // assign a new type
			}
			return true;
		}
		
		return false; // piece is ok... move to the next
	}
	
	// to output the score to the text label
	void txtUpdate(){
		if(scoreTxtObject != null){
			scoreTxtObject.text = score.ToString();
		}
		if(movesTxtObject != null){
			movesTxtObject.text = moves.ToString();
		}
	}
	
	// random cubeType generator , just coz the code is too long
	public int ranType() {
		return Random.Range(0,Mathf.Min( NumOfActiveType, pieceTypes[0].skin.Length) );
		// limited by normalpieces types available if numOfActiveType is declared out of bounds
	}
	
	// the gravity check as a function call - to keep the updater() neat
	void gravityCheck(){
		for(int x = 0; x < boardWidth ; x++){
			for( int y = 0; y < boardHeight ; y++) {
				dropPieces(x,y);
			}
		}
	}
	
	// primarily for the suggestion functions... but you can do other stuff when the board change as you like...
	public void notifyBoardHasChanged(){
		checkedPossibleMove = false; // board has changed, will check possible moves again
		
		// clears the suggestion animation if any
		StopCoroutine("suggestPiece");
		if(suggestedPiece != null){
			LeanTween.cancel(suggestedPiece,suggestionID); // cancel the suggestion when done
			suggestedPiece.transform.localScale = pieceOriginalSize; // return back to normal
			suggestedPiece = null;
		}
	}
	
	// increase the combo counter & display to GUI(dont worry, combo is reset elsewhere)
	public void increaseCombo() {
		// increase combo count!
		currentCombo += 1;

		JMFRelay.onCombo();

		// relay to the combo script
		if(currentCombo > 1 && comboScript != null){ // only show if 2 or more combo
			comboScript.StopCoroutine("displayCombo");
			comboScript.StartCoroutine("displayCombo",currentCombo);
		}
		
		if(maxCombo < currentCombo){
			maxCombo = currentCombo; // just to keep track of the max combo
		}
	}
	
	// increase the score counter (for external scripts to update)
	public void increaseScore(int num, int x, int y) {
		num = JMFRelay.onScoreIssue(num);
		if(currentCombo > 1){
			num = (int) (num * (1.5+(currentCombo/10.0)) ); // increase with multiplier from combo
		}
		if(displayScoreHUD && scoreHUD != null){ // display the HUD?
			board[x,y].scoreHUD.display(num);
		}
		score += num; // add to the game score
	}
	
	// helper function - called by matchType class to splash damage to its neighbouring boards
	public void splashFromHere(int x, int y){
		// splash left, right, up, down ... with out-of-bounds check
		if(x-1 >= 0){
			board[x-1,y].SplashDamage();
		}
		if(x+1 < boardWidth){
			board[x+1,y].SplashDamage();
		}
		if(y-1 >= 0){
			board[x,y-1].SplashDamage();
		}
		if(y+1 < boardHeight){
			board[x,y+1].SplashDamage();
		}
	}
	
	// looper for the boardCheck based on the set interval
    IEnumerator boardCheckLooper () {
		while (!isGameOver){  // loop again (infinite) until game over
			yield return new WaitForSeconds(boardRefreshSpeed); // wait for the given intervals
			// then check the board
			boardChecker();
		}
    }
	
	// status update on given intervals
    IEnumerator updater () {
		while (!isGameOver){  // loop again (infinite) until game over
	    	yield return new WaitForSeconds(gameUpdateSpeed); // wait for the given intervals
			gravityCheck(); // for dropping pieces into empty board box
			detectPossibleMoves(); // to make sure the game doesn't get stuck with no more possible moves
			txtUpdate(); // NGUI's widgets update
		}
    }
	
	// destroys the box after a given time so that it looks cooler
	public IEnumerator destroyInTime(int x, int y,float delay, int mScore){
		if( board[x,y].piece != null && !board[x,y].piece.markedForDestroy ){ // ignore those marked for destroy
			// so that the gravity check does not affect the current box...
			board[x,y].isFalling = true;
			
			yield return new WaitForSeconds(delay); // wait for it...
			// enable gravity check so that it can take over to fill the empty box...
			board[x,y].isFalling = false;
			
			if(board[x,y].isFilled && board[x,y].piece.pd.isDestructible){
				increaseScore( mScore, x, y ); // add to the score
			}
			
			board[x,y].destroyBox();
			if(!board[x,y].panel.isDestructible()){ // if the panel is a solid type with no piece to destroy...
				board[x,y].panelHit(); // got hit by power attack~!
			}
		}
	}

	// destroys the box after a given time so that it looks cooler - object being marked for delayed destruction
	public IEnumerator destroyInTimeMarked(int x, int y, float delay, int mScore){
		if(!board[x,y].isFilled){
			board[x,y].isFalling = false;
			yield break;
		}

		// save the piece reference
		GamePiece refPiece = board[x,y].piece;

		// mark the piece as to be destroyed later
		refPiece.markedForDestroy = true;
		refPiece.thisPiece.GetComponent<PieceTracker>().enabled = false; // no longer movable
		
		yield return new WaitForSeconds(delay); // wait for it...
		
		if(refPiece.master.isFilled){
			increaseScore( mScore, refPiece.master.arrayRef[0], refPiece.master.arrayRef[1] ); // add to the score
		}
		
		refPiece.master.destroyMarked();

		if(!refPiece.master.panel.isDestructible()){ // if the panel is a solid type with no piece to destroy...
			refPiece.master.panelHit(); // got hit by power attack~!
		}
	}
	
	// tween the merging piece ( mostly for gui effect only to show something is happening...)
    public IEnumerator mergePieces (int posX1, int posY1, int posX2, int posY2,bool both) {
		// freeze the boxes involved
		board[posX1,posY1].isFalling = true;
		board[posX2,posY2].isFalling = true;
		
		// switch the two pieces around in memory (not visual in GUI yet)
		GamePiece holder = board[posX1,posY1].piece;
		board[posX1,posY1].piece = board[posX2,posY2].piece;
		board[posX2,posY2].piece = holder;
		board[posX1,posY1].piece.master = board[posX1,posY1];
		board[posX2,posY2].piece.master = board[posX2,posY2];
		
		// tween it ( now only visual in GUI)
		if(both){
			board[posX1,posY1].applyTweening(gemSwitchSpeed); // two sided if u want, else disabled
		}
		board[posX2,posY2].applyTweening(gemSwitchSpeed); // one sided tweening

    	yield return new WaitForSeconds(gemSwitchSpeed); // the timer
	
		// un-freeze the boxes involved
		board[posX1,posY1].isFalling = false;
		board[posX2,posY2].isFalling = false;
			
		currentCombo = 0; // reset combo
    }
	
	
	// >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
	// Matcher functions
	// >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
	
	// Matcher - phase 1 : board block checker for potential matches
	void boardChecker () {

		for (int i =  pieceTypes.Length - 1 ; i >= 0 ; i--){ // loop for each special piece + normal match 3
			for( int x = 0; x < boardWidth; x++) { // iterate through each board block
				for( int y = 0; y < boardHeight; y++) {
					if (board[x,y].isFilled && !board[x,y].isFalling && board[x,y].isActive) { // means the board block has a ready cube
						pieceTypes[i].checkPattern(x,y,i); // check pattern based on piece definition
					}
					if ( i == 0){ // finished cycling through each matching criteria
						board[x,y].isActive = false; // turns this block to passive
					}
				}
			}
		}
	}

	// Matcher - phase 2 : perform clean up matches based on external script's decision
	public void validateMatch(int checkNum, int xPos, int yPos, List<Board> linkedCubesX, List<Board> linkedCubesY){
		if(board[xPos,yPos].piece == null){
			linkedCubesX.Clear();
			linkedCubesY.Clear();
			return;
		}

		if(pieceTypes[checkNum].matchConditions(xPos,yPos,linkedCubesX,linkedCubesY)){

			int scorePerCube = pieceTypes[checkNum].scorePerCube;

			audioScript.playSound(PlayFx.MATCHFX); // play the match sound fx
			
			// manage the combo
			increaseCombo();
			
			increaseScore(scorePerCube,xPos,yPos); // give out score for the main reference piece
			// to cause a splash damage for panels that are damaged only by splash
			splashFromHere(xPos,yPos); // splash from the origin board
			
			foreach (Board mBoardX in linkedCubesX) {
				mBoardX.destroyBox(); // destroy the linked boxes too
				// to cause a splash damage for panels that are damaged only by splash
				splashFromHere(mBoardX.arrayRef[0],mBoardX.arrayRef[1]);
				increaseScore(scorePerCube,
				                 mBoardX.arrayRef[0],mBoardX.arrayRef[1]); // give out score
			}
			foreach (Board mBoardY in linkedCubesY) {
				mBoardY.destroyBox(); // destroy the linked boxes too
				// to cause a splash damage for panels that are damaged only by splash
				splashFromHere(mBoardY.arrayRef[0],mBoardY.arrayRef[1]);
				increaseScore(scorePerCube,
				                 mBoardY.arrayRef[0],mBoardY.arrayRef[1]); // give out score
			}
		
			// free the memory just in case? or perhaps not neccesary for auto GC...
			linkedCubesX.Clear();
			linkedCubesY.Clear();
		}
	}
	
	// function to lock a piece from being destroyed with a cooldown timer
	public IEnumerator lockJustCreated(int x, int y, float time){
		// lock the piece so that it isnt destroyed so fast
		GamePiece refPiece = null;
		if(board[x,y].isFilled){
			refPiece = board[x,y].piece;
			refPiece.justCreated = true;
			refPiece.master.isActive = false;
			yield return new WaitForSeconds(time); // wait for it...
			// un-lock the piece again
			refPiece.justCreated = false;
			refPiece.master.isActive = true;
		}
	}
	
	// >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
	// possible moves detector + suggestor  ( DO NOT TOUCH UNLESS NECCESSARY~! )
	// >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
	
	// moves detector phase 1
	void detectPossibleMoves() {
		
		//checks through each board boxes
		if(!checkedPossibleMove && !isCheckingPossibleMoves){
			isCheckingPossibleMoves = true;
			for (int x = 0; x < boardWidth ; x++){
				for (int y = 0; y < boardHeight ; y++) {
					if( board[x,y].isBeingDelayed || board[x,y].isFalling || board[x,y].isActive ) {
						isCheckingPossibleMoves = false;
						return; // do not continue, wait for board to clear and stabilize
					}
				}
			}
			checkedPossibleMove = true; // once we checked, no need to check again until needed

			JMFRelay.onBoardStabilize();

			List<GameObject> suggestedPieces = new List<GameObject>(); // to hold all the possible moves
			
			for (int x = 0; x < boardWidth ; x++){
				for (int y = 0; y < boardHeight ; y++) {
					if( board[x,y].isFilled && board[x,y].panel.isSwitchable() &&
						checkNeighbourMatch(x,y,board[x,y].piece.slotNum) ) {
						// recognize possible moves and save the piece location
						suggestedPieces.Add(board[x,y].piece.thisPiece);
					}
				}
			}
			
			if (suggestedPieces.Count == 0) { // no more possible moves
				StartCoroutine( resetBoard() ); // reset board in co-routine mode for delayed event
			}
			else {
				// suggest a random piece for next possible move to player
				suggestedPiece = ((GameObject) suggestedPieces[Random.Range(0,suggestedPieces.Count)]);
				suggestedPieces.Clear(); // remove stored memory
				pieceOriginalSize = suggestedPiece.transform.localScale; // remember the current size
				newSize = Vector3.Scale(pieceOriginalSize,new Vector3(1.35f,1.35f,1f)); // the scaling size
				StartCoroutine("suggestPiece"); // its a string coroutine so that we can use StopCoroutine!
				isCheckingPossibleMoves = false;
			}

		}
	}
	
	// moves detector sub-routine phase 2-a - check its surroundings
	bool checkNeighbourMatch(int x, int y, int type) {
		// this piece is a power piece that can be merged?
		if ( !(board[x,y].piece.pd is NormalPiece) ) {
			PieceDefinition pType = board[x,y].piece.pd;
			// can the special piece move to merge with it's neighbour?
			if(specialToPosition(x,y+1, pType) || specialToPosition(x,y-1, pType) ||
				specialToPosition(x+1,y, pType) || specialToPosition(x-1,y, pType) ){
				return true; // can special merge 
			}
		}
		if(board[x,y].piece.pd.isSpecial){
			return false; // piece cannot match normally.. return
		}
		
		// check if the piece is moved in all directions
		if( checkThisPosition(x,y+1,type,Check.UP) || checkThisPosition(x,y-1,type,Check.DOWN) ||
			checkThisPosition(x-1,y,type,Check.LEFT) || checkThisPosition(x+1,y,type,Check.RIGHT) ) {
			return true; // it can make a match if this piece moved...
		}
		return false; // if it reaches here, means no match if this piece moved...
	}
	
	// moves detector sub-routine phase 2-b - can this piece move here to special merge?
	public bool specialToPosition(int xPos,int yPos, PieceDefinition pType ){
		
		if(xPos < 0 || xPos >= boardWidth || yPos < 0 || yPos >= boardHeight ) {
			return false; // assumption is out of bounds ... stop this check
		}
		if(!board[xPos,yPos].isFilled || board[xPos,yPos].isFalling ||
		   !board[xPos,yPos].panel.isSwitchable() ){ // the piece cannot move here, quit too~!
			return false;
		}

		PieceDefinition otherPd = board[xPos,yPos].piece.pd;
		if(pType.powerMatched(0,0,0,0,false,pType,otherPd) || otherPd.powerMatched(0,0,0,0,false,otherPd,pType)){
			return true; // is a power piece combo
		}
		return false; // not a power piece combo
	}
	
	// moves detector sub-routine phase 2-c - scenario when this piece is moved in this direction
	bool checkThisPosition(int xPos,int yPos ,int mType, Check dir){
		
		if(xPos < 0 || xPos >= boardWidth || yPos < 0 || yPos >= boardHeight ) {
			return false; // assumption is out of bounds ... stop this check
		}
		if(!board[xPos,yPos].isFilled || !board[xPos,yPos].panel.isSwitchable() ){ // the piece cannot move here, quit too~!
			return false;
		}
		
		// count of possible matching blocks
		int count = 0;
		
		if(dir != Check.RIGHT) {
			for (int x = (xPos-1) ; x >= 0; x--) { //check the left side of the cube 
				if ( board[x,yPos].canBeMatched() && board[x,yPos].piece.slotNum == mType) {
					count++; // increase linked counter
				} else {
					break; // exit loop as no more match this side...
				}
			}
		}
		
		if(dir != Check.LEFT){
			for (int x = (xPos+1) ; x < boardWidth ; x++) { //check the right side of the cube 
				if ( board[x,yPos].canBeMatched() && board[x,yPos].piece.slotNum == mType) {
					count++; // increase linked counter
				} else {
					break; // exit loop as no more match this side...
				}
			}
		}
		if( count > 1) { // there is a matching row...
			return true; // no need to go further as there is already a possible match
		} else {
			count = 0; // reset count for column matching...
		}
		
		if(dir != Check.UP) {
			for (int y = (yPos-1) ; y >= 0; y--) { //check the bottom side of the cube
				if ( board[xPos,y].canBeMatched() && board[xPos,y].piece.slotNum == mType) {
					count++; // increase linked counter
				} else {
					break; // exit loop as no more match this side...
				}
			}
		}
		if(dir != Check.DOWN) {
			for (int y = (yPos+1) ; y < boardHeight ; y++) { //check the top side of the cube 
				if ( board[xPos,y].canBeMatched() && board[xPos,y].piece.slotNum == mType) {
					count++; // increase linked counter
				} else {
					break; // exit loop as no more match this side...
				}
			}
		}
		if( count > 1) { // there is a matching column...
			return true;
		}
		return false; // if it reaches here, means no match in this position...
	}
	
	
	// resets the board due to no more moves
	IEnumerator resetBoard() {
		animScript.doAnim(animType.NOMOREMOVES,0,0);
		JMFRelay.onNoMoreMoves();
		yield return new WaitForSeconds(noMoreMoveResetTime);
		notifyBoardHasChanged(); // reset the board status
		// for the board width size
		for( int x = 0; x < boardWidth; x++) {
			// for the board height size
			for( int y = 0; y < boardHeight; y++) {
				//reset the pieces with a random type..
				board[x,y].reset(pieceTypes[0], ranType());
			}
		}
		JMFRelay.onComboEnd();
		JMFRelay.onBoardReset();
		isCheckingPossibleMoves = false;
	}
	
	// suggest a piece after a given time...
	IEnumerator suggestPiece() {
		yield return new WaitForSeconds(suggestionTimer); // wait till it's time
		if(suggestedPiece != null){
			// animate it for the user
			suggestionID = LeanTween.scale( suggestedPiece, newSize ,1f, new object[]{"loopType",LeanTweenType.pingPong});
		}
	}
	
	// >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
	// Board Piece position Fall by gravity function ( DO NOT TOUCH UNLESS NECCESSARY~! )
	// >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
	
	// (main gravity function)
	public void dropPieces(int x, int y) {
		if( !(x >= 0 && x < boardWidth && y >=0 && y < boardHeight) ){
			return; // index out of bounds... do not continue~!
		}
		if( board[x,y].replacementNeeded()) {
			board[x,y].isBeingDelayed = true; // status to verify that board already active in drop sequence
			StartCoroutine( movePieces(x,y) ); // coroutine that can be delayed
		}
	}
	
	// secondary gravity function as a coroutine for delay ability
	IEnumerator movePieces(int x, int y){
		if(delayedGravity && board[x,y].mustWait){ // if delay is required by GameManager or by board
			yield return new WaitForSeconds(gravityDelayTime); // delay time between each dropped pieces
		}
		board[x,y].mustWait = false; // change status of board to drop other pieces without delay
		board[x,y].isBeingDelayed = false; // reset status once delay is over
		
		Board prevBoard = null;
		Vector3 gravityVector = new Vector3(); // gravity in vector3
		int nextX = x; // the next board x position ; will be updated in the switch case~!
		int nextY = y; // the next board y position ; will be updated in the switch case~!
		
		bool loopMe = true; // criteria to continue the do-while loop for solid blockades
		int i = 1; // the distance of the solid blockade (if any)

		switch(currentGravity){
		case Gravity.DOWN :
			nextY = y-1; // info about the next board block too for immediate update after animation
			gravityVector = new Vector3(0,-size,0); // gravity in vector3
			if(y+1 < boardHeight){
				prevBoard = getPrevBoard(x,y+1); // get the previous board box
				if(prevBoard == null && board[x,y+1].panel.isSolid()) { // the prevBoard is a solid type panel
					do // loop to slide pieces like an avalanche
					{
						loopMe = false; // did a loop cycle, turn the bool off
						
						// initial check if the top is covered with solids
						if(( ( x-i >= 0 && board[x-i,y+i].panel.isSolid() ) || x-i < 0 ) && 
						   ( ( x+i < boardWidth && board[x+i,y+i].panel.isSolid() ) || x+i >= boardWidth)
							&& ( y-1 >=0 && board[x,y-1].replacementNeeded() ) ){
							loopMe = true; // reactivate the loop; we need to check further inside
							i++; // increase the depth of the search
							y = y-1; // change the check to be the bottom box and check instead.
							nextY = y-1; // update the nextY too
						}
						// try getting from the 'left' side of currentGravity
						if(x-1 >= 0 && !board[x-1,y].replacementNeeded() ){
							prevBoard = getPrevBoard(x-1,y+1);
						}
						// if not, try getting from the 'right' side of currentGravity
						if(prevBoard == null && x+1 < boardWidth && !board[x+1,y].replacementNeeded() ){
							prevBoard = getPrevBoard(x+1,y+1);
						}
					}while(prevBoard == null && y-1 >= 0 && loopMe);
				}
			} else{ // if its the end of the board, spawn a new piece instead~
				StartCoroutine(spawnNew(x,y,gravityVector,nextX,nextY));
			}
			break;
		case Gravity.UP :
			nextY = y+1; // info about the next board block too for immediate update after animation
			gravityVector = new Vector3(0,size,0); // gravity in vector3
			if(y-1 >=0){
				prevBoard = getPrevBoard(x,y-1); // get the previous board box
				if(prevBoard == null && board[x,y-1].panel.isSolid()) { // the prevBoard is a solid type panel
					do // loop to slide pieces like an avalanche
					{
						loopMe = false; // did a loop cycle, turn the bool off
						
						// initial check if the bottom is covered with solids
						if(( ( x-i >= 0 && board[x-i,y-i].panel.isSolid() ) || x-i < 0 ) && 
						   ( ( x+i < boardWidth && board[x+i,y-i].panel.isSolid() ) || x+i >= boardWidth)
						   && ( y+1 < boardHeight && board[x,y+1].replacementNeeded() ) ){
							loopMe = true; // reactivate the loop; we need to check further inside
							i++; // increase the depth of the search
							y = y+1; // change the check to be the top box and check instead.
							nextY = y+1; // update the nextY too
						}
						// try getting from the 'left' side of currentGravity
						if(x-1 >= 0 && !board[x-1,y].replacementNeeded() ){
							prevBoard = getPrevBoard(x-1,y-1);
						}
						// if not, try getting from the 'right' side of currentGravity
						if(prevBoard == null && x+1 < boardWidth && !board[x+1,y].replacementNeeded() ){
							prevBoard = getPrevBoard(x+1,y-1);
						}
					}while(prevBoard == null && y+1 < boardHeight && loopMe);
				}
			} else{ // if its the end of the board, spawn a new piece instead~
				StartCoroutine(spawnNew(x,y,gravityVector,nextX,nextY));
			}
			break;
		case Gravity.LEFT :
			nextX = x-1; // info about the next board block too for immediate update after animation
			gravityVector = new Vector3(-size,0,0); // gravity in vector3
			if(x+1 < boardWidth){
				prevBoard = getPrevBoard(x+1,y); // get the previous board box
				if(prevBoard == null && board[x+1,y].panel.isSolid()) { // the prevBoard is a solid type panel
					do // loop to slide pieces like an avalanche
					{
						loopMe = false; // did a loop cycle, turn the bool off
						
						// initial check if the right is covered with solids
						if(( ( y-i >= 0 && board[x+i,y-i].panel.isSolid() ) || y-i < 0 ) && 
						   ( ( y+i < boardHeight && board[x+i,y+i].panel.isSolid() ) || y+i >= boardHeight)
						   && ( x-1 >=0 && board[x-1,y].replacementNeeded() ) ){
							loopMe = true; // reactivate the loop; we need to check further inside
							i++; // increase the depth of the search
							x = x-1; // change the check to be the left box and check instead.
							nextX = x-1; // update the nextX too
						}							// try getting from the 'left' side of currentGravity
						if(prevBoard == null && y-1 >= 0 && !board[x,y-1].replacementNeeded() ){
							prevBoard = getPrevBoard(x+1,y-1);
						}
						// if not, try getting from the 'right' side of currentGravity
						if(prevBoard == null && y+1 < boardHeight && !board[x,y+1].replacementNeeded() ){
							prevBoard = getPrevBoard(x+1,y+1);
						}
					}while(prevBoard == null && x-1 >= 0 && loopMe);
				}
			} else{ // if its the end of the board, spawn a new piece instead~
				StartCoroutine(spawnNew(x,y,gravityVector,nextX,nextY));
			}
			break;
		case Gravity.RIGHT :
			nextX = x+1; // info about the next board block too for immediate update after animation
			gravityVector = new Vector3(size,0,0); // gravity in vector3
			if(x-1 >= 0){
				prevBoard = getPrevBoard(x-1,y); // get the previous board box
				if(prevBoard == null && board[x-1,y].panel.isSolid()) { // the prevBoard is a solid type panel
					do // loop to slide pieces like an avalanche
					{
						loopMe = false; // did a loop cycle, turn the bool off
						
						// initial check if the left is covered with solids
						if(( ( y-i >= 0 && board[x-i,y-i].panel.isSolid() ) || y-i < 0 ) && 
						   ( ( y+i < boardHeight && board[x-i,y+i].panel.isSolid() ) || y+i >= boardHeight)
						   && ( x+1 < boardWidth && board[x+1,y].replacementNeeded() ) ){
							loopMe = true; // reactivate the loop; we need to check further inside
							i++; // increase the depth of the search
							x = x+1; // change the check to be the left box and check instead.
							nextX = x+1; // update the nextX too
						} 
						// try getting from the 'left' side of currentGravity
						if(prevBoard == null && y-1 >= 0 && !board[x,y-1].replacementNeeded() ){
							prevBoard = getPrevBoard(x-1,y-1);
						}
						// if not, try getting from the 'right' side of currentGravity
						if(prevBoard == null && y+1 < boardHeight && !board[x,y+1].replacementNeeded() ){
							prevBoard = getPrevBoard(x-1,y+1);
						}
					}while(prevBoard == null && x+1 < boardWidth && loopMe);
				}
			} else{ // if its the end of the board, spawn a new piece instead~
				StartCoroutine(spawnNew(x,y,gravityVector,nextX,nextY));
			}
			break;
		}
		
		if(prevBoard != null){
			if(board[x,y].piece != null){
				board[x,y].piece.removePiece(); // just in case the reference is lost without removal
			}
			board[x,y].piece = prevBoard.piece; // steal the piece
			prevBoard.piece = null;
			StartCoroutine(animateMove(x,y,nextX,nextY)); // animate the change
			
			// do the same check on the board we stole from as itself needs replacement
			dropPieces(prevBoard.arrayRef[0],prevBoard.arrayRef[1]);
		} else{
			board[x,y].isActive = false; // cannot be filled, so make it not active
		}
	}
	
	// function to check the piece from the previous board but skip special panel type board slot
	Board getPrevBoard(int x, int y){
		// the previous board is legit and has a piece to take from, then take it!
		if(board[x,y].allowGravity()){
			return board[x,y];
		}
		return null; // did not find a piece to take... boo hoo~!
	}
	
	// sub-function to update the board box and tween the piece due to gravity movement
    IEnumerator animateMove (int x, int y, int nextX, int nextY) {
		// update the local data...
		board[x,y].isFalling = true; // board is falling...
		
		int	distance = countBlockedUnfilled(x,y, false);
		float delay = piecesDropSpeed;
		if(acceleratedVelocity){
			delay =  piecesDropSpeed / Mathf.Max(distance, 1);
		}
		
		board[x,y].applyTweening(delay);
		notifyBoardHasChanged(); // board structure changed, so notify the change~!
		
		// the timer according to the drop speed or updatespeed (whichever longer)
   		yield return new WaitForSeconds(delay);
		
		// update the board box once animation has finished..
		board[x,y].isFalling = false; // no longer falling into position
		board[x,y].isActive = true; // piece is active for checks
		
		if( distance < 1 ){ // check if it has reached bottom
			board[x,y].mustWait = true; // reached bottom, re-activate gravity delay
			if(pieceDropExtraEffect){ // if extra effect is enabled
				board[x,y].applyTweeningAfterEffects(piecesDropSpeed, getVectorEffect(x,y) );
			}
			audioScript.playSound(PlayFx.DROPFX); // play the drop sound
		}else {
			dropPieces(nextX,nextY); // check if this new piece needs to fall or not...
		}
    }
	
	// gravity effect after falling down - simulates easeInBack
	Vector3[] getVectorEffect(int x, int y){
		
		float offset = 0.35f * size; // the amount of offset you wish for effect
		Vector3 position = board[x,y].localPos;
		if(board[x,y].isFilled){
			position.z = board[x,y].piece.thisPiece.transform.position.z; // ensure the Z order stays when tweening
		}

		Vector3 pos;
		
		switch(currentGravity){
		case Gravity.DOWN : default :
			pos = new Vector3( 0f , offset, 0f);
			return new Vector3[] {position, (position - pos ), position, position};
		case Gravity.UP :
			pos = new Vector3( 0f , offset/2, 0f);
			return new Vector3[] {(position + pos), position, position, position };
		case Gravity.LEFT :
			pos = new Vector3( offset/2, 0f , 0f);
			return new Vector3[] {(position - pos), position, position, position };
		case Gravity.RIGHT :
			pos = new Vector3( offset/2, 0f , 0f);
			return new Vector3[] {(position + pos), position, position, position };
		}
	}
	
	// sub-function to compensate delay of a new spawned piece tweening process
    public IEnumerator spawnNew (int x, int y, Vector3 spawnPoint, int nextX, int nextY) {
		board[x,y].isFalling = true; // board is falling...
		
		int	distance = countBlockedUnfilled(x,y, false);
		float delay = piecesDropSpeed;
		if(acceleratedVelocity){
			delay =  piecesDropSpeed / Mathf.Max(distance, 1);
		}

		// for custom pieces spawn rate
		PieceDefinition spawned;
		for(int w = 0; w < pieceTypes.Length; w++){
			spawned = pieceTypes[w].chanceToSpawnThis(x,y);
			if(spawned != null){
				board[x,y].spawnNew(spawned, spawnPoint, delay, spawned.skinToUseDuringSpawn(x,y) );
				break;
			}
			if( w == pieceTypes.Length - 1){
				// reached the end, no custom spawn... spawn the default
				board[x,y].spawnNew(pieceTypes[0],spawnPoint, delay, ranType() );
			}
		}
		
		notifyBoardHasChanged(); // board structure changed, so notify the change~!
		
		// the timer according to the drop speed or updatespeed (whichever longer)
   		yield return new WaitForSeconds(delay);
		// update the board box once animation has finished..
		board[x,y].isFalling = false;
		board[x,y].isActive = true;
		board[x,y].mustWait = true; // reached bottom, re-activate gravity delay
		if( distance < 1 ){ // check if it has reached bottom			
			if(pieceDropExtraEffect){ // if extra effect is enabled
				board[x,y].applyTweeningAfterEffects(piecesDropSpeed, getVectorEffect(x,y) );
			}
			audioScript.playSound(PlayFx.DROPFX); // play the drop sound
		} else {
			dropPieces(nextX,nextY); // check if this new piece needs to fall or not...
		}
    }

	// used to determine the number of unfilled board boxes beyond the current panel
	// limited by panels that pieces cannot pass through
	public int countUnfilled(int x, int y, bool ignoreTotalCount){  // extra function currently un-used by GameManager...
		int count = 0;
		switch(currentGravity){
		case Gravity.UP :
			for(int cols = y+1; cols < boardHeight; cols++){
				if(board[x,cols].replacementNeeded() ){
					count++;
					if(ignoreTotalCount) return count; // performance saver, reduce redundant check
				} 
				if(!board[x,cols].panel.isStealable() ){
					break; // do not check further as it cannot pass through here
				}
			}
			break;
		case Gravity.DOWN :
			for(int cols = y-1; cols >= 0 ; cols--){
				if(board[x,cols].replacementNeeded() ){
					count++;
					if(ignoreTotalCount) return count; // performance saver, reduce redundant check
				} 
				if(!board[x,cols].panel.isStealable() ){
					break; // do not check further as it cannot pass through here
				}
			}
			break;
		case Gravity.RIGHT : 
			for(int rows = x+1; rows < boardWidth; rows++){
				if(board[rows,y].replacementNeeded() ){
					count++;
					if(ignoreTotalCount) return count; // performance saver, reduce redundant check
				}
				if(!board[rows,y].panel.isStealable() ){
					break; // do not check further as it cannot pass through here
				}
			}
			break;
		case Gravity.LEFT :
			for(int rows = x-1; rows >=0 ; rows--){
				if(board[rows,y].replacementNeeded() ){
					count++;
					if(ignoreTotalCount) return count; // performance saver, reduce redundant check
				}
				if(!board[rows,y].panel.isStealable() ){
					break; // do not check further as it cannot pass through here
				}
			}
			break;
		}
		return count;
	}
	
	// used to determine the number of unfilled board boxes beyond the current panel
	// limited by panels that block gravity
	public int countBlockedUnfilled(int x, int y, bool ignoreTotalCount){
		int count = 0;
		switch(currentGravity){
		case Gravity.UP :
			for(int cols = y+1; cols < boardHeight; cols++){
				if(board[x,cols].replacementNeeded() ){
					count++;
					if(ignoreTotalCount && count > 0 ) return count; // performance saver, reduce redundant check
				}
				if(!board[x,cols].panel.allowsGravity() ){
					break; // do not check further as it cannot pass through here
				}
				if(!board[x,cols].panel.pnd.hasStartingPiece ) count--;
			}
			break;
		case Gravity.DOWN :
			for(int cols = y-1; cols >= 0 ; cols--){
				if(board[x,cols].replacementNeeded() ){
					count++;
					if(ignoreTotalCount && count > 0 ) return count; // performance saver, reduce redundant check
				}
				if(!board[x,cols].panel.allowsGravity() ){
					break; // do not check further as it cannot pass through here
				}
				if(!board[x,cols].panel.pnd.hasStartingPiece ) count--;
			}
			break;
		case Gravity.RIGHT : 
			for(int rows = x+1; rows < boardWidth; rows++){
				if(board[rows,y].replacementNeeded() ){
					count++;
					if(ignoreTotalCount && count > 0 ) return count; // performance saver, reduce redundant check
				}
				if(!board[rows,y].panel.allowsGravity() ){
					break; // do not check further as it cannot pass through here
				}
				if(!board[rows,y].panel.pnd.hasStartingPiece ) count--;
			}
			break;
		case Gravity.LEFT :
			for(int rows = x-1; rows >=0 ; rows--){
				if(board[rows,y].replacementNeeded() ){
					count++;
					if(ignoreTotalCount && count > 0 ) return count; // performance saver, reduce redundant check
				}
				if(!board[rows,y].panel.allowsGravity() ){
					break; // do not check further as it cannot pass through here
				}
				if(!board[rows,y].panel.pnd.hasStartingPiece ) count--;
			}
			break;
		}
		return count;
	}

	// used to determine the number of unfilled board boxes in a line to fall through.
	public int emptyBoxesBeyond(int x, int y){ // extra function currently un-used by GameManager...
		int count = 0;
		switch(currentGravity){
		case Gravity.UP :
			for(int cols = y+1; cols < boardHeight; cols++){
				if(board[x,cols].replacementNeeded() ){
					count++;
				} else {
					break;
				}
			}
			break;
		case Gravity.DOWN :
			for(int cols = y-1; cols >= 0 ; cols--){
				if(board[x,cols].replacementNeeded() ){
					count++;
				} else {
					break;
				}
			}
			break;
		case Gravity.RIGHT : 
			for(int rows = x+1; rows < boardWidth; rows++){
				if(board[rows,y].replacementNeeded() ){
					count++;
				} else {
					break;
				}
			}
			break;
		case Gravity.LEFT :
			for(int rows = x-1; rows >=0 ; rows--){
				if(board[rows,y].replacementNeeded() ){
					count++;
				} else {
					break;
				}
			}
			break;
		}
		return count;
	}
	
	// >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
	// PieceTracker movement notifier ( DO NOT TOUCH UNLESS NECCESSARY~! )
	// >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
	
	// for external source call method (called from PieceTracker.cs script), 
	// this is to drag gems on the board
	public void draggedFromHere(int[] pos,PieceTracker.SwitchedWith partner){
		
		if(moveOnlyAfterSettle){
			if(!checkedPossibleMove){
				return; // player needs to wait for board to settle before making the next move...
			}
		}
		if (!canMove){
			return; // if cannot move, exit~! ( player perhaps made a bad move previously )
		}
		
		int posX1,posY1; // the calling board position
		int posX2,posY2; // the partner board position
		posX1 = pos[0];	posY1 = pos[1];
		posX2 = posX1; posY2 = posY1; // for the stupid warning of uninstantiated value
		
		switch( partner ) {
			case PieceTracker.SwitchedWith.DOWN :
				posX2 = (int) posX1;
				posY2 = (int) posY1-1;
				break;
			case PieceTracker.SwitchedWith.UP :
				posX2 = (int) posX1;
				posY2 = (int) posY1+1;
				break;
			case PieceTracker.SwitchedWith.LEFT :
				posX2 = (int) posX1-1;
				posY2 = (int) posY1;
				break;
			case PieceTracker.SwitchedWith.RIGHT :
				posX2 = (int) posX1+1;
				posY2 = (int) posY1;
				break;
		}
		
		// extra conditioning check if pieces can be moved
		if(!(posX2 >=0 && posX2 < boardWidth) || !(posY2 >=0 && posY2 < boardHeight) ||
		   !board[posX1,posY1].panel.isSwitchable() || !board[posX2,posY2].panel.isSwitchable() ||
			board[posX1,posY1].isFalling || !board[posX1,posY1].isFilled ||
			board[posX2,posY2].isFalling || !board[posX2,posY2].isFilled ||
			!board[posX1,posY1].piece.thisPiece.GetComponent<PieceTracker>().enabled ||
			!board[posX2,posY2].piece.thisPiece.GetComponent<PieceTracker>().enabled){
			// condition above states the box are not legit selections, do not proceed!!!
			return;
		}

		PieceDefinition pdMain = board[posX1,posY1].piece.pd; // the calling piece definition
		PieceDefinition pdSub = board[posX2,posY2].piece.pd; // the partner piece definition

		// check if we are merging two power gems
		if(pdMain.powerMatched(posX1,posY1,posX2,posY2,true,pdMain,pdSub) ||
		   pdSub.powerMatched(posX1,posY1,posX2,posY2,true,pdSub,pdMain) ){
			playerMadeAMove(); // call the function when player makes a valid move
			return; // we are merging, so it is handled elsewhere, job done here...so, return!
		}
		
		// initiate the switch if the two board pieces are switchable
		StartCoroutine(switchPositions(posX1,posY1,posX2,posY2));
	}
	
	
	// tween the pieces and perform actions after the given time (to accomodate the tweening)
    IEnumerator switchPositions (int posX1, int posY1, int posX2, int posY2) {
		
		audioScript.playSound(PlayFx.SWITCHFX); // play the switch sound fx
		
		// freeze the boxes involved
		board[posX1,posY1].isFalling = true;
		board[posX2,posY2].isFalling = true;
		
		// switch the two pieces around in memory (not visual in GUI yet)
		GamePiece holder = board[posX1,posY1].piece;
		board[posX1,posY1].piece = board[posX2,posY2].piece;
		board[posX2,posY2].piece = holder;
		
		// tween it ( now only visual in GUI)
		board[posX1,posY1].applyTweening(gemSwitchSpeed);
		board[posX2,posY2].applyTweening(gemSwitchSpeed);
		
    	yield return new WaitForSeconds(gemSwitchSpeed); // the timer
		
		// assign the type in a shorter reference just for easier usage
		int t1 = board[posX1,posY1].piece.slotNum;
		int t2 = board[posX2,posY2].piece.slotNum;
		
		// extensive check to verify that the move is legit
		if( checkThisPosition(posX1,posY1,t1,Check.UP)|| checkThisPosition(posX1,posY1,t1,Check.DOWN)
		|| checkThisPosition(posX1,posY1,t1,Check.LEFT) || checkThisPosition(posX1,posY1,t1,Check.RIGHT)
		|| checkThisPosition(posX2,posY2,t2,Check.UP)|| checkThisPosition(posX2,posY2,t2,Check.DOWN)
		|| checkThisPosition(posX2,posY2,t2,Check.LEFT) || checkThisPosition(posX2,posY2,t2,Check.RIGHT)) {
	
			// if legit, un- freeze the boxes involved
			board[posX1,posY1].isFalling = false;
			board[posX2,posY2].isFalling = false;
			
			board[posX1,posY1].isActive = true; // make the piece active for checks
			board[posX2,posY2].isActive = true; // make the piece active for checks
			
			playerMadeAMove(); // call the function when player makes a valid move
		} else {
			// if move is not legit, revert it back
			StartCoroutine(revertMove(posX1,posY1,posX2,posY2)); // to revert the last move
		}
    }
	
	// to revert the actions if the last move was an invalid move
    IEnumerator revertMove (int posX1, int posY1, int posX2, int posY2) {
		// NOTE : remember that the boxes is still frozen... (in switchPositions() )
		
		audioScript.playSound(PlayFx.BADMOVEFX); // play the bad move sound fx
		
		canMove = false; // player cannot move until it is reverted
		// switch it back around...
		GamePiece holder = board[posX1,posY1].piece;
		board[posX1,posY1].piece = board[posX2,posY2].piece;
		board[posX2,posY2].piece = holder;
			
		// tween it ( make it visual in GUI)
		board[posX1,posY1].applyTweening(gemSwitchSpeed);
		board[posX2,posY2].applyTweening(gemSwitchSpeed);
		
    	yield return new WaitForSeconds(gemSwitchSpeed); // the timer
		// un- freeze the boxes involved for checks
		board[posX1,posY1].isFalling = false;
		board[posX2,posY2].isFalling = false;
		board[posX1,posY1].isActive = true; // make the piece active for checks
		board[posX2,posY2].isActive = true; // make the piece active for checks
		
		canMove = true; // give power back to the player
    }

	void playerMadeAMove(){
		if(movingResetsCombo) JMFRelay.onComboEnd(); // end the combo if no special override...
		moves++; // merging, so number of moves increase

		JMFRelay.onPlayerMove();
		notifyBoardHasChanged(); // notify the change~!
	}
	
	
	// ===========================
	// UNITY FUNCTIONS
	// ===========================

	void Awake () { // board needs to be initialized before other scripts can access it
		JMFUtils.gm = this; // make a easy reference to the GameManager ( this script ! ) 
		JMFUtils.wc = GetComponent<WinningConditions>(); // make a easy reference to the WinningConditions script~!
		JMFRelay.onPreGameStart();
		initializeGame();
		preGameSetup();
	}
	
	void Start() { // when the game is actually running...
		// Initialize Timers and settings
		StartCoroutine(updater()); // initiate the update loop
		StartCoroutine(boardCheckLooper()); // initiate the check loop

		JMFRelay.onGameStart();
	}
	
	// Update is called once per frame
	void Update () {
		// woohoo~ nothing here??
	}
}
