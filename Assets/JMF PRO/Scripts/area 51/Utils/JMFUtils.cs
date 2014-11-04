
/// <summary>
/// JMF utils. use as a helper class for various static function calls
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class JMFUtils {

	public const string gmPanelName = "GameManagerPanel";
	public const string panelPoolName = "Panels";
	public const string piecePoolName = "Pieces";
	public const string particlePoolName = "Particles";
	public static GameManager gm; // updated by GameManager -> Awake()
	public static WinningConditions wc;  // updated by WinningConditions -> Awake()
	public static bool isPooling {get{return gm.usingPoolManager;}}

	// -----------------------------------------------------------------------------------------



	// look for an object bounds
	public static Bounds findObjectBounds(GameObject obj){
		// includes all mesh types (filter; renderer; skinnedRenderer)
		Renderer ren = obj.GetComponent<Renderer>();
		if(ren == null){
			ren = obj.GetComponentInChildren<Renderer>();
		}
		if(ren != null){
			return ren.bounds;
		}
		Debug.LogError("Your prefab" + obj.ToString() + "needs a mesh to scale!!!");
		return new Bounds(Vector3.zero,Vector3.zero); // fail safe
	}
	
	// auto scale objects to fit into a board box size
	public static void autoScale(GameObject obj){

		// auto scaling feature
		Bounds bounds = findObjectBounds(obj);
		float val = gm.size / // get the bigger size to keep ratio
			Mathf.Clamp( Mathf.Max(bounds.size.x,bounds.size.y),0.0000001f,float.MaxValue);
		obj.transform.localScale = new Vector3 (val, val, val ); // the final scale value
		
		// adjust the box collider if present...
		BoxCollider bc = obj.GetComponent<BoxCollider>();
		if ( bc != null){
			float maxSize = Mathf.Max( new float[] {bounds.size.x,bounds.size.y,bounds.size.z} );
			bc.size = new Vector3(maxSize, maxSize, bounds.size.z + 0.01f);
			bc.center = Vector3.zero;
		}
	}
	
	// auto scale objects to fit into a board box size - with padding!
	public static void autoScalePadded(GameObject obj){
		// auto scaling feature
		Bounds bounds = findObjectBounds(obj);
		// get the bigger size to keep ratio
		float val = (gm.size - gm.boxPadding) /
			Mathf.Clamp( Mathf.Max(bounds.size.x,bounds.size.y),0.0000001f,float.MaxValue);
		obj.transform.localScale = new Vector3 (val, val, val ); // the final scale value
		
		// adjust the box collider if present...
		BoxCollider bc = obj.GetComponent<BoxCollider>();
		if ( bc != null){
			float maxSize = Mathf.Max( new float[] {bounds.size.x,bounds.size.y,bounds.size.z} );
			bc.size = new Vector3(maxSize, maxSize, bounds.size.z + 0.01f);
			bc.center = Vector3.zero;
		}
	}
}
