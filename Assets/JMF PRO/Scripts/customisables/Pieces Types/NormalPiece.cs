using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("JMF/Pieces/NormalPiece")]
public class NormalPiece : PieceDefinition {
	
	public override bool performPower(int[] arrayRef){
		// no power to perform
		return false;
	}

	public override bool powerMatched(int posX1, int posY1, int posX2, int posY2, bool execute,
	                                  PieceDefinition thisPd, PieceDefinition otherPd){
		// no power to perform
		return false;
	}

	public override bool matchConditions(int xPos, int yPos, List<Board> linkedCubesX, List<Board> linkedCubesY){
		if ( linkedCubesX.Count > 1 || linkedCubesY.Count > 1) { // 3 matching pieces

			gm.board[xPos,yPos].destroyBox(); // nothing special...
			return true;
		}
		return false;
	}
}
