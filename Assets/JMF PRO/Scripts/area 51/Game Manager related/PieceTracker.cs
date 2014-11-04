using UnityEngine;
using System.Collections;


/// <summary> ##################################
/// 
/// NOTICE :
/// This script is just a simple delegate to announce to GameManager
/// on which piece is being dragged and dragged towards which piece.
/// 
/// DO NOT TOUCH UNLESS REQUIRED
/// 
/// </summary> ##################################

public class PieceTracker : MonoBehaviour {
	
	[HideInInspector] public GameManager gm = JMFUtils.gm;
	[HideInInspector] public int[] boardPosition = new int[2]; // a tracker to keep note on which board this piece belongs too..
	bool isBeingDragged = false;
	Vector3 startTouch;
	
	public enum SwitchedWith {LEFT,RIGHT,UP,DOWN};

	void OnMouseDown() { // initiate the drag sequence from a given position
		isBeingDragged = true;
		startTouch = Input.mousePosition; // save the start position as a reference
	}

	void OnMouseUp(){ // key released... disable the check
		isBeingDragged = false;
	}

	void OnMouseUpAsButton(){
		JMFRelay.onPieceClick(boardPosition[0],boardPosition[1]);
	}

	// Update is called once per frame
	void Update () {
		if(isBeingDragged){
			if((startTouch.x - Input.mousePosition.x) > gm.size*5){ // if passed the left treshold.
				gm.draggedFromHere(boardPosition,SwitchedWith.LEFT);
				isBeingDragged = false;
			}
			else if((startTouch.x - Input.mousePosition.x) < -(gm.size*5)){ // if passed the right treshold.
				gm.draggedFromHere(boardPosition,SwitchedWith.RIGHT);
				isBeingDragged = false;
			}
			else if((startTouch.y - Input.mousePosition.y) > gm.size*5){ // if passed the down treshold.
				gm.draggedFromHere(boardPosition,SwitchedWith.DOWN);
				isBeingDragged = false;
			}
			else if((startTouch.y - Input.mousePosition.y) < -(gm.size*5)){ // if passed the up treshold.
				gm.draggedFromHere(boardPosition,SwitchedWith.UP);
				isBeingDragged = false;
			}
		}
	}
}
