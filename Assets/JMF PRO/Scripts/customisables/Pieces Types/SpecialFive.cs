using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("JMF/Pieces/SpecialFive")]
public class SpecialFive : PieceDefinition {

	public override bool performPower(int[] arrayRef){
		StartCoroutine( doPower5Match(arrayRef, 2f) );
		return true;
	}
	
	public override bool powerMatched(int posX1, int posY1, int posX2, int posY2, bool execute,
	                                  PieceDefinition thisPd, PieceDefinition otherPd){
		if(otherPd is NormalPiece){
			if(execute) StartCoroutine( doPowerMerge5(posX1, posY1, posX2, posY2));
			return true;
		}
		if(otherPd is VerticalPiece || otherPd is HorizontalPiece){
			if(execute) StartCoroutine( doPowerMerge5X(posX1, posY1, posX2, posY2));
			return true;
		}
		if(otherPd is BombPiece){
			if(execute) StartCoroutine( doPowerMerge5T(posX1, posY1, posX2, posY2));
			return true;
		}
		if(otherPd is SpecialFive){
			if(execute) StartCoroutine( doPowerMerge55(posX1, posY1, posX2, posY2));
			return true;
		}
		return false;
	}
	
	public override bool matchConditions(int xPos, int yPos, List<Board> linkedCubesX, List<Board> linkedCubesY){
		if ( linkedCubesX.Count > 3 || linkedCubesY.Count > 3) { // 5 match special pieces
			gm.board[xPos,yPos].convertToSpecial(this,0); // makes the cube a special piece
			gm.board[xPos,yPos].panelHit();

			//lock the piece for just created power piece
			StartCoroutine(gm.lockJustCreated(xPos,yPos,0.3f));
			return true;
		}
		return false;
	}

	public override void extraPiecePositioning (GameObject thisPiece){
		thisPiece.transform.localPosition += new Vector3(0,0,-1*thisPiece.transform.localScale.z);
	}


	// match 5 type power ( randomly pick a color and destroys same color )
	public IEnumerator doPower5Match(int[] pos, float delay){ // it's a numerator coz we need the timer function
		
		gm.audioScript.playSound(PlayFx.RAINBOW); // play this sound fx
		
		float delayPerPiece = 0.01f;
		
		int mScore = 50; // the score you want to give per destroyed box in this range
		gm.board[pos[0],pos[1]].isFalling = true; // lock the box to ignore gravity
		gm.animScript.doAnim(animType.RAINBOW,pos[0],pos[1]); // visual fx animation that lasts for 2 seconds

		StartCoroutine( gm.destroyInTimeMarked(pos[0],pos[1], delay, mScore) );

		yield return new WaitForSeconds(delay);
		int type = Random.Range(0,gm.NumOfActiveType);
		for(int x = 0; x < gm.boardWidth;x++){
			for(int y = 0; y < gm.boardHeight;y++)
			{
				if(gm.board[x,y].isFilled && !gm.board[x,y].piece.pd.isSpecial && 
				   gm.board[x,y].piece.slotNum == type){
					StartCoroutine(gm.destroyInTime(x,y, delayPerPiece, mScore));
				}
			}
		}
	}

	// destroys same color
	IEnumerator doPowerMerge5(int posX1, int posY1, int posX2, int posY2){
		
		StartCoroutine(gm.mergePieces(posX1,posY1,posX2,posY2,false)); // for visual effect mostly
		yield return new WaitForSeconds(gm.gemSwitchSpeed);
		
		float delayPerPiece = 2f;
		int mScore = 50; // the score you want to give per destroyed box in this range
		int type = 0;
				
		if(gm.board[posX2,posY2].piece.pd is SpecialFive){
			type = gm.board[posX1,posY1].piece.slotNum;
			StartCoroutine( gm.destroyInTimeMarked(posX2,posY2, delayPerPiece, mScore) );
		} else {
			type = gm.board[posX2,posY2].piece.slotNum;
			StartCoroutine( gm.destroyInTimeMarked(posX1,posY1, delayPerPiece, mScore) );
		}
		
		StartCoroutine(doPower5MatchColored(new int[] {posX2,posY2},2f,type,true)); // call the power with specific color.
	}
	
	// match 5 type power ( destroys specified param color )
	// it's a IEnumerator coz we need the timer function
	public IEnumerator doPower5MatchColored(int[] pos, float delay, int colorType, bool visuals){

		if(visuals){ // if play visual and sound effect
			gm.audioScript.playSound(PlayFx.RAINBOW); // play this sound fx
			gm.animScript.doAnim(animType.RAINBOW,pos[0],pos[1]); // visual fx animation
		} 
		
		float delayPerPiece = 0.01f;
		int mScore = 50; // the score you want to give per destroyed box in this range

		yield return new WaitForSeconds(delay);

		for(int x = 0; x < gm.boardWidth;x++){
			for(int y = 0; y < gm.boardHeight;y++)
			{
				if(gm.board[x,y].isFilled && !gm.board[x,y].piece.pd.isSpecial && 
				   gm.board[x,y].piece.slotNum == colorType){
					StartCoroutine(gm.destroyInTime(x,y, delayPerPiece, mScore));
				}
			}
		}
	}

	// converts all same color to V or H and destroys
	IEnumerator doPowerMerge5X(int posX1, int posY1, int posX2, int posY2){
		
		gm.audioScript.playSound(PlayFx.SPECIALMATCH); // play this sound fx
		
		StartCoroutine(gm.mergePieces(posX1,posY1,posX2,posY2,false)); // for visual effect mostly
		yield return new WaitForSeconds(gm.gemSwitchSpeed);
		
		float delayPerPiece = 4f;
		int mScore = 50; // the score you want to give per destroyed box in this range

		int type = 0;

		if(gm.board[posX2,posY2].piece.pd is SpecialFive){
			type = gm.board[posX1,posY1].piece.slotNum;
			StartCoroutine( gm.destroyInTimeMarked(posX2,posY2, 1.5f, mScore) );
		} else {
			type = gm.board[posX2,posY2].piece.slotNum;
			StartCoroutine( gm.destroyInTimeMarked(posX1,posY1, 1.5f, mScore) );
		}

		for(int x = 0; x < gm.boardWidth;x++){
			for(int y = 0; y < gm.boardHeight;y++)
			{
				if(gm.board[x,y].isFilled && !gm.board[x,y].piece.pd.isSpecial && 
				   gm.board[x,y].piece.slotNum == type){
					if(Random.Range(0,2) == 0){// convert the piece to this type (either vertical or horizontal)
						gm.board[x,y].piece.specialMe(gm.pieceManager.GetComponent<HorizontalPiece>());
					} else {
						gm.board[x,y].piece.specialMe(gm.pieceManager.GetComponent<VerticalPiece>());
					}
					// destroys them after the delay
					StartCoroutine(gm.destroyInTime(x,y, 1f + Random.Range(0.0f,delayPerPiece), mScore));
				}
			}
		}
	}
	// destroys a random color; explodes the star 5x5; then destroys a random color again
	IEnumerator doPowerMerge5T(int posX1, int posY1, int posX2, int posY2){
		
		gm.audioScript.playSound(PlayFx.STAR); // play this sound fx
		
		StartCoroutine(gm.mergePieces(posX1,posY1,posX2,posY2,true)); // for visual effect mostly
		yield return new WaitForSeconds(gm.gemSwitchSpeed);
		
		int colorType = 0; // specific color type to destroy by rainbow

		if(gm.board[posX2,posY2].piece.pd is SpecialFive){ // find out which piece is the special five

			colorType = gm.board[posX1,posY1].piece.slotNum; // update the color to the star's color
			StartCoroutine(gm.destroyInTimeMarked(posX2,posY2,4f,50)); // destroys them after the delay
			GamePiece ref2 = gm.board[posX2,posY2].piece; // save a reference for later
			
			if(ref2 != null){
				// visual effect for a time bomb
				Vector3 newSize = Vector3.Scale(ref2.thisPiece.transform.localScale,new Vector3(1.45f,1.45f,1f));
				LeanTween.scale( ref2.thisPiece, newSize ,0.5f, new object[]{"loopType",LeanTweenType.pingPong});
				ref2.thisPiece.GetComponent<PieceTracker>().enabled = false;
			}
			StartCoroutine( doPower5MatchColored(new int[2]{posX2,posY2},0.01f, colorType,false) ); // color specific rainbow bust
			gm.pieceManager.GetComponent<BombPiece>().doPowerTMatchBig(new int[2]{posX1,posY1}); // do the T match (big ver.) power!
			yield return new WaitForSeconds(2f); // wait for 2 secs
			gm.board[ref2.master.arrayRef[0],ref2.master.arrayRef[1]].isFalling = true; // lock the box to ignore gravity
			StartCoroutine( doPower5Match(new int[] {ref2.master.arrayRef[0],ref2.master.arrayRef[1]},
							2f) ); // randomly picks another color
		} else {
			colorType = gm.board[posX2,posY2].piece.slotNum; // update the color to the star's color
			StartCoroutine(gm.destroyInTimeMarked(posX1,posY1,4f,50)); // destroys them after the delay
			GamePiece ref1 = gm.board[posX1,posY1].piece; // save a reference for later
			
			if(ref1 != null){
				// visual effect for a time bomb
				Vector3 newSize = Vector3.Scale(ref1.thisPiece.transform.localScale,new Vector3(1.45f,1.45f,1f));
				LeanTween.scale( ref1.thisPiece, newSize ,0.5f, new object[]{"loopType",LeanTweenType.pingPong});
				ref1.thisPiece.GetComponent<PieceTracker>().enabled = false;
			}
			StartCoroutine( doPower5MatchColored(new int[2]{posX1,posY1},0.01f, colorType,false)); // color specific rainbow bust
			gm.pieceManager.GetComponent<BombPiece>().doPowerTMatchBig(new int[2]{posX2,posY2}); // do the T match (big ver.) power!
			yield return new WaitForSeconds(2f); // wait for 2 secs
			gm.board[ref1.master.arrayRef[0],ref1.master.arrayRef[1]].isFalling = true; // lock the box to ignore gravity
			StartCoroutine( doPower5Match(new int[] {ref1.master.arrayRef[0],ref1.master.arrayRef[1]},
							2f) ); // randomly picks another color
		}
	}

	// destroy all blocks power... it's a IENumerator because we want to wait for switch animation
	IEnumerator doPowerMerge55(int posX1, int posY1, int posX2, int posY2){
		
		gm.audioScript.playSound(PlayFx.STAR); // play this sound fx
		
		StartCoroutine(gm.mergePieces(posX1,posY1,posX2,posY2,false)); // for visual effect mostly
		yield return new WaitForSeconds(gm.gemSwitchSpeed);

		StartCoroutine( gm.destroyInTimeMarked(posX1,posY1,0.1f,50) );
		StartCoroutine( gm.destroyInTimeMarked(posX2,posY2,0.1f,50) );
		doPower6Match(new int[] {posX2,posY2});
	}

	// match 6 type power ( clears the entire board )
	public void doPower6Match(int[] pos){
		
		gm.audioScript.playSound(PlayFx.STAR); // play this sound fx
		
		float delayPerPiece = 0.05f;
		int mScore = 50; // the score you want to give per destroyed box in this range
		gm.animScript.doAnim(animType.BOMB,pos[0],pos[1]); // visual fx animation
		for(int x = 0; x < gm.boardWidth;x++){
			for(int y = 0; y < gm.boardHeight;y++)
			{
				// code below fans out the destruction with the bomb being the epicentre
				if( (pos[0]-x) >= 0 && (pos[1]-y) >=0 ){
					StartCoroutine(gm.destroyInTime(pos[0]-x,pos[1]-y, delayPerPiece*(x+y), mScore));
				}
				if( (pos[0]+x) < gm.boardWidth && (pos[1]+y) < gm.boardHeight ){
					StartCoroutine(gm.destroyInTime(pos[0]+x,pos[1]+y, delayPerPiece*(x+y), mScore));
				}
				if( (pos[0]-x) >= 0 && (pos[1]+y) < gm.boardHeight ){
					StartCoroutine(gm.destroyInTime(pos[0]-x,pos[1]+y, delayPerPiece*(x+y), mScore));
				}
				if( (pos[0]+x) < gm.boardWidth && (pos[1]-y) >=0 ){
					StartCoroutine(gm.destroyInTime(pos[0]+x,pos[1]-y, delayPerPiece*(x+y), mScore));
				}
			}
		}
	}
}
