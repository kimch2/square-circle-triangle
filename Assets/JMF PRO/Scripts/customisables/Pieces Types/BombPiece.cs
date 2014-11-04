using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("JMF/Pieces/BombPiece")]
public class BombPiece : PieceDefinition {
	
	public override bool performPower(int[] arrayRef){
		doPowerTMatch(arrayRef); // match T line type power ( destroys surrounding pieces 3x3 area)
		return false;
	}


	public override bool powerMatched(int posX1, int posY1, int posX2, int posY2, bool execute,
	                                  PieceDefinition thisPd, PieceDefinition otherPd){
		if(otherPd is VerticalPiece || otherPd is HorizontalPiece){
			if(execute) StartCoroutine( doPowerMergeTX(posX1, posY1, posX2, posY2));
			return true;
		}
		if(otherPd is BombPiece){
			if(execute) StartCoroutine( doPowerMergeT(posX1, posY1, posX2, posY2));
			return true;
		}
		return false;
	}
	
	public override bool matchConditions(int xPos, int yPos, List<Board> linkedCubesX, List<Board> linkedCubesY){
		if ( linkedCubesX.Count > 1 && linkedCubesY.Count > 1) { // + or T or L-type match special pieces
			gm.board[xPos,yPos].convertToSpecial(this); // makes the cube a special piece
			gm.board[xPos,yPos].panelHit();

			//lock the piece for just created power piece
			StartCoroutine(gm.lockJustCreated(xPos,yPos,0.3f));
			return true;
		}
		return false;
	}

	//
	// POWER DEFINITION
	//

	// match T line type power ( destroys surrounding pieces 3x3 area)
	public void doPowerTMatch(int[] pos){
		
		gm.audioScript.playSound(PlayFx.STAR); // play this sound fx
		
		float delayPerPiece = 0.1f;
		int mScore = 50; // the score you want to give per destroyed box in this range
		gm.animScript.doAnim(animType.STAR,pos[0],pos[1]); // visual fx animation
		for(int x = pos[0]-1; x <= pos[0]+1; x++){
			if(x >= 0 && x < gm.boardWidth){
				for(int y = pos[1]-1;y <= pos[1]+1;y++){
					if(y >= 0 && y < gm.boardHeight){
						StartCoroutine(gm.destroyInTime(x,y, delayPerPiece, mScore));
					}	
				}
			}
		}
	}
	
	// match T line type power ( destroys surrounding pieces (bigger scale, 5x5) )
	public void doPowerTMatchBig(int[] pos){
		
		gm.audioScript.playSound(PlayFx.STAR); // play this sound fx
		
		float delayPerPiece = 0.1f;
		int mScore = 50; // the score you want to give per destroyed box in this range
		gm.animScript.doAnim(animType.STAR,pos[0],pos[1]); // visual fx animation
		for(int x = pos[0]-2; x <= pos[0]+2; x++){
			if(x >= 0 && x < gm.boardWidth){
				for(int y = pos[1]-2;y <= pos[1]+2;y++){
					if(y >= 0 && y < gm.boardHeight){
						StartCoroutine(gm.destroyInTime(x,y, delayPerPiece, mScore));
					}	
				}
			}
		}
	}

	// double surrounding-box destruction ( 5x5 area effect )
	IEnumerator doPowerMergeT(int posX1, int posY1, int posX2, int posY2){
		
		gm.audioScript.playSound(PlayFx.STAR); // play this sound fx
		
		StartCoroutine(gm.mergePieces(posX1,posY1,posX2,posY2,true)); // for visual effect mostly
		yield return new WaitForSeconds(gm.gemSwitchSpeed);
		
		// lock the piece so that it is not destroyed prematurely
		StartCoroutine(gm.lockJustCreated(posX1,posY1,3f)); 
		StartCoroutine(gm.lockJustCreated(posX2,posY2,3f));
		
		doPowerTMatchBig(new int[2]{posX1,posY1}); // do the T match (big ver.) power!
		doPowerTMatchBig(new int[2]{posX2,posY2}); // do the T match (big ver.) power!
		
		GamePiece ref1 = gm.board[posX1,posY1].piece; // save a reference for later
		GamePiece ref2 = gm.board[posX2,posY2].piece; // save a reference for later
		
		
		if(ref1 != null){
			// visual effect for a time bomb
			Vector3 newSize = Vector3.Scale(ref1.thisPiece.transform.localScale,new Vector3(1.45f,1.45f,1f));
			LeanTween.scale( ref1.thisPiece, newSize ,0.5f, new object[]{"loopType",LeanTweenType.pingPong});
			ref1.thisPiece.GetComponent<PieceTracker>().enabled = false;
		}
		if(ref2 != null){
			// visual effect for a time bomb
			Vector3 newSize = Vector3.Scale(ref2.thisPiece.transform.localScale,new Vector3(1.45f,1.45f,1f));
			LeanTween.scale( ref2.thisPiece, newSize ,0.5f, new object[]{"loopType",LeanTweenType.pingPong});
			ref2.thisPiece.GetComponent<PieceTracker>().enabled = false;
		}
		
		yield return new WaitForSeconds(3f); // wait for 3 secs
		ref1.master.destroyBox(); // destroy the piece causing another explosion!!
		ref2.master.destroyBox(); // destroy the piece causing another explosion!!
		// incomplete
	}
	
	// + shape destruction size 3-lines
	IEnumerator doPowerMergeTX(int posX1, int posY1, int posX2, int posY2){
		
		gm.audioScript.playSound(PlayFx.ARROWFX); // play this sound fx
		
		StartCoroutine(gm.mergePieces(posX1,posY1,posX2,posY2,false)); // for visual effect mostly
		yield return new WaitForSeconds(gm.gemSwitchSpeed);
		
		float delayPerPiece = 0.1f;
		int mScore = 50;
		
		gm.animScript.doAnim(animType.ARROWTX, posX2, posY2); // the visual fx animation
		
		// destroy the power gems without triggering it's natural power
		gm.board[posX1,posY1].destroyMarked();
		gm.board[posX2,posY2].destroyMarked();
		
		// 3x3 area around the origin piece
		doPowerTMatch(new int[] {posX2,posY2});
		
		// outside the 3x3 area and beyond...
		for(int y = posY2-1; y <= posY2+1; y++){
			for(int x = 2; x < gm.boardWidth; x++){ // going left n right
				if(posX2-x >= 0 && y >=0 && y < gm.boardHeight){
					StartCoroutine(gm.destroyInTime(posX2-x,y, delayPerPiece*x, mScore));
				}
				if(posX2+x < gm.boardWidth && y >=0 && y < gm.boardHeight){
					StartCoroutine(gm.destroyInTime(posX2+x,y, delayPerPiece*x, mScore));
				}
			}
		}
		
		for(int x = posX2-1; x <= posX2+1; x++){
			for(int y = 2; y < gm.boardHeight; y++){ // going up n down
				if(posY2-y >= 0 && x >=0 && x < gm.boardWidth){
					StartCoroutine(gm.destroyInTime(x,posY2-y, delayPerPiece*y, mScore));
				}
				if(posY2+y < gm.boardHeight && x >=0 && x < gm.boardWidth){
					StartCoroutine(gm.destroyInTime(x,posY2+y, delayPerPiece*y, mScore));
				}
			}
		}
	}
}
