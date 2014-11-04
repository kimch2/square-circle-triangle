using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathologicalGames;

/// <summary> ##################################
/// 
/// NOTICE :
/// This script is the GamePiece class used by the "Board" script.
/// It controls the visual representation as well as the defining character of a piece.
/// 
/// DO NOT TOUCH UNLESS REQUIRED
/// 
/// </summary> ##################################

public class GamePiece { // the game pieces as individual pieces on the board

	const string piecePoolName = JMFUtils.piecePoolName;
	public int slotNum; // definition of the skin array to use and piece type
	public GameObject thisPiece; // GUI object reference
	public Board master; // reference of this script
	public int extraEffectID = 0; // the ID for extra effect tweening called by Board.cs
	public bool markedForDestroy = false; // state to signify that it is waiting to be destroyed
	public bool justCreated = false;  // for match create power piece so that it doesnt get destroyed instantly
	public PieceDefinition pd;
	public Vector3 position;

	public GamePiece(PieceDefinition newPd,Board newMaster, int type, Vector3 newPosition){
		pd = newPd;
		master = newMaster;
		slotNum = type;
		position = newPosition;
	}

	public void init(){
		dressMe();
	}
	
	// visual representation of the game piece to the player
	public void dressMe() {

		destroyCall();

		if(pd == null){
			return; // no piece definition... quit
		}

		if(JMFUtils.isPooling){
			// PoolManager version
			thisPiece = PoolManager.Pools[piecePoolName].Spawn(pd.getSkin(slotNum).transform).gameObject;
		} else {
			// non-PoolManager version...		
			thisPiece = (GameObject) Object.Instantiate(pd.getSkin(slotNum));
		}

		pd.onPieceCreated(this); // piece is created, call the onCreate (if any)
		
		thisPiece.transform.parent = master.gm.gameObject.transform; // re-parent the object to the gameManager panel
		thisPiece.transform.position = position;
		
		if ( thisPiece.GetComponent<BoxCollider>() == null){ 
			thisPiece.AddComponent<BoxCollider>(); // add a box collider if not present
		}
		
		JMFUtils.autoScalePadded(thisPiece); // auto scaling feature
		pd.extraPiecePositioning(thisPiece);
		
		// prefab properties to sync-up ( don't forget the PieceTracker script )
		PieceTracker pt = thisPiece.GetComponent<PieceTracker>();
		if(pt == null){ // if the prefab doesnt have the script, add it dynamically...
			pt = thisPiece.AddComponent<PieceTracker>();
		}
		pt.boardPosition = master.arrayRef;
	}
	
	// for external scripts to call, destroys the game piece with validation checks
	public void destroy(){
		if(pd != null){
			if( !pd.isSpecial ){ // not a special piece... it is a colored piece
				master.gm.matchCount[slotNum]++; // increase the type count that is destroyed.
			}
			pd.onPieceDestroyed(this); // call the piece type onDestroy function (if any)
			pd = null; // null the piece attribute here
			master.gm.animScript.doAnim(animType.GLOBALDESTROY,master.arrayRef[0],master.arrayRef[1]);
		}
		destroyCall();
	}
	
	// method to remove the piece as we dont need it anymore...
	// no validation or checks performed.. just remove it and reset!
	public void removePiece(){
		pd = null; // null the piece attribute here
		destroyCall();
	}

	// for board reset when no more moves
	public void resetMe (PieceDefinition pieceType, int skinNum) {
		if(pd.ignoreReset){ // non-resettable piece
			return;
		}

		pd = pieceType;
		slotNum = skinNum;
		dressMe();
	}
	
	// converts this piece to a special piece
	public void specialMe(PieceDefinition specialType){
		pd = specialType;
		dressMe();
	}

	// generic destroy function - all destroy call refers to this one function. Intended for Pooling friendly
	public void destroyCall(){
		if(JMFUtils.isPooling){
			// POOL MANAGER Version
			
			if(thisPiece != null){
				// reset to default settings
				thisPiece.GetComponent<PieceTracker>().enabled = true;
				LeanTween.cancel(thisPiece);
				thisPiece.transform.localScale = Vector3.one;
				
				// give back to the pool
				PoolManager.Pools[piecePoolName].Despawn(thisPiece.transform);
				thisPiece = null;
			}
		} else {
			// NON- POOL MANAGER
			if(thisPiece != null){
				Object.Destroy(thisPiece); // destroy the piece (if any)
			}
		}
	}

}