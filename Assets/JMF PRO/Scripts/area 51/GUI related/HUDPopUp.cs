using UnityEngine;
using System.Collections;

public class HUDPopUp : MonoBehaviour {
	
	TextMesh myLabel;
	Vector3 showSize;
	Vector3 hideSize = Vector3.zero;

	// Use this for initialization
	void Awake () {
		myLabel = GetComponent<TextMesh>();
		GameManager gm = JMFUtils.gm;

		myLabel.text = "123456";
		Bounds bounds = JMFUtils.findObjectBounds( gameObject ) ; // simulate 6 chars spacing
		
		showSize = new Vector3(Mathf.Clamp(gm.size / bounds.size.x,0.0000001f,float.MaxValue),
			gm.size / Mathf.Clamp(bounds.size.x,0.0000001f,float.MaxValue),
			gm.size / Mathf.Clamp(bounds.size.z,0.0000001f,float.MaxValue) );

		transform.localScale = hideSize; // initially hidden
	}
	
	// called by external script (GameManager) to display the ScoreHUD
	public void display(int score){
		myLabel.text = "+" + score.ToString();
		StopCoroutine("showMe");
		StartCoroutine("showMe");
	}
	
	// show the score with timing
	public IEnumerator showMe(){
		transform.localScale = showSize; // show it (makes it pop-out big)
		yield return new WaitForSeconds(0.4f); // wait for time
		transform.localScale = hideSize; // end with nothing
	}
}
