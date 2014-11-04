using UnityEngine;
using System.Collections;


/// <summary> ##################################
/// 
/// NOTICE :
/// This script is the Board class.
/// It is the board position on the screen that will control whether pieces
/// or panels appear or not. It is the the container for GamePieces and BoardPanel.
/// 
/// DO NOT TOUCH UNLESS REQUIRED
/// 
/// </summary> ##################################
/// 

public class Board { // the game board as individual boxes

	public Vector3 position; // board position on the GUI (world position)
	public Vector3 localPos; // board position (local position)
	public GamePiece piece; // the game piece that is linked to this board
	public BoardPanel panel; // the panel that is linked to this board
	public HUDPopUp scoreHUD; // as the scoreHUD reference
	public bool isFilled { //= false; // determines if there is a piece on this board
		get { 
			if(piece == null || piece.pd == null){ return false;} // no piece here, is not filled
			else { return true;} // piece exist, isFilled...
		} // set { } // restricted to read-only
	}
	public bool isFalling = false; // states whether the piece is falling into position
	public bool isActive = true; // states whether it was active since the last routine check
	public bool justCreated { // for match create power piece so that it doesnt get destroyed instantly
		get{if(isFilled)return piece.justCreated; return false;} 
		set{if(isFilled) piece.justCreated = value;} }
	public bool isBeingDelayed = false; // state for the board performing the gravity delay
	public bool mustWait = true; // state that the board must wait for the gravity delay
	public int[] arrayRef; // to know its array number for reference
	public GameManager gm; // this script as a reference
	
	public Board(GameManager gameManager, int[] boardPosition, Vector3 pos) {
		gm = gameManager;
		arrayRef = boardPosition; // to help it remember it's position in the array
		localPos = pos;
		position = gm.transform.TransformPoint(pos);
		
		// scoreHUD display setup
		if(gm.scoreHUD != null){
			GameObject scoreHUDObj  = (GameObject) Object.Instantiate(gm.scoreHUD);
			scoreHUDObj.transform.parent = gm.gameObject.transform;
			scoreHUDObj.transform.position = position;
			scoreHUD = scoreHUDObj.GetComponent<HUDPopUp>();
			if(scoreHUD == null){
				Debug.Log("scoreHUD 'HUDPopUp' script missing from prefab~!!"); // tell developers the problem
			}
		} else {
			Debug.Log("No scoreHUD prefab detected..."); // tell developers the problem
		}
	}

	// function to init all the required stuff during OnStart()
	public void init(){
		piece.init(); // init piece (game objects are created now...)
		panel.initPanels(); // init panels (game objects are created now...)

		// call the GameStart() for custom pieces and panels
		if(piece != null && piece.pd != null) piece.pd.onGameStart(this);
		if(panel != null && panel.pnd != null) panel.pnd.onGameStart(this);
	}

	/// $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
	/// Panel stuff
	/// $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$


	public void panelHit(){
		if( panel.gotHit() ) {
			isActive = true; // panel activity registered, set board active for checks
		}
	}

	// piece and panel splash damage call
	public void SplashDamage(){
		// 
		// panel splash call
		//
		if ( panel.splashDamage() ) {
			isActive = true; // panel activity registered, set board active for checks
		}

		//
		// piece splash call
		//
		if(isFilled){
			piece.pd.splashDamage(this); // function call ( if any )
		}
	}

	/// $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
	/// Others
	/// $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$

	// destroy the piece in this board
	public void destroyBox () {
		if(!justCreated && panel.isDestructible() && isFilled && !piece.markedForDestroy) {
			if( piece.pd.isDestructible ){ // valid for destroy
				if(piece.pd.performPower(arrayRef) ) { // if true, mark for delayed destroy
					piece.markedForDestroy = true;
				}
				if(!piece.markedForDestroy){ // if not marked for delayed destroy, destroy immediately
					piece.pd.splashDamage(this); // virtual function call ( if any )
					piece.destroy();
					isFalling = false;
					panelHit(); // reduce panel durability ( if possible )
				}
			} else {
				panelHit(); // reduce panel durability ( if possible )
			}
		}
	}

	// destroy the piece in this board
	public void forceDestroyBox () {
		if(isFilled && !piece.markedForDestroy){ // valid for destroy
			if(piece.pd.performPower(arrayRef) ) { // if true, mark for delayed destroy
				piece.markedForDestroy = true;
			}
			if(!piece.markedForDestroy){ // if not marked for delayed destroy, destroy immediately
				piece.destroy();
//				isFilled = false;
				isFalling = false;
				panelHit(); // reduce panel durability ( if possible )
			}
		}
	}

	// for external scripts to call, signify that it is time to destroy it after being delayed...
	public void destroyMarked(){

		if(piece != null){
			piece.markedForDestroy = false;
			piece.destroy();
		}
		isFalling = false;
		panelHit(); // reduce panel durability ( if possible )
	}
	
	// converts a piece that is here to be a special piece
	public void convertToSpecial(PieceDefinition pd) {
		if(isFilled){
			piece.pd.performPower(arrayRef); // trigger specials if any
		}
		piece.destroy();
		piece.specialMe(pd);
	}
	public void convertToSpecial(PieceDefinition pd, int newSlotNum) {
		if(isFilled){
			piece.pd.performPower(arrayRef); // trigger specials if any
		}
		piece.destroy();
		piece.slotNum = newSlotNum;
		piece.specialMe(pd);
	}
	
	// converts a piece that is here to be a special piece
	public void convertToSpecialNoDestroy(PieceDefinition pd, int newSlotNum) {
		piece.destroy();
		piece.slotNum = newSlotNum;
		piece.specialMe(pd);
	}
	// sets the piece that is here to be a special piece
	public void setSpecialPiece(PieceDefinition pd) {
		if(panel.pnd.hasStartingPiece){
			piece.removePiece();
			if(pd.isSpecial){ // if it's a special type, define the appropriate skin
				piece.slotNum = pd.skinToUseDuringSpawn(arrayRef[0],arrayRef[1]);
			}
			piece.pd = pd; // sets the pd type
		}
	}
	
	// to create a new piece object when the board is new
	public void createObject (PieceDefinition pd, int skinNum) {
		piece = new GamePiece(pd, this, skinNum, position);
		isFalling = false;
		isActive = true;
	}
	
	// reset the board when no more moves
	public void reset(PieceDefinition pd, int skinNum) {
		if(panel.isFillable()){ // if the panel can hold a game piece
			if (isFilled){
				piece.resetMe(pd, skinNum); // reset it
			} else { // game piece was stolen by another board and the reference is wrong. create a new piece
				piece = new GamePiece(pd, this, skinNum, position);
				piece.init();
			}
			isFalling = false;
			isActive = true;
		}
	}
	
	// function to determine if pieces are allowed to be stolen by other boards
	public bool allowGravity() {
		if ( panel.isStealable() && isFilled && !isFalling && piece.pd.allowGravity ) {
			return true;
		}
		return false;
	}
	// function to determine if this board requires a piece replacement when empty
	public bool replacementNeeded() {
		if ( panel.allowsGravity() && !isBeingDelayed && !isFilled && !isFalling ) {
			return true;
		}
		return false;
	}
	
	// call function to determine if the board is ready to be matched by 'MatchCheck()' in GameManager
	public bool canBeMatched(){
		if(panel.isMatchable() && !justCreated && !isFalling && isFilled
		   && !piece.markedForDestroy && !piece.pd.isSpecial){
			return true;
		}
		return false;
	}

	// call function to determine if the board has a match by 'eliminate prestart-match' in GameManager
	public bool preStartMatched(){
		if(!isFilled){
			return false; // does not have a piece here... ( non-piece holding panel )
		}
		if(panel.isMatchable()){
			return true;
		}
		return false;
	}
	
	// to spawn a new object dropping out of the box by gravity
	public void spawnNew(PieceDefinition pd, Vector3 pos, float dropSpeed, int skinNum) {
		isActive = true;
		piece = new GamePiece(pd, this, skinNum, position - pos);
		piece.init();
		applyTweening(dropSpeed);
	}
	
	// moves the pieces on the GUI for visual feedback
	public void applyTweening(float dropSpeed){
		if(!isFilled){
			isFalling = false;
			return; // likely destroyed by other powers already before it managed to tween, reset the board
		}
		piece.master = this; // update the 'master' reference
		piece.position = this.position; // sync the position data
		if(piece.thisPiece != null){
			piece.thisPiece.GetComponent<PieceTracker>().boardPosition = arrayRef;
			LeanTween.cancel(piece.thisPiece, piece.extraEffectID);
			Vector3 movePos = position;
			movePos.z = piece.thisPiece.transform.position.z; // ensure the Z order stays when tweening
			LeanTween.move( piece.thisPiece, movePos ,dropSpeed);
		} else { // likely destroyed by other powers already before it managed to tween, reset the board
			isFalling = false;
		}
	}
	
	// special effects tweening... 
	public void applyTweeningAfterEffects(float effectSpeed, Vector3[] path){
		if(isFilled && piece.thisPiece != null){
			// play the visual effect
			piece.extraEffectID = LeanTween.moveLocal(  piece.thisPiece, path, effectSpeed, new object[]{});
		} else { // likely destroyed by other powers already before it managed to tween, reset the board
			isFalling = false;
		}
	}
} // end of Board class
