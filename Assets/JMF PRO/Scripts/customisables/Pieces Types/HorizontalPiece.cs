using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("JMF/Pieces/HorizontalPiece")]
public class HorizontalPiece : PieceDefinition {
	
	public override bool performPower(int[] arrayRef){
		doPowerHorizontal(arrayRef); // match 4 line type power Horizontal line
		return false;
	}
	
	public override bool powerMatched(int posX1, int posY1, int posX2, int posY2, bool execute,
	                                  PieceDefinition thisPd, PieceDefinition otherPd){
		if(otherPd is VerticalPiece || otherPd is HorizontalPiece){
			if(execute) StartCoroutine( doPowerMergeVH(posX1, posY1, posX2, posY2));
			return true;
		}
		return false;
	}
	
	public override bool matchConditions(int xPos, int yPos, List<Board> linkedCubesX, List<Board> linkedCubesY){
		if (linkedCubesY.Count > 2) { // 4 match in a row
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

	// match 4 line type power Horizontal line
	public void doPowerHorizontal(int[] pos){
		
		gm.audioScript.playSound(PlayFx.ARROWFX); // play this sound fx
		
		float delayPerPiece = 0.1f;
		int mScore = 50; // the score you want to give per destroyed box in this range
		gm.animScript.doAnim(animType.ARROWH,pos[0],pos[1]); // visual fx animation
		for(int x = 0; x < gm.boardWidth; x++){
			if(pos[0]-x >= 0){
				StartCoroutine(gm.destroyInTime(pos[0]-x,pos[1], delayPerPiece*x, mScore));
			}
			if(pos[0]+x < gm.boardWidth){
				StartCoroutine(gm.destroyInTime(pos[0]+x,pos[1], delayPerPiece*x, mScore));
			}
		}
	}

	IEnumerator doPowerMergeVH(int posX1, int posY1, int posX2, int posY2){
		
		gm.audioScript.playSound(PlayFx.ARROWFX); // play this sound fx
		
		StartCoroutine(gm.mergePieces(posX1,posY1,posX2,posY2,false)); // for visual effect mostly
		yield return new WaitForSeconds(gm.gemSwitchSpeed);
		
		float delayPerPiece = 0.1f;
		int mScore = 50;
		
		gm.animScript.doAnim(animType.ARROWVH, posX2, posY2);
		
		// destroy the power gems without triggering it's natural power
		gm.board[posX1,posY1].destroyMarked();
		gm.board[posX2,posY2].destroyMarked();
		
		for(int x = 0; x < gm.boardWidth ; x++){
			if(posX2-x >= 0){
				StartCoroutine(gm.destroyInTime(posX2-x,posY2, delayPerPiece*x, mScore));
			}
			if(posX2+x < gm.boardWidth){
				StartCoroutine(gm.destroyInTime(posX2+x,posY2, delayPerPiece*x, mScore));
			}
		}
		for(int y = 0; y < gm.boardHeight ; y++){
			if(posY2-y >= 0){
				StartCoroutine(gm.destroyInTime(posX2,posY2-y, delayPerPiece*y, mScore));
			}
			if(posY2+y < gm.boardHeight){
				StartCoroutine(gm.destroyInTime(posX2,posY2+y, delayPerPiece*y, mScore));
			}
		}
	}
}
