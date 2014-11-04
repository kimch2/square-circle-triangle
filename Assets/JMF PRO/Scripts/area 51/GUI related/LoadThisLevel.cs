using UnityEngine;
using System.Collections;

/// <summary> ##################################
/// 
/// NOTICE :
/// This script is a simple level loader.
/// 
/// DO NOT TOUCH UNLESS REQUIRED
/// 
/// </summary> ##################################

public class LoadThisLevel : MonoBehaviour {
	public int sceneNumber = 0; // changable in the inspector
	
	void OnMouseUpAsButton(){
		Application.LoadLevel(sceneNumber); // loads the specified level when clicked
	}
}
